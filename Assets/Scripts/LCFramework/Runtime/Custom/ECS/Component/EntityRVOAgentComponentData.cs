
using Nebukam.Common;
using Nebukam.ORCA;
using Unity.Entities;
using Unity.Mathematics;

public struct EntityRVOAgentComponentData : IComponentData
{
    public float3 TargetPos;
    public float CurrentSpeed;

    public int AgentIndex;
    public float MaxSpeed;
    public float Radius;
    public ORCALayer LayerIgnore;
    public ORCALayer LayerOccupation;
    public bool NavigationEnabled;
    public bool CollisionEnabled;

    public float3 velocityOfORCA;
}