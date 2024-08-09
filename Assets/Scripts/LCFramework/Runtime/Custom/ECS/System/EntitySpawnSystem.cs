using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(EntityPoolGroup))]
[UpdateAfter(typeof(EntityUnSpawnSystem))]
public partial class EntitySpawnSystem : SystemBase
{
    private int _maxSpawnNumber = 100;
    private DynamicBuffer<EntityPrefabBuffer> _prefabBuffer;
    private EntityCommandBuffer _cmd;    

    protected override void OnUpdate()
    {
        _prefabBuffer = SystemAPI.GetSingletonBuffer<EntityPrefabBuffer>();
        _cmd = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        var spawnBuffer = SystemAPI.GetSingletonBuffer<EntitySpawnBuffer>();
        var spawnStop = spawnBuffer.Length - math.min(spawnBuffer.Length, _maxSpawnNumber);
        for (int i = spawnBuffer.Length - 1; i >= spawnStop; i--)
        {
            var spawnData = spawnBuffer[i];
            var entity = Spawn(spawnData);
            if (entity == Entity.Null)
            {
                Debug.LogError($"[mmmyyliu] spawn的prefab有问题，未找到对应的PrefabId{spawnData.PrefabId}");
                goto Continue;
            }            

            ComponentSet(entity, spawnData);

        Continue:
            {
                spawnBuffer.RemoveAt(i);
            }
        }

        _cmd.Playback(EntityManager);
        _cmd.Dispose();
    }

    private Entity Spawn(EntitySpawnBuffer spawnData)
    {
        var entity = Entity.Null;
        for (int i = 0; i < _prefabBuffer.Length; i++)
        {
            var prefabData = _prefabBuffer[i];

            //TODO:改成哈希
            if(prefabData.PrefabId == spawnData.PrefabId)
            {
                var elementBuffer = SystemAPI.GetBuffer<EntityPrefabElementBuffer>(prefabData.Prefab);
                var len = elementBuffer.Length;
                prefabData.TotalCount += 1;
                prefabData.UseCount += 1;
                if (len == 0)
                {
                    entity = _cmd.Instantiate(prefabData.Prefab);
                    _cmd.AddComponent<InstanceTag>(entity, new InstanceTag()
                    {
                        PrefabId = prefabData.PrefabId,
                        Self = entity,
                    });
                    _cmd.RemoveComponent<EntityPrefabElementBuffer>(entity);
                }
                else
                {
                    entity = elementBuffer[len-1].Entity;
                    elementBuffer.RemoveAt(len-1);                    
                    prefabData.FreeCount -= 1;
                }
                _prefabBuffer[i] = prefabData;
                _cmd.SetComponentEnabled<InstanceTag>(entity, true);
            }
        }
        return entity;
    }

    private void ComponentSet(Entity entity, EntitySpawnBuffer spawnData)
    {
        _cmd.SetComponent<LocalTransform>(entity, spawnData.TsData);
    }
}
