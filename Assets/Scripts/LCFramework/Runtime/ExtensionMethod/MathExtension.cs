
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public static class MathExtension
{
    public static float3 OutRange(this Unity.Mathematics.Random random, float3 _minPos, float3 _maxPos, float outSize, ref Random useRandom)
    {
        var minPos = _minPos *( outSize + 1); 
        var maxPos = _maxPos * (outSize + 1);
        float3 value = float3.zero;
        do
        {
            value = useRandom.NextFloat3(minPos, maxPos);
        } while ((value >= _minPos).x && (value >= _minPos).y && (value >= _minPos).z
                && (value <=_maxPos).x && (value <= _maxPos).y && (value <= _maxPos).z);
        return value;
    }
}
