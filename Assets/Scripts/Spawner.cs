using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;

    public float spawnAreaRadius;
    public GameObject enemyPrefab;
    public GameObject bulletPrefab;

    public int maxEnemies = 1000;
    public int maxBullets = 1000;
    public int defaultBulletSpawnCount = 30;
    public int defaultEnemySpawnCount = 30; 

    [HideInInspector] public Transform[] enemies;
    [HideInInspector] public Transform[] bullets;
    public NativeArray<float3> bulletPositions;
    public NativeArray<float2> bulletDirections;
    public NativeArray<bool> bulletStatuses;

    public NativeArray<float3> enemyPositions;
    public NativeArray<bool> enemyStatuses;

    public BulletBehaviorManager bulletBehaviorManager;
    public EnemyBehaviorManager enemyBehaviorManager;

    public int currentEnemyCount = 0;
    public int currentBulletCount = 0;

    public void AddEnemies(int count) {
        //TODO: add object pooling
        for (int i = 0; i < count; i++) {
            var instance = Instantiate(enemyPrefab);
            instance.transform.position = Random.insideUnitCircle.normalized * spawnAreaRadius;
            var behavior = instance.GetComponent<EnemyBehavior>();

            //TODO: replace with stack of empty array positions
            for (int j = 0; j < maxEnemies; j++) {
                if (enemyStatuses[j] == false) {
                    enemies[j] = instance.transform;
                    enemyPositions[j] = instance.transform.position;
                    enemyStatuses[j] = true;
                    behavior.index = j;
                    break;
                }
            }
        }
    }

    public void AddBullets(int count) {
        //TODO: add object pooling
        for (int i = 0; i < count; i++) {
            var instance = Instantiate(bulletPrefab);
            instance.transform.position = Vector3.zero;
            var behavior = instance.GetComponent<BulletBehavior>();

            //TODO: replace with stack of empty array positions
            for (int j = 0; j < maxBullets; j++) {
                if (bulletStatuses[j] == false) {
                    bullets[j] = instance.transform;
                    bulletDirections[j] = Random.insideUnitCircle.normalized;
                    bulletPositions[j] = instance.transform.position;
                    bulletStatuses[j] = true;
                    behavior.index = j;
                    break;
                }
            }
        }
    }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        enemies = new Transform[maxEnemies];
        bullets = new Transform[maxBullets];
        bulletDirections = new NativeArray<float2>(maxBullets, Allocator.Persistent);
        bulletPositions = new NativeArray<float3>(maxBullets, Allocator.Persistent);
        bulletStatuses = new NativeArray<bool>(maxBullets, Allocator.Persistent);
        enemyPositions = new NativeArray<float3>(maxEnemies, Allocator.Persistent);
        enemyStatuses = new NativeArray<bool>(maxEnemies, Allocator.Persistent);

        bulletBehaviorManager.Initialize();
        enemyBehaviorManager.Initialize();
    }

    private void Update() {
        // destroy game objects of enemies marked for deletion
        for (int i = 0; i < enemyStatuses.Length; i++) {
            if (enemyStatuses[i] == false) { 
                if (enemies[i] != null) {
                    Destroy(enemies[i].gameObject);
                }
            }
        }

        // spawn new enemies
        currentEnemyCount = enemyStatuses.Where(b => b).Count();
        if (currentEnemyCount < maxEnemies) {
            int count = math.min(defaultEnemySpawnCount, maxEnemies - currentEnemyCount);
            AddEnemies(count);
        }

        // destroy game objects of bullets marked for deletion
        for (int i = 0; i < bulletStatuses.Length; i++) {
            if (bulletStatuses[i] == false) {
                if (bullets[i] != null) {
                    Destroy(bullets[i].gameObject);
                }
            }
        }
        // spawn new bullets
        currentBulletCount = bulletStatuses.Where(b => b).Count();
        if (currentBulletCount < maxBullets) {
            int count = math.min(defaultBulletSpawnCount, maxBullets - currentBulletCount);
            AddBullets(count);
        }
    }

    private void OnDisable() {
        bulletDirections.Dispose();
        bulletPositions.Dispose();
        bulletStatuses.Dispose();
        enemyPositions.Dispose();
        enemyStatuses.Dispose();
    }
}
