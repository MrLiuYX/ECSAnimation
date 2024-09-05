
using cfg;
using JetBrains.Annotations;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(EntityAnimationGroup))]
[UpdateAfter(typeof(EntityAnimationSystem))]
[BurstCompile]
public partial struct EntityAnimationEventSystem : ISystem
{
    [BurstCompile]
    public partial struct EntityAnimationEventTestSystemJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<EntityStateComponentData> _stateDataLookup;
        [NativeDisableParallelForRestriction] public BufferLookup<EntityDamageBuffer> _entityDamageBufferLookup;

        [BurstCompile]
        public void Execute(
            DynamicBuffer<EntityAnimationTriggerEventBuffer> triggerEvent
            , DynamicBuffer<EntityAttributeBuffer> attributeBuffer
            , EntityAnimationRuntimeComponentData animationRuntimeData
            , Entity entity
            )
        {
            for (int i = 0; i < triggerEvent.Length; i++)
            {
                var triggerEventData = triggerEvent[i];

                //1表示是攻击事件 2表示是攻击动画
                if(triggerEventData.EventId == 1 && animationRuntimeData.CurrentConfig.AnimationId == 2)
                {
                    //攻击
                    if (!_stateDataLookup.TryGetComponent(entity, out var stateData)) continue;

                    if (stateData.SearchData.entity == Entity.Null) continue;

                    if (!_entityDamageBufferLookup.TryGetBuffer(stateData.SearchData.entity, out var targetDamageBuffer)) continue;

                    var attack = 0f;

                    //可以做范围攻击类型啥的
                    var attackType = EntityAttackType.Type1;

                    for (int j = 0; j < attributeBuffer.Length; j++)
                    {
                        var attributeBufferData = attributeBuffer[j];
                        if (attributeBufferData.AttributeType == AttributeType.Attack) attack = attributeBufferData.Value;
                        if (attributeBufferData.AttributeType == AttributeType.AttackType) attackType = (EntityAttackType)(int)attributeBufferData.Value;
                    }

                    targetDamageBuffer.Add(new EntityDamageBuffer()
                    {
                        DamageSrouce = entity,
                        DamageValue = attack,
                    });
                }
            }
        }
    }

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency = new EntityAnimationEventTestSystemJob
        {
            _stateDataLookup = SystemAPI.GetComponentLookup<EntityStateComponentData>(),
            _entityDamageBufferLookup = SystemAPI.GetBufferLookup<EntityDamageBuffer>(),
        }.ScheduleParallel(state.Dependency);
        state.Dependency.Complete();
    }
}
