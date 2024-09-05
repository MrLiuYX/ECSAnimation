
using Unity.Entities;
using Unity.Mathematics;

public struct EntityLookupComponentData : IComponentData, IEnableableComponent
{
    public float3 LookupDir;
    public float RotSpeed;
}