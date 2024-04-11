using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class EnemyBehaviorManager : MonoBehaviour
{
    TransformAccessArray enemyTransformsAccessArray;
    bool initialized;

    public void Initialize() {
        initialized = true;
    }

    private void Update() {
        if (!initialized) return;

        // UPDATE ENEMY POSITIONS
        var spawner = Spawner.Instance;
        var behaviorJob = new EnemyBehaviorJob {
            deltaTime = Time.deltaTime,
            enemyPositions = spawner.enemyPositions,
            enemyStatuses = spawner.enemyStatuses,
        };
        var behaviorJobHandle = behaviorJob.Schedule(spawner.enemies.Length, 100);
        behaviorJobHandle.Complete();

        // COUTN LIVE ENEMIES, AND EXIT EARLY IF THERE ARE NONE
        int liveEnemeyCount = spawner.enemyStatuses.Where(e => e).Count();
        if (liveEnemeyCount == 0) return;

        // CREATE TRANSFORM ACCESS ARRAY FOR LIVE ENEMIES
        var enemyTransforms = new Transform[liveEnemeyCount];
        var liveEnemyPositions = new NativeArray<float3>(liveEnemeyCount, Allocator.TempJob);
        int cnt = 0;
        for (int i = 0; i < spawner.enemies.Length; i++) {
            if (!behaviorJob.enemyStatuses[i]) continue;
            enemyTransforms[cnt] = spawner.enemies[i];
            liveEnemyPositions[cnt] = spawner.enemyPositions[i];
            cnt++;
        }
        enemyTransformsAccessArray = new TransformAccessArray(enemyTransforms);

        // UPDATE ENEMY TRANSFORMS
        var updateTransformsJob = new UpdateTransformsJob {
            positions = liveEnemyPositions,
        };
        var handle = updateTransformsJob.Schedule(enemyTransformsAccessArray);
        handle.Complete();
    }
}
