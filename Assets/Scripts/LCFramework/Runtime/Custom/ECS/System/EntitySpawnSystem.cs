using cfg;
using Nebukam.ORCA;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


[UpdateInGroup(typeof(EntityPoolGroup))]
[UpdateAfter(typeof(EntityUnSpawnSystem))]
public partial class EntitySpawnSystem : SystemBase
{

    [BurstCompile]
    public partial struct EntitySpawnSystemJob:IJobEntity
    {
        public NativeList<long>.ParallelWriter _spawnIdRelease;

        [ReadOnly] public NativeParallelHashMap<int, ORCALayer> LayerDict;
        [ReadOnly] public ComponentLookup<LocalTransform> _tsDataLookup;

        [NativeDisableContainerSafetyRestriction] public NativeParallelHashMap<long, NativeList<EntityAttributeBuffer>> _customData;

        [NativeDisableParallelForRestriction] public ComponentLookup<EntityAttributeComponentData> _attributeDataLookup;
        [NativeDisableParallelForRestriction] public ComponentLookup<EntityRVOAgentComponentData> _rvoDataLookup;
        [NativeDisableParallelForRestriction] public ComponentLookup<EntityLookupComponentData> _lookupDataLookup;
        [NativeDisableParallelForRestriction] public ComponentLookup<EntityStateComponentData> _stateDataLookup;
        [NativeDisableParallelForRestriction] public ComponentLookup<EntitySearchTag> _searchDataLookup;
        [NativeDisableParallelForRestriction] public ComponentLookup<HpBarComponentData> _hpBarDataLookup;
        

