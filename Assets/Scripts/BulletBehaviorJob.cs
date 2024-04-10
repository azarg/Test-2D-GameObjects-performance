using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public partial struct BulletBehaviorJob : IJobParallelFor
{
    public NativeArray<float3> bulletPositions;
    public NativeArray<float2> directions;
    public float deltaTime;
    public float2 spawnAreaSize;

    public void Execute(int index) {
        bulletPositions[index] += 10f * deltaTime * new float3(directions[index].x, directions[index].y, 0);

        var pos = bulletPositions[index];
        var absDirection = math.abs(directions[index]);
        if (pos.x < -spawnAreaSize.x / 2) directions[index] = new float2(absDirection.x, directions[index].y);
        if (pos.x > spawnAreaSize.x / 2) directions[index] = new float2(-absDirection.x, directions[index].y);
        if (pos.y < -spawnAreaSize.y / 2) directions[index] = new float2(directions[index].x, absDirection.y);
        if (pos.y > spawnAreaSize.y / 2) directions[index] = new float2(directions[index].x, -absDirection.y);
    }
}
