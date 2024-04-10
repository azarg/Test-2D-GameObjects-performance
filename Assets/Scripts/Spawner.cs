using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;

    public Vector2 spawnAreaSize;
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
            var pos = GetRandomPosition();
            instance.transform.position = pos;
            bullets[i] = instance.transform;
            bulletDirections[i] = Random.insideUnitCircle.normalized;
            bulletPositions[i] = pos;
        }

        behaviorManager.Initialize();
    }

    private Vector3 GetRandomPosition() {
        float x = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float y = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        return new Vector3(x, y, 0);
    }

    private void OnDisable() {
        bulletDirections.Dispose();
        bulletPositions.Dispose();
    }
}