        [BurstCompile]
        public void Execute(InstanceTag instanceData
            , DynamicBuffer<EntityAttributeBuffer> attributeBuffer
            , Entity entity, [ChunkIndexInQuery] int sortKey
            )
        {
            _attributeDataLookup.SetComponentEnabled(entity, true);
            _tsDataLookup.TryGetComponent(entity, out var tsData);

            var maxSpeed = 0f;
            var radius = 0f;
            var LayerIgnore = ORCALayer.NONE;
            var LayerOccupation = ORCALayer.NONE;
            var CollisionEnabled = false;
            var RVOEnable = false;
            var rotSpeed = 1f;
            var initState = EntityState.Idel;
            var maxHp = 0f;
            var hasState = (long)0;
            var attackRadius = 0f;

            var attackInterval = 0f;
            var hasAttackInterval = false;
            
            var attackSpeed = 0f;
            var hasAttackSpeed = false;


            EntityTeam team = EntityTeam.Single;
            var hasTeam = false; ;

            //TODO: 改为map注册
            for (int i = 0; i < attributeBuffer.Length; i++)
            {
                var bufferData = attributeBuffer[i];
                var type = bufferData.AttributeType;
                if(type == cfg.AttributeType.MaxSpeed)
                {
                    maxSpeed = GetCustom(instanceData.SpawnId, type, out var typeValue) 
                        ? typeValue
                        : bufferData.Value;
                }
                else if(type == cfg.AttributeType.Radius)
                {
                    radius = GetCustom(instanceData.SpawnId, type, out var typeValue) 
                        ? typeValue
                        : bufferData.Value;
                }
                else if(type == cfg.AttributeType.LayerIgnore)
                {
                    LayerIgnore = GetCustom(instanceData.SpawnId, type, out var typeValue)
                        ? LayerDict[(int)typeValue]
                        : LayerDict[(int)bufferData.Value];
                }
                else if (type == cfg.AttributeType.LayerOccupation)
                {
                    LayerOccupation = GetCustom(instanceData.SpawnId, type, out var typeValue)
                        ? LayerDict[(int)typeValue]
                        : LayerDict[(int)bufferData.Value];
                }
                else if (type == cfg.AttributeType.CollisionEnabled)
                {
                    CollisionEnabled = GetCustom(instanceData.SpawnId, type, out var typeValue)
                        ? typeValue > 0
                        : bufferData.Value > 0;
                }
                else if(type == cfg.AttributeType.RVOEnable)
                {
                    RVOEnable = GetCustom(instanceData.SpawnId, type, out var typeValue)
                        ? typeValue > 0
                        : bufferData.Value > 0;
                }
                else if(type == cfg.AttributeType.RotSpeed)
                {
                    rotSpeed = GetCustom(instanceData.SpawnId, type, out var typeValue)
                        ? typeValue
                        : bufferData.Value;
                }
                else if(type == AttributeType.InitState)
                {
                    initState = GetCustom(instanceData.SpawnId, type, out var typeValue)
                        ? (EntityState)typeValue
                        : (EntityState)bufferData.Value;
                }
                else if(type == AttributeType.Team)
                {
                    team = GetCustom(instanceData.SpawnId, type, out var typeValue)
                        ? (EntityTeam)(int)typeValue
                        : (EntityTeam)(int)bufferData.Value;
                    hasTeam = true;
                }
                else if(type == AttributeType.MaxHp)
                {
                    maxHp = GetCustom(instanceData.SpawnId, type, out var typeValue)
                        ? typeValue
                        : bufferData.Value;
                }
                else if (type == AttributeType.AttackInterval)
                {
                    attackInterval = GetCustom(instanceData.SpawnId, type, out var typeValue)
                        ? typeValue
                        : bufferData.Value;
                    hasAttackInterval = true;
                }
                else if (type == AttributeType.AttackSpeed)
                {
                    attackSpeed = GetCustom(instanceData.SpawnId, type, out var typeValue)
                        ? typeValue
                        : bufferData.Value;
                    hasAttackSpeed = true;
                }
                else if(type == AttributeType.EntityStates)
                {
                    hasState = (long)bufferData.Value;
                }
                else if (type == AttributeType.AttackRadius)
                {
                    attackRadius = bufferData.Value;
                }
            }

            {
                //可能没有属性 由动态赋值的
                if(!hasTeam && GetCustom(instanceData.SpawnId,  AttributeType.Team, out var typeValue))
                {
                    team = (EntityTeam)(int)typeValue;
                    attributeBuffer.Add(new EntityAttributeBuffer()
                    {
                        AttributeType = AttributeType.Team,
                        Value = typeValue,
                    });
                }
                else if (!hasAttackInterval && GetCustom(instanceData.SpawnId, AttributeType.AttackInterval, out typeValue))
                {
                    attackInterval = typeValue;
                    attributeBuffer.Add(new EntityAttributeBuffer()
                    {
                        AttributeType = AttributeType.AttackInterval,
                        Value = typeValue,
                    });
                }
                else if (!hasAttackSpeed && GetCustom(instanceData.SpawnId, AttributeType.AttackSpeed, out typeValue))
                {
                    attackSpeed = typeValue;
                    attributeBuffer.Add(new EntityAttributeBuffer()
                    {
                        AttributeType = AttributeType.AttackSpeed,
                        Value = typeValue,
                    });
                }
            }

            if (_rvoDataLookup.HasComponent(entity))
            {
                var data = _rvoDataLookup.GetRefRW(entity);
                data.ValueRW.MaxSpeed = maxSpeed;
                data.ValueRW.CurrentSpeed = maxSpeed;
                data.ValueRW.Radius = radius;
                data.ValueRW.LayerIgnore = LayerIgnore;
                data.ValueRW.LayerOccupation = LayerOccupation;
                data.ValueRW.CollisionEnabled = CollisionEnabled;
                data.ValueRW.NavigationEnabled = RVOEnable;
                data.ValueRW.AgentIndex = int.MaxValue;                
                data.ValueRW.TargetPos = tsData.Position;
            }

            if(_lookupDataLookup.HasComponent(entity))
            {
                var data = _lookupDataLookup.GetRefRW(entity);
                data.ValueRW.RotSpeed = rotSpeed;
            }

            if (_stateDataLookup.HasComponent(entity))
            {
                var data = _stateDataLookup.GetRefRW(entity);
                data.ValueRW.CurrentEntityState = initState;
                data.ValueRW.AttackIntervalTime = attackInterval;
                data.ValueRW.AttackSpeed = attackSpeed;
                data.ValueRW.HasState = hasState;
                data.ValueRW.AttackRadius = attackRadius;
                data.ValueRW.CurrentAttackTime = attackInterval * 0.8f;

                var initSearchData = EntitySearchHelper.SearchData.Null;
                initSearchData.tsData.Position = tsData.Position;
                data.ValueRW.SearchData = initSearchData;
            }

            if(_searchDataLookup.HasComponent(entity))
            {
                var data = _searchDataLookup.GetRefRW(entity);
                data.ValueRW.Team = team;
            }

            if(_hpBarDataLookup.HasComponent(entity))
            {
                var data = _hpBarDataLookup.GetRefRW(entity);
                data.ValueRW.MaxHp = maxHp;
                data.ValueRW.CurrentHp = maxHp;
            }

            if (_customData.ContainsKey(instanceData.SpawnId))
            {
                _spawnIdRelease.AddNoResize(instanceData.SpawnId);
            }
        }

