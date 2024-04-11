using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class BulletBehaviorManager : MonoBehaviour
{
    public float bulletSpeed = 30f;

    TransformAccessArray bulletTransformsAccessArray;
    bool initialized;

    public void Initialize() {
        initialized = true;
    }

    private void Update() {
        if (!initialized) return;

        var spawner = Spawner.Instance;

        // UPDATE BULLET POSITIONS
        var behaviorJob = new BulletBehaviorJob {
            deltaTime = Time.deltaTime,
            spawnAreaRadius = spawner.spawnAreaRadius,
            bulletPositions = spawner.bulletPositions,
            directions = spawner.bulletDirections,
            bulletStatuses = spawner.bulletStatuses,
            bulletSpeed = bulletSpeed,
        };
        var behaviorJobHandle = behaviorJob.Schedule(spawner.bulletPositions.Length, 100);
        behaviorJobHandle.Complete();

        // COUTN LIVE BULLETS, AND EXIT EARLY IF THERE ARE NONE
        int liveBulletCount = spawner.bulletStatuses.Where(b => b).Count();
        if (liveBulletCount == 0) return;

        // CREATE ACCESS ARRAY FOR LIVE BULLET TRANSFORMS
        var bulletTransforms = new Transform[liveBulletCount];
        var liveBulletPositions = new NativeArray<float3>(liveBulletCount, Allocator.TempJob);
        int cnt = 0;
        for (int i = 0; i < spawner.bullets.Length; i++) {
            if (!behaviorJob.bulletStatuses[i]) continue;           
            bulletTransforms[cnt] = spawner.bullets[i];
            liveBulletPositions[cnt] = spawner.bulletPositions[i];
            cnt++;
        }
        bulletTransformsAccessArray = new TransformAccessArray(bulletTransforms);

        // UPDATE BULLET TRANSFORMS
        var updateTransformsJob = new UpdateTransformsJob {
            positions = liveBulletPositions,
        };
        var handle = updateTransformsJob.Schedule(bulletTransformsAccessArray);
        handle.Complete();
    }

    private void OnDisable() {
        bulletTransformsAccessArray.Dispose();
    }
}
