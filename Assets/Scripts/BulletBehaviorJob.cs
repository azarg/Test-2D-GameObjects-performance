using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public partial struct BulletBehaviorJob : IJobParallelFor
{
    public NativeArray<float3> bulletPositions;
    public NativeArray<float2> directions;
    public NativeArray<bool> bulletStatuses;
    public float bulletSpeed;

    public float deltaTime;
    public float spawnAreaRadius;

    public void Execute(int index) {
        // do not process if bullet is marked for deletion
        if (bulletStatuses[index] == false) return;

        bulletPositions[index] += bulletSpeed * deltaTime * new float3(directions[index].x, directions[index].y, 0);

        // mark bullet for deletion if it passes the spawn area border
        if (math.length(bulletPositions[index]) > spawnAreaRadius) {
            bulletStatuses[index] = false;
        }
    }
}