        [BurstCompile]
        private bool GetCustom(long spawnId, AttributeType type, out float typeValue)
        {
            typeValue = 0;
            if (_customData.TryGetValue(spawnId, out var value))
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i].AttributeType == type)
                    {
                        typeValue = value[i].Value;
                        return true;
                    }
                }
            }

            return false;
        }
    }

    private int _maxSpawnNumber = 30;
    private DynamicBuffer<EntityPrefabBuffer> _prefabBuffer;
    private EntityCommandBuffer _cmd;
    private NativeParallelHashMap<int, ORCALayer> LayerDict;
    private NativeList<long> _spawnIdRelease;

    protected override void OnCreate()
    {
        base.OnCreate();
        LayerDict = new NativeParallelHashMap<int, ORCALayer>(64, Allocator.Persistent);
        LayerDict.Add(0, ORCALayer.NONE);
        LayerDict.Add(1, ORCALayer.L0);
        LayerDict.Add(2, ORCALayer.L1);
        LayerDict.Add(3, ORCALayer.L2);
        LayerDict.Add(32, ORCALayer.ANY);

        EntityManager.AddComponent<SpawnHelperComponentData>(SystemHandle);
        var spawnHelper = SystemAPI.GetSingletonRW<SpawnHelperComponentData>();
        spawnHelper.ValueRW.SpawnCustomAttributeMap = new NativeParallelHashMap<long, NativeList<EntityAttributeBuffer>>(1 << 20, Allocator.Persistent);
        spawnHelper.ValueRW.FreePool = new NativeList<long>(1 << 20, Allocator.Persistent);

        _spawnIdRelease = new NativeList<long>(_maxSpawnNumber, Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        LayerDict.Dispose();
        var spawnHelper = SystemAPI.GetSingletonRW<SpawnHelperComponentData>();
        foreach (var kv in spawnHelper.ValueRW.SpawnCustomAttributeMap)
        {
            kv.Value.Dispose();
        }
        spawnHelper.ValueRW.SpawnCustomAttributeMap.Dispose();
        spawnHelper.ValueRW.FreePool.Dispose();
        _spawnIdRelease.Dispose();
    }

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

        var entityQuery = SystemAPI
            .QueryBuilder()
            .WithAll<EntityAttributeBuffer, LocalTransform, InstanceTag>()
            .WithDisabled<EntityAttributeComponentData>()
            .Build();

        var spawnHelper = SystemAPI.GetSingletonRW<SpawnHelperComponentData>();
        var customAttributeMap = spawnHelper.ValueRW.SpawnCustomAttributeMap;

        _spawnIdRelease.Clear();
        Dependency = new EntitySpawnSystemJob
        {
            LayerDict = LayerDict,
            _attributeDataLookup = SystemAPI.GetComponentLookup<EntityAttributeComponentData>(),
            _rvoDataLookup = SystemAPI.GetComponentLookup<EntityRVOAgentComponentData>(),
            _tsDataLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
            _lookupDataLookup = SystemAPI.GetComponentLookup<EntityLookupComponentData>(),
            _stateDataLookup = SystemAPI.GetComponentLookup<EntityStateComponentData>(),
            _searchDataLookup = SystemAPI.GetComponentLookup<EntitySearchTag>(),
            _hpBarDataLookup = SystemAPI.GetComponentLookup<HpBarComponentData>(),
            _customData = customAttributeMap,
            _spawnIdRelease = _spawnIdRelease.AsParallelWriter(),
        }.ScheduleParallel(entityQuery, Dependency);
        Dependency.Complete();

        for (int i = 0; i < _spawnIdRelease.Length; i++)
        {
            spawnHelper.ValueRW.ReleaseKey(_spawnIdRelease[i]);
        }
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

                    var instanceData = new InstanceTag()
                    {
                        PrefabId = prefabData.PrefabId,
                        Self = entity,
                        SpawnId = spawnData.SpawnId,
                    };

                    _cmd.AddComponent<InstanceTag>(entity, instanceData);
                    _cmd.RemoveComponent<EntityPrefabElementBuffer>(entity);
                }
                else
                {
                    entity = elementBuffer[len-1].Entity;
                    elementBuffer.RemoveAt(len-1);                    
                    prefabData.FreeCount -= 1;

                    var instanceData = new InstanceTag()
                    {
                        PrefabId = prefabData.PrefabId,
                        Self = entity,
                        SpawnId = spawnData.SpawnId,
                    };
                    _cmd.SetComponent<InstanceTag>(entity, instanceData);
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
        _cmd.AddComponent<EntityAttributeComponentData>(entity);
        _cmd.SetComponentEnabled<EntityAttributeComponentData>(entity, false);

        var entityCfg = Exl.TB.TbEntityConfig.Get(spawnData.EntityId);
        for (int i = 0; i < entityCfg.ComponentTypes.Count; i++)
        {
            var type = entityCfg.ComponentTypes[i];

            switch (type)
            {
                case cfg.ComponentType.EntityRVOAgentComponentData:
                    {
                        _cmd.AddComponent<EntityRVOAgentComponentData>(entity);
                        break;
                    }
                case cfg.ComponentType.EntityLookupComponentData:
                    {
                        _cmd.AddComponent<EntityLookupComponentData>(entity);
                        break;
                    }
                case cfg.ComponentType.EntityStateComponentData:
                    {
                        _cmd.AddComponent<EntityStateComponentData>(entity);
                        break;
                    }
                case cfg.ComponentType.EntitySearchTag:
                    {
                        _cmd.AddComponent<EntitySearchTag>(entity);
                        _cmd.SetComponentEnabled<EntitySearchTag>(entity, true);
                        break;
                    }
                case cfg.ComponentType.HpBarComponentData:
                    {
                        _cmd.AddComponent<HpBarComponentData>(entity);
                        _cmd.AddBuffer<EntityDamageBuffer>(entity);
                        break;
                    }
            }
        }

        _cmd.AddBuffer<EntityAttributeBuffer>(entity);
        var entityAttributeCfg = Exl.TB.TbEntityAttributeConfig.Get(spawnData.EntityId);
        for (int i = 0; i < entityAttributeCfg.Attributes.Count; i++)
        {
            var type = entityAttributeCfg.Attributes[i];
            var value = entityAttributeCfg.Values[i];
            _cmd.AppendToBuffer(entity, new EntityAttributeBuffer
            {
                AttributeType = type,
                Value = value,
            });
        }
    }
}
