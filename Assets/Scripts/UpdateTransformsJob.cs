using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;

[BurstCompile]
public struct UpdateTransformsJob : IJobParallelForTransform
{
    [ReadOnly] public NativeArray<float3> positions;

    public void Execute(int index, TransformAccess transform) {
        transform.position = positions[index];
    }
}
