
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using static EntitySearchHelper;
using UnityEngine;

[UpdateBefore(typeof(EntityLogicGroup))]
[UpdateAfter(typeof(TransformSystemGroup))]
[BurstCompile]
public partial struct EntityGridSystem : ISystem
{
    public const float Size = 5;

    [BurstCompile]
    public partial struct EntityGridSystemJob:IJobEntity
    {        
        public NativeParallelMultiHashMap<int3, SearchData>.ParallelWriter _datas;
        public NativeParallelHashSet<int3>.ParallelWriter _keys;

        [BurstCompile]
        public void Execute(InstanceTag _
                , LocalTransform tsData
                , EntitySearchTag searchData
                , DynamicBuffer<EntityAttributeBuffer> attributeBuffer
                , Entity entity
            )
        {
            var radius = 0f;
            for (int i = 0; i < attributeBuffer.Length; i++)
            {
                var bufferData = attributeBuffer[i];
                if (bufferData.AttributeType == cfg.AttributeType.Radius)
                {
                    radius = bufferData.Value;
                    break;
                }
            }

            var gridPosMin = (tsData.Position + new float3(-1, 0, -1) * radius) / Size;
            var gridPosMax = (tsData.Position + new float3(1, 0, 1) * radius) / Size;

            var keyMin = new int2((int)math.round(gridPosMin.x), (int)math.round(gridPosMin.z));
            var keyMax = new int2((int)math.round(gridPosMax.x), (int)math.round(gridPosMax.z));

            var keys = new NativeList<int2>(Allocator.Temp);
            for (int x = keyMin.x; x <= keyMax.x; x++)
            {
                for (int z = keyMin.y; z <= keyMax.y; z++)
                {
                    var key = new int2(x, z);
                    if (!keys.Contains(key)) keys.Add(key);
                }
            }

            for (int i = 0; i < keys.Length; i++)
            {
                var key = new int3(keys[i].x, keys[i].y, (int)searchData.Team);

                _datas.Add(key, new SearchData()
                {
                    entity = entity,
                    tsData = tsData,
                    Team = searchData.Team,
                    Radius = radius,
                });

                _keys.Add(key);
            }

            keys.Dispose();
        }
    }

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.EntityManager.AddComponent<EntitySearchHelper>(state.SystemHandle);
        var data = SystemAPI.GetSingletonRW<EntitySearchHelper>();
        data.ValueRW.EntitySearchMap = new NativeParallelMultiHashMap<int3, SearchData>(1 << 20, Allocator.Persistent);
        data.ValueRW.Keys = new NativeParallelHashSet<int3>(1 << 12, Allocator.Persistent);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        var data = SystemAPI.GetSingletonRW<EntitySearchHelper>();
        data.ValueRW.EntitySearchMap.Dispose();
        data.ValueRW.Keys.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var data = SystemAPI.GetSingletonRW<EntitySearchHelper>();
        data.ValueRW.EntitySearchMap.Clear();
        data.ValueRW.Keys.Clear();

        state.Dependency = new EntityGridSystemJob()
        { 
             _datas = data.ValueRW.EntitySearchMap.AsParallelWriter(),
             _keys = data.ValueRW.Keys.AsParallelWriter(),
        }.ScheduleParallel(state.Dependency);
        state.Dependency.Complete();
    }
}