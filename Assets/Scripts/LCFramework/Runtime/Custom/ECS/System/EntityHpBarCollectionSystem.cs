
using cfg;
using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using static Unity.Collections.NativeSortExtension;

[UpdateInGroup(typeof(EntityAnimationGroup))]
[BurstCompile]
public partial struct EntityHpBarCollectionSystem : ISystem
{
    [BurstCompile]
    public partial struct EntityHpBarCollectionSystemJob : IJobEntity
    {

        [NativeDisableContainerSafetyRestriction] public NativeList<HpBarComponentData>.ParallelWriter Datas;

        [ReadOnly] public EntityCommonValueComponentData _commonValueData;

        [BurstCompile]
        public void Execute(
                HpBarComponentData hpBarData
                , LocalTransform tsData
                , Entity entity
                , DynamicBuffer<EntityAttributeBuffer> attributeBuffer
            )
        {

            var team = EntityTeam.Single;
            for (int i = 0; i < attributeBuffer.Length; i++)
            {
                var attributeBufferData = attributeBuffer[i];
                if(attributeBufferData.AttributeType == AttributeType.Team)
                {
                    team = (EntityTeam)(int)attributeBufferData.Value;
                    break;
                }
            }

            //屏幕内才渲染
            if (!_commonValueData.CheckInScreen(tsData.Position) || hpBarData.CurrentHp <= 0)
            {
                return;
            }
            Datas.AddNoResize(HpBarComponentData.Create(hpBarData.MaxHp, hpBarData.CurrentHp, tsData.Position, entity.Index, team, _commonValueData.CameraDistanceRatio));
        }
    }

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.EntityManager.AddComponent<HpBarCollectionComponentData>(state.SystemHandle);
        var data=  SystemAPI.GetSingletonRW<HpBarCollectionComponentData>();
        data.ValueRW.Datas = new NativeList<HpBarComponentData>(1 << 20, Allocator.Persistent);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        var data = SystemAPI.GetSingletonRW<HpBarCollectionComponentData>();
        data.ValueRW.Datas.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var data = SystemAPI.GetSingletonRW<HpBarCollectionComponentData>();
        data.ValueRW.Datas.Clear();
        state.Dependency = new EntityHpBarCollectionSystemJob()
        {
            Datas = data.ValueRW.Datas.AsParallelWriter(),
            _commonValueData = SystemAPI.GetSingleton<EntityCommonValueComponentData>(),
        }.ScheduleParallel(state.Dependency);
        state.Dependency.Complete();        
        data.ValueRW.Datas.Sort();
    }
}

public struct HpBarCollectionComponentData : IComponentData
{
    public NativeList<HpBarComponentData> Datas;
}