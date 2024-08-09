
using Unity.Collections;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;
using UnityEngine.UIElements;
using static EntityAnimationSystem;

[UpdateInGroup(typeof(EntityAnimationGroup))]
[BurstCompile]
public partial struct EntityAnimationSystem : ISystem
{
    public struct RendererWriteData
    {
        public int MeshAndMatIndex;
        public AnimationInstanceComponentData InstanceData;
    }

    [BurstCompile]
    public partial struct ECSAnimationSystemJob : IJobEntity
    {
        [ReadOnly]public float DeltaTime;
        public NativeList<RendererWriteData>.ParallelWriter _datas;

        [BurstCompile]
        public void Execute(
                AnimationConfigComponentData config
                , ref AnimationRuntimeComponentData data
                , ref AnimationInstanceComponentData instanceData
                , DynamicBuffer<AnimationConfigBuffer> configBuffer
                , DynamicBuffer<AnimationEventConfigBuffer> eventConfigBuffer
                , DynamicBuffer<EntityAnimationTriggerEventBuffer> eventBuffer
                , DynamicBuffer<EntityAnimationWaitTriggerEventBuffer> waitEventBuffer
                , InstanceTag tag
                , ref LocalTransform tsData, [ChunkIndexInQuery] int sortKey
            )
        {
            if (configBuffer.Length == 0) goto PlayAnim;

            if (data.CurrentAnimationId == data.NextAnimationId || data.Stop) goto PlayAnim;

            //切换
            data.CurrentAnimationId = data.NextAnimationId;
            for (int i = 0; i < configBuffer.Length; i++)
            {
                if (data.CurrentAnimationId == configBuffer[i].AnimationId)
                {
                    data.Time = 0;
                    data.NormalizeTime = 0;
                    data.CurrentRepeatCount = 0;

                    data.CurrentConfig = configBuffer[i];
                    data.CurrentAnimationId = data.CurrentConfig.AnimationId;
                    data.NextAnimationId = data.CurrentConfig.AnimationId;
                    ReigsterWaitEvent(eventConfigBuffer, waitEventBuffer, data);
                    goto PlayAnim;
                }
            }

        //播放动画
        PlayAnim:
            if (data.CurrentConfig == AnimationConfigBuffer.Null)
            {
                if (configBuffer.Length > 0)
                {
                    data.CurrentConfig = configBuffer[0];
                    data.CurrentAnimationId = data.CurrentConfig.AnimationId;
                    data.NextAnimationId = data.CurrentConfig.AnimationId;
                    ReigsterWaitEvent(eventConfigBuffer, waitEventBuffer, data);
                }
            }

            var tempRepeatCount = data.CurrentRepeatCount;
            data.Time += DeltaTime * data.Speed * (data.Stop ? 0 : 1);
            data.CurrentRepeatCount = (int)(data.Time / data.CurrentConfig.TotalSec);
            var trulyValue = data.CurrentConfig.TotalSec == 0
                ? 0
                : data.Time / data.CurrentConfig.TotalSec;
            data.NormalizeTime = data.CurrentConfig.AnimationLoop
                ? math.abs(trulyValue - math.trunc(trulyValue))
                : trulyValue;
            var uvY = math.clamp((data.NormalizeTime * ((data.CurrentConfig.EndLine - data.CurrentConfig.StartLine) * 1.0f / config.Height)) + data.CurrentConfig.StartLine * 1.0f / config.Height, 0, data.CurrentConfig.EndLine * 1.0f / config.Height - 1f / config.Height);

            instanceData.pos = tsData.Position;
            instanceData.uvY = uvY;
            instanceData.rot = ((Quaternion)tsData.Rotation).eulerAngles;
            instanceData.scale = tsData.Scale;
            _datas.AddNoResize(new RendererWriteData()
            {
                MeshAndMatIndex = data.MeshAndMatIndex,
                InstanceData = instanceData,
            });


            //TODO:事件处理
            eventBuffer.Clear();

            var compareNormalizeTime = tempRepeatCount == data.CurrentRepeatCount ? data.NormalizeTime : 1;
            for (int i = 0; i < waitEventBuffer.Length; i++)
            {
                var waitEventData = waitEventBuffer[i];
                if (compareNormalizeTime >= waitEventData.NormalizeTriggerTime)
                {
                    eventBuffer.Add(new EntityAnimationTriggerEventBuffer { EventId = waitEventData.EventId });
                    waitEventBuffer.RemoveAt(i--);
                }
            }

            if (!data.CurrentConfig.AnimationLoop) return;

            var internalRepeatCount = data.CurrentRepeatCount - tempRepeatCount;
            if (internalRepeatCount == 0) return;

            for (int i = 0; i < internalRepeatCount - 1; i++)
            {
                for (int j = 0; j < eventConfigBuffer.Length; j++)
                {
                    var eventConfigBufferData = eventConfigBuffer[j];
                    if(eventConfigBufferData.AnimationId == data.CurrentAnimationId)
                    {
                        eventBuffer.Add(new EntityAnimationTriggerEventBuffer() { EventId = eventConfigBufferData.EventId });
                    }
                }
            }
            
            ReigsterWaitEvent(eventConfigBuffer, waitEventBuffer, data);
        }

        private void ReigsterWaitEvent(
            DynamicBuffer<AnimationEventConfigBuffer> eventConfigBuffer
            , DynamicBuffer<EntityAnimationWaitTriggerEventBuffer> waitEventBuffer
            , AnimationRuntimeComponentData data)
        {
            waitEventBuffer.Clear();
            for (int i = 0; i < eventConfigBuffer.Length; i++)
            {
                var eventConfigBufferData = eventConfigBuffer[i];
                if (eventConfigBufferData.AnimationId == data.CurrentAnimationId)
                {
                    waitEventBuffer.Add(new EntityAnimationWaitTriggerEventBuffer()
                    {
                        EventId = eventConfigBufferData.EventId,
                        NormalizeTriggerTime = eventConfigBufferData.NormalizeTriggerTime
                    });
                }
            }
        }
    }

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.EntityManager.AddComponent<AnimationInstanceDatas>(state.SystemHandle);
        var datas = SystemAPI.GetSingletonRW<AnimationInstanceDatas>();
        datas.ValueRW.Datas = new NativeList<RendererWriteData>(1 << 20, Allocator.Persistent);
    }

    [BurstCompile]
    public void OnDestory(ref SystemState state)
    {
        var datas = SystemAPI.GetSingletonRW<AnimationInstanceDatas>();
        datas.ValueRW.Datas.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var datas = SystemAPI.GetSingletonRW<AnimationInstanceDatas>();
        datas.ValueRW.Datas.Clear();
        state.Dependency = new ECSAnimationSystemJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            _datas = datas.ValueRW.Datas.AsParallelWriter()
        }.ScheduleParallel(state.Dependency);
        state.Dependency.Complete();
    }
}

public partial struct AnimationInstanceDatas : IComponentData
{
    public NativeList<RendererWriteData> Datas;
}