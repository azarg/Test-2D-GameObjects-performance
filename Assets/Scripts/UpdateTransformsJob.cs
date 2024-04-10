using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine.Jobs;

public struct UpdateTransformsJob : IJobParallelForTransform
{
    [ReadOnly] public NativeArray<float3> positions;

    [NativeDisableUnsafePtrRestriction]
    public TransformAccessArray transforms;

    public void Execute(int index, TransformAccess transform) {
        transform.position = positions[index];
    }
}
