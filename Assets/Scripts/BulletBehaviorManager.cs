using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class BulletBehaviorManager : MonoBehaviour
{
    Spawner spawner;
    TransformAccessArray bulletTransformsAccessArray;
    bool initialized;

    public void Initialize() {
        spawner = Spawner.Instance;

        // Create transforms access array for the Update Transforms Job
        Transform[] bulletTransforms = new Transform[spawner.bullets.Length];
        for (int i = 0; i < spawner.bullets.Length; i++) {
            bulletTransforms[i] = spawner.bullets[i];
        }
        bulletTransformsAccessArray = new TransformAccessArray(bulletTransforms);
        initialized = true;
    }

    private void Update() {
        if (!initialized) return;

        var behaviorJob = new BulletBehaviorJob {
            deltaTime = Time.deltaTime,
            spawnAreaSize = spawner.spawnAreaSize,
            bulletPositions = spawner.bulletPositions,
            directions = spawner.bulletDirections,
        };
        var behaviorJobHandle = behaviorJob.Schedule(spawner.bulletPositions.Length, 100);
        behaviorJobHandle.Complete();

        var updateTransformsJob = new UpdateTransformsJob {
            positions = behaviorJob.bulletPositions,
            transforms = bulletTransformsAccessArray,
        };

        var handle = updateTransformsJob.Schedule(bulletTransformsAccessArray);
        handle.Complete();
    }

    private void OnDisable() {
        bulletTransformsAccessArray.Dispose();
    }
}