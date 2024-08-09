

using Unity.Entities;
using Unity.Mathematics;

public struct AnimationInstanceComponentData : IComponentData
{
    public float3 pos;
    public float uvY;
    public float3 rot;
    public float scale;
}