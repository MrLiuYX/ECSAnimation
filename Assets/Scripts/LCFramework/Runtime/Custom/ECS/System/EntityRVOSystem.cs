
using Nebukam.ORCA;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(EntityLogicGroup))]
public partial class EntityRVOSystem : SystemBase
{
    private float3 Float3DotOne = new float3(1, 1, 1);

    private ObstacleGroup _obstacles;
    private ORCA _simulation;
    private bool First = true;
    private bool ForceRun = false;
    private NativeArray<Entity> _rvoEntities;

    protected override void OnCreate()
    {
        base.OnCreate();

        _obstacles = new ObstacleGroup();
        _simulation = new ORCA();
        _simulation.staticObstacles = _obstacles;
        _simulation.plane = Nebukam.Common.AxisPair.XZ;
        _simulation.AgentKDTree.AgentProvider.MakeLength(10000);
    }

    protected override void OnUpdate()
    {
        if (_simulation.TryComplete() || First)
        {
            First = false;

            if(_rvoEntities.Length >= 1)
            {
                new EntityRVOSystemSyncJob()
                {
                    AgentResultDatas = _simulation.ORCALines.results.AsReadOnly(),
                    _rvoEntities = _rvoEntities.AsReadOnly(),
                    DeltaTime = SystemAPI.Time.DeltaTime,
                    _orcaData = _simulation.AgentKDTree.AgentProvider.m_outputAgents.AsReadOnly(),
                    _lookupDataLookup = SystemAPI.GetComponentLookup<EntityLookupComponentData>(),
                    _rvoDataLookup = SystemAPI.GetComponentLookup<EntityRVOAgentComponentData>(),
                    _tsDataLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
                }.Schedule(_rvoEntities.Length, 10, Dependency).Complete();
                _rvoEntities.Dispose();
            }

            var entityQuery = SystemAPI.QueryBuilder().WithAll<InstanceTag, EntityRVOAgentComponentData>().Build();
            _rvoEntities = entityQuery.ToEntityArray(Allocator.Persistent);

            _simulation.AgentKDTree.AgentProvider.MakeLength(_rvoEntities.Length);

            Dependency = new EntityRVOSystemInjectJob()
            {
                _outputAgents = _simulation.AgentKDTree.AgentProvider.m_outputAgents
            }.ScheduleParallel(Dependency);
            Dependency.Complete();

            _simulation.Schedule(SystemAPI.Time.DeltaTime);
        }
    }

    public void Clear()
    {
        _simulation.DisposeAll();
    }

    [BurstCompile]
    public partial struct EntityRVOSystemSyncJob:IJobParallelFor
    {
        public NativeArray<AgentDataResult>.ReadOnly AgentResultDatas;
        public NativeArray<AgentData>.ReadOnly _orcaData;
        public NativeArray<Entity>.ReadOnly _rvoEntities;

        [NativeDisableParallelForRestriction] public ComponentLookup<EntityRVOAgentComponentData> _rvoDataLookup;
        [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> _tsDataLookup;
        [NativeDisableParallelForRestriction] public ComponentLookup<EntityLookupComponentData> _lookupDataLookup; 

        [ReadOnly] public float DeltaTime;

        [BurstCompile]
        public void Execute(int index)
        {
            var entity = _rvoEntities[_orcaData[index].index];

            if (!_rvoDataLookup.HasComponent(entity)) return;
            var rvoData = _rvoDataLookup.GetRefRW(entity);
            var tsData = _tsDataLookup.GetRefRW(entity);

            if (_lookupDataLookup.HasComponent(entity))
            {
                var lookUpData = _lookupDataLookup.GetRefRW(entity);
                lookUpData.ValueRW.LookupDir = math.normalize(rvoData.ValueRO.TargetPos - tsData.ValueRO.Position);
            }

            if (rvoData.ValueRO.AgentIndex >= AgentResultDatas.Length)
            {
                return;
            }

            var orcaResultData = AgentResultDatas[index];

            var newPos = new float3(orcaResultData.position.x, 0, orcaResultData.position.y);
            var newVelocity = new float3(orcaResultData.velocity.x, 0, orcaResultData.velocity.y);

            rvoData.ValueRW.velocityOfORCA = newVelocity;
            tsData.ValueRW.Position = newPos;
        }
    }

    [BurstCompile]
    public partial struct EntityRVOSystemInjectJob : IJobEntity
    {

        public NativeArray<AgentData> _outputAgents;

        [BurstCompile]
        public void Execute(InstanceTag _            
                , ref EntityRVOAgentComponentData rvoData
                , ref LocalTransform tsData
                , ref EntityStateComponentData stateData
                , [EntityIndexInQuery] int index
            )
        {
            var prefV = math.distancesq(rvoData.TargetPos, tsData.Position) < 0.01f || stateData.CurrentEntityState != cfg.EntityState.Move
                ? float3.zero
                : math.normalize(rvoData.TargetPos - tsData.Position) * rvoData.CurrentSpeed;

            rvoData.AgentIndex = index;
            _outputAgents[index] = new AgentData()
            {
                index = index,
                position = new float2(tsData.Position.x, tsData.Position.z),
                worldPosition = tsData.Position,
                baseline = tsData.Position.y,
                prefVelocity = new float2(prefV.x, prefV.z),
                velocity = new float2(rvoData.velocityOfORCA.x, rvoData.velocityOfORCA.z),
                worldVelocity = rvoData.velocityOfORCA,
                radius = rvoData.Radius,
                radiusObst = rvoData.Radius * 0.5f,
                maxSpeed = rvoData.MaxSpeed,
                layerIgnore = rvoData.LayerIgnore,
                layerOccupation = rvoData.LayerOccupation,
                navigationEnabled = stateData.CurrentEntityState == cfg.EntityState.Move,
                collisionEnabled = rvoData.CollisionEnabled,

                height = 0f,
                maxNeighbors = 15,
                neighborDist = rvoData.Radius * 5,
                neighborElev = 0.5f,
                timeHorizon = 1f,
                timeHorizonObst = 1f,
            };
        }
    }
}