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
    public float spawnAreaRadius;

    public void Execute(int index) {
        bulletPositions[index] += 10f * deltaTime * new float3(directions[index].x, directions[index].y, 0);

        var pos = bulletPositions[index];
        if (math.length(pos) > spawnAreaRadius) {

            //TODO: this may cause bullets getting stuck outside of the circle
            directions[index] = -directions[index];
        }
    }
}
