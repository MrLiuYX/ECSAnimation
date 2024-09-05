
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(EntityLogicGroup))]
[UpdateBefore(typeof(EntityStateSystem))]
public partial struct EntitySearchSystem : ISystem
{
    [BurstCompile]
    public partial struct EntitySearchSystemJob : IJobEntity
    {
        [ReadOnly] public EntitySearchHelper SearchHelper;
        [ReadOnly] public float DeltaTime;

        [NativeDisableParallelForRestriction] public ComponentLookup<EntityStateComponentData> _stateDataLookup;
        [NativeDisableParallelForRestriction] public ComponentLookup<EntityRVOAgentComponentData> _rvoDataLookup;

        [BurstCompile]
        public void Execute(InstanceTag _
                , ref EntitySearchTag searchData
                , LocalTransform tsData
                , Entity entity
            )
        {
            searchData.SearchInterval -= DeltaTime;
            if (!searchData.NeedSearch || searchData.SearchInterval > 0) return;

            if (SearchHelper.QueryEntityNearestExceptTeam(searchData.Team, tsData.Position, out var nearest))
            {
                if (_rvoDataLookup.HasComponent(entity))
                {
                    var rvoData = _rvoDataLookup.GetRefRW(entity);
                    rvoData.ValueRW.TargetPos = nearest.tsData.Position;
                }

                if (_stateDataLookup.HasComponent(entity))
                {
                    var stateData = _stateDataLookup.GetRefRW(entity);
                    stateData.ValueRW.SearchData = nearest;
                }

                searchData.SearchInterval = 0.1f;
            }
        }
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var data = SystemAPI.GetSingletonRW<EntitySearchHelper>();
        state.Dependency = new EntitySearchSystemJob
        {
            SearchHelper = data.ValueRO,
            DeltaTime = SystemAPI.Time.DeltaTime,
            _stateDataLookup = SystemAPI.GetComponentLookup<EntityStateComponentData>(),
            _rvoDataLookup = SystemAPI.GetComponentLookup<EntityRVOAgentComponentData>(),
        }.ScheduleParallel(state.Dependency);
        state.Dependency.Complete();
    }
}
