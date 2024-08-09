
using Unity.Entities;

public struct EntityUnSpawnBuffer : IBufferElementData
{
    public int PrefabId;
    public Entity Entity;
}