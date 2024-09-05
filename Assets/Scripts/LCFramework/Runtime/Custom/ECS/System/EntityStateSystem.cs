using cfg;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using static EntitySearchHelper;


[UpdateInGroup(typeof(EntityLogicGroup))]
[UpdateBefore(typeof(EntityRVOSystem))]
[BurstCompile]
public partial struct EntityStateSystem : ISystem
{
    [BurstCompile]
    public partial struct EntityStateSystemJob : IJobEntity
    {
        [ReadOnly] public EntityIdleState IdelState;
        [ReadOnly] public EntityMoveState MoveState;
        [ReadOnly] public EntityAttackState AttackState;
        [ReadOnly] public float DeltaTime;

        [ReadOnly] public NativeParallelHashMap<int, int> _stateAnimationMap;

        [NativeDisableParallelForRestriction] public ComponentLookup<EntityRVOAgentComponentData> _rvoDataLookup;
        [NativeDisableParallelForRestriction] public ComponentLookup<EntitySearchTag> _searchTagLookup;
        [NativeDisableParallelForRestriction] public DynamicBuffer<EntityUnSpawnBuffer> _unSpawnBuffer;
        [ReadOnly] public ComponentLookup<HpBarComponentData> _hpDataLookup;


        [BurstCompile]
        public void Execute(
            InstanceTag instanceData
            , ref EntityStateComponentData stateData
            , ref EntityAnimationRuntimeComponentData animationData
            , ref LocalTransform tsData
            , Entity entity
            )
        {
            stateData.CurrentAttackTime += DeltaTime;

            EntityState state = stateData.CurrentEntityState;

            if (state == EntityState.Die)
            {
                stateData.CurrentDeadTime += DeltaTime;

                if (stateData.CurrentDeadTime >= 1)
                {
                    _unSpawnBuffer.Add(new EntityUnSpawnBuffer()
                    {
                        Entity = instanceData.Self,
                        PrefabId = instanceData.PrefabId,
                    });
                    return;
                }
            }

            var team = EntityTeam.Single;
            var searchData = new RefRW<EntitySearchTag>();

            if (_searchTagLookup.HasComponent(entity))
            {
                searchData = _searchTagLookup.GetRefRW(entity);
                team = searchData.ValueRO.Team;
            }

            var dead = false;
            if (_hpDataLookup.TryGetComponent(entity, out var hpBarData))
            {
                dead = hpBarData.CurrentHp <= 0;
            }

            if (_searchTagLookup.HasComponent(stateData.SearchData.entity) && !_searchTagLookup.IsComponentEnabled(stateData.SearchData.entity))
            {
                stateData.SearchData = SearchData.Null;
                stateData.SearchData.tsData.Position = tsData.Position;
            }

            var conditionProvider = new EntityNormalStateConditionProvider(tsData, stateData, entity, team, dead);

            switch (stateData.CurrentEntityState)
            {
                case EntityState.Idel:
                    {
                        IdelState.EntityStateUpdate(conditionProvider, ref state);
                        break;
                    }
                case EntityState.Move:
                    {
                        MoveState.EntityStateUpdate(conditionProvider, ref state);
                        break;
                    }
                case EntityState.Attack:
                    {
                        AttackState.EntityStateUpdate(conditionProvider, ref state);
                        break;
                    }
                default: break;
            }

            if (_rvoDataLookup.HasComponent(entity))
            {
                var rvoData = _rvoDataLookup.GetRefRW(entity);
                rvoData.ValueRW.NavigationEnabled = state == EntityState.Move;
                rvoData.ValueRW.CollisionEnabled = state != EntityState.Attack && state != EntityState.Die;
            }

            if (animationData.CurrentAnimationId == (int)AnimationType.Attack
                //保证攻击动画播放到80%(后摇 前摇)
                && (animationData.Time <= animationData.CurrentConfig.TotalSec * 0.8f)
                && state != EntityState.Die)
            {
                animationData.NextAnimationId = (int)AnimationType.Attack;

                if (searchData.IsValid)
                {
                    searchData.ValueRW.NeedSearch = false;
                }
            }
            else
            {
                animationData.NextAnimationId = _stateAnimationMap[(int)state];

                if (searchData.IsValid)
                {
                    searchData.ValueRW.NeedSearch = state != EntityState.Die;
                }
            }

            if (stateData.CurrentEntityState == state) return;

            stateData.CurrentEntityState = state;

            if (state == EntityState.Attack) stateData.CurrentAttackTime = 0;

            if (state == EntityState.Die)
            {
                stateData.SearchData = SearchData.Null;
            }
        }
    }

    private NativeParallelHashMap<int, int> _stateAnimationMap;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _stateAnimationMap = new NativeParallelHashMap<int, int>(128, Allocator.Persistent);
        _stateAnimationMap.Add((int)EntityState.Idel, (int)AnimationType.Idel);
        _stateAnimationMap.Add((int)EntityState.Move, (int)AnimationType.Move);
        _stateAnimationMap.Add((int)EntityState.Attack, (int)AnimationType.Attack);
        _stateAnimationMap.Add((int)EntityState.Die, (int)AnimationType.Die);
    }

    [BurstCompile]
    public void OnDestroy()
    {
        _stateAnimationMap.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency = new EntityStateSystemJob()
        {
            _stateAnimationMap = _stateAnimationMap,
            DeltaTime = SystemAPI.Time.DeltaTime,
            _rvoDataLookup = SystemAPI.GetComponentLookup<EntityRVOAgentComponentData>(),
            _searchTagLookup = SystemAPI.GetComponentLookup<EntitySearchTag>(),
            _hpDataLookup = SystemAPI.GetComponentLookup<HpBarComponentData>(),
            _unSpawnBuffer = SystemAPI.GetSingletonBuffer<EntityUnSpawnBuffer>(),
        }.ScheduleParallel(state.Dependency);
    }
}