
using Unity.Entities;

public struct EntityPrefabBuffer : IBufferElementData
{
    public int PrefabId;
    public Entity Prefab;
    public int TotalCount;
    public int FreeCount;
    public int UseCount;
}
