

using Unity.Entities;
using Unity.Mathematics;

public struct EntityAnimationInstanceComponentData : IComponentData
{
    public float3 pos;
    public float uvY;

    public float3 rot;
    public float scale;

    public float lastUVY;
    public float lerpValue;
    public float flashWhite;
    public float None;
}