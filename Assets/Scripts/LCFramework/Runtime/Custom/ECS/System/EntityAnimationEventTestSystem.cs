
using JetBrains.Annotations;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(EntityAnimationGroup))]
[UpdateAfter(typeof(EntityAnimationSystem))]
[BurstCompile]
public partial struct EntityAnimationEventTestSystem : ISystem
{
    [BurstCompile]
    public partial struct EntityAnimationEventTestSystemJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(
            DynamicBuffer<EntityAnimationTriggerEventBuffer> triggerEvent
            , AnimationRuntimeComponentData animationRuntimeData
            )
        {
            for (int i = 0; i < triggerEvent.Length; i++)
            {
                var triggerEventData = triggerEvent[i];
                Debug.LogError($"Trigger Animation {triggerEventData.EventId}");
            }
        }
    }

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.Enabled = false;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency = new EntityAnimationEventTestSystemJob
        {

        }.ScheduleParallel(state.Dependency);
        state.Dependency.Complete();
    }
}
