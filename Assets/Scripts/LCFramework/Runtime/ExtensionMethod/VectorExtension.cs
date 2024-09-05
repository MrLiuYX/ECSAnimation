
using Unity.Mathematics;
using UnityEngine;

public static class VectorExtension
{
    public static Vector3 RotZ(this Vector3 vector, float angle)
    {
        return Quaternion.Euler(0, 0, angle) * vector;
    }

    public static float3 ToFloat3(this Vector3 vecotr)
    {
        return new float3(vecotr.x, vecotr.y, vecotr.z);
    }

    public static float2 ToFloat2(this Vector3 vecotr)
    {
        return new float2(vecotr.x, vecotr.z);
    }

    public static int2 ToInt2(this Vector3 vecotr)
    {
        return new int2((int)vecotr.x, (int)vecotr.z);
    }

    public static int2 ToInt2(this Vector3 vecotr, float size)
    {
        return new int2(Mathf.RoundToInt(vecotr.x/ size), Mathf.RoundToInt(vecotr.z/ size));
    }

    public static Vector3 ToVecotr3(this int2 @int2)
    {
        return new Vector3(int2.x, 0, int2.y);
    }
}
