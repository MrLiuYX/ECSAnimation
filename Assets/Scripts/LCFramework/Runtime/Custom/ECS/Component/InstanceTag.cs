
using Unity.Entities;

/// <summary>
/// true则为池子外
/// </summary>
public struct InstanceTag : IComponentData, IEnableableComponent 
{
    public Entity Self;
    public int PrefabId;
}
