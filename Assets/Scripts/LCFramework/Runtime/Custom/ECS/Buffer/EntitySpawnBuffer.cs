using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct EntitySpawnBuffer : IBufferElementData
{
    public int EntityId;
    public int PrefabId;
    public LocalTransform TsData;

    //Spawn操作需要在Mono层进行特殊逻辑处理
    public long SpawnId;
}