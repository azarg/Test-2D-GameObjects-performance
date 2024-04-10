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

    [HideInInspector] public Transform[] enemies;
    [HideInInspector] public Transform[] bullets;
    [HideInInspector] public NativeArray<float3> bulletPositions;
    [HideInInspector] public NativeArray<float2> bulletDirections;

    public BulletBehaviorManager behaviorManager;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        enemies = new Transform[maxEnemies];
        bullets = new Transform[maxBullets];
        bulletDirections = new NativeArray<float2>(maxBullets, Allocator.Persistent);
        bulletPositions = new NativeArray<float3>(maxBullets, Allocator.Persistent);

        for(int i = 0; i < maxEnemies; i++) {
            var instance = Instantiate(enemyPrefab);
            instance.transform.position = GetRandomPosition();
            enemies[i] = instance.transform;
        }

        for(int i = 0;i < maxBullets; i++) {
            var instance = Instantiate(bulletPrefab);

            instance.transform.position = GetRandomPosition();
            bullets[i] = instance.transform;
            bulletDirections[i] = Random.insideUnitCircle.normalized;
            bulletPositions[i] = instance.transform.position;
        }

        behaviorManager.Initialize();
    }

    private Vector2 GetRandomPosition() {
        var pos = Random.insideUnitCircle * spawnAreaRadius;
        return pos;
    }

    private void OnDisable() {
        bulletDirections.Dispose();
        bulletPositions.Dispose();
    }
}
