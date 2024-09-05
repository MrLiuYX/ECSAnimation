
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(EntityLogicGroup))]
[UpdateBefore(typeof(EntityStateSystem))]
[BurstCompile]
public partial struct EntityDamageSystem : ISystem
{
    [BurstCompile]
    public partial struct EntityDamageSystemJob : IJobEntity
    {
        [NativeDisableParallelForRestriction] public ComponentLookup<EntitySearchTag> _searchDataLookup;
        [NativeDisableParallelForRestriction] public ComponentLookup<EntityAnimationInstanceComponentData> _animDataLookup;

        [BurstCompile]
        public void Execute(InstanceTag _
            , DynamicBuffer<EntityDamageBuffer> damageBuffer
            , ref HpBarComponentData hpData
            , Entity entity
            )
        {
            for (int i = 0; i < damageBuffer.Length; i++)
            {
                hpData.CurrentHp -= damageBuffer[i].DamageValue;

                if (hpData.CurrentHp <= 0)
                {
                    _searchDataLookup.SetComponentEnabled(entity, false);
                    break;
                }                
            }

            if(damageBuffer.Length >= 1)
            {
                if (_animDataLookup.HasComponent(entity))
                {
                    var animData = _animDataLookup.GetRefRW(entity);
                    animData.ValueRW.flashWhite = 1;
                }
            }

            damageBuffer.Clear();
        }
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency = new EntityDamageSystemJob()
        {
            _searchDataLookup = SystemAPI.GetComponentLookup<EntitySearchTag>(),
            _animDataLookup = SystemAPI.GetComponentLookup<EntityAnimationInstanceComponentData>(),
        }.ScheduleParallel(state.Dependency);
        state.Dependency.Complete();
    }
}