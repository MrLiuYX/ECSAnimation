
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
}
