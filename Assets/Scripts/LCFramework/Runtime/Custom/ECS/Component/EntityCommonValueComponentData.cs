
using Codice.CM.Common;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct EntityCommonValueComponentData : IComponentData
{
    public float2 ScreenMin;
    public float2 ScreenMax;

    public float2 CheckScreenMin;
    public float2 CheckScreenMax;

    public float CameraDistanceRatio;

    public NativeHashMap<int, float4> ColorMap;

    /// <summary>
    /// 判断物体是否在屏幕类
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool CheckInScreen(float3 pos)
    {
        var posRel = new float2(pos.x, pos.z);
        var minCompare = posRel > CheckScreenMin;
        var maxCompare = posRel < CheckScreenMax;
        return minCompare.x && minCompare.y && maxCompare.x && maxCompare.y;
    }
}