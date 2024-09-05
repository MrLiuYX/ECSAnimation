using Native.Event;
using Unity.Entities;
using UnityEngine;

public partial class EntityBakeDoneSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (!SystemAPI.HasSingleton<EntityPoolTag>())
            return;
        ECSBridgeManager.ECSInitDone = true;
        var pool = SystemAPI.GetSingleton<EntityPoolTag>();
        var cmd = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        var prefabBuffer = SystemAPI.GetSingletonBuffer<EntityPrefabBuffer>();
        var rendererDictData = EntityManager.GetComponentObject<EntityAnimationRendererAssetDictComponentData>(pool.Owner);
        for (int i = 0; i < prefabBuffer.Length; i++)
        {
            var prefab = prefabBuffer[i].Prefab;
            cmd.AddBuffer<EntityPrefabElementBuffer>(prefab);
            if(SystemAPI.HasComponent<EntityAnimationRuntimeComponentData>(prefab))
            {
                var animRuntimeData = SystemAPI.GetComponentRW<EntityAnimationRuntimeComponentData>(prefab);
                var pathData = EntityManager.GetComponentData<EntityAnimationRendererPathComponentData>(prefab);
                if(rendererDictData.AssetPathDict.TryGetValue((pathData.MeshPath, pathData.MatPath), out var index))
                {
                    animRuntimeData.ValueRW.MeshAndMatIndex = index;
                }
                else
                {
                    animRuntimeData.ValueRW.MeshAndMatIndex = -1;
                }
            }
        }
        Enabled = false;
        cmd.Playback(EntityManager);
        cmd.Dispose();
    }
}