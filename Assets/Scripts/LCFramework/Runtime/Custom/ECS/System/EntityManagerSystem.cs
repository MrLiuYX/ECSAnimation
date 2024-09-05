
using Unity.Collections;
using Unity.Entities;

public partial class EntityManagerSystem : SystemBase
{

    public void ClearAllEntity()
    {
        var spawnBuffer =  SystemAPI.GetSingletonBuffer<EntitySpawnBuffer>();
        var unSpawnBuffer = SystemAPI.GetSingletonBuffer<EntityUnSpawnBuffer>();

        spawnBuffer.Clear();

        foreach (var instanceData in SystemAPI.Query<InstanceTag>())
        {
            unSpawnBuffer.Add(new EntityUnSpawnBuffer()
            {
                Entity = instanceData.Self,
                PrefabId = instanceData.PrefabId,
            });
        }
    }

    protected override void OnUpdate()
    {
        
    }

    public int GetEntityCount()
    {
        return SystemAPI.QueryBuilder().WithAll<InstanceTag>().Build().ToEntityArray(Allocator.Persistent).Length;
    }
}