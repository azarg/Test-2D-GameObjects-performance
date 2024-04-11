using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;


[BurstCompile]
public partial struct EnemyBehaviorJob : IJobParallelFor
{
    public NativeArray<float3> enemyPositions;
    public NativeArray<bool> enemyStatuses;

    public float deltaTime;

    public void Execute(int index) {
        // do not process if enemy status is false, i.e. enemy marked for deletion
        if (enemyStatuses[index] == false) return;

        // move towards center, i.e. (0,0,0)
        enemyPositions[index] += 20f * deltaTime * -math.normalize(enemyPositions[index]);
    }
}
