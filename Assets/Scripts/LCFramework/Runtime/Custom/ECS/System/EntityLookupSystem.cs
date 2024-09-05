using Unity.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine.Rendering;
using UnityEngine;

[UpdateInGroup(typeof(EntityLogicGroup))]
[UpdateAfter(typeof(EntityRVOSystem))]
[BurstCompile]
public partial struct EntityLookupSystem : ISystem
{

    [BurstCompile]
    public partial struct EntityLookupSystemJob : IJobEntity
    {
        [ReadOnly] public float _deltaTime;

        [BurstCompile]
        public void Execute(
            ref LocalTransform tsData
            , ref EntityLookupComponentData lookupData
            )
        {            
            tsData.Rotation = quaternion.LookRotationSafe(math.lerp(tsData.Forward(), lookupData.LookupDir, _deltaTime * lookupData.RotSpeed * 10), tsData.Up());
        }
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency = new EntityLookupSystemJob()
        {
            _deltaTime = SystemAPI.Time.DeltaTime,
        }.ScheduleParallel(state.Dependency);
        state.Dependency.Complete();
    }
}