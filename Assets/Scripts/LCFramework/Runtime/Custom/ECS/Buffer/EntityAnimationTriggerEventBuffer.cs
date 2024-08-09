
using Unity.Entities;

public struct EntityAnimationTriggerEventBuffer : IBufferElementData
{
    public int EventId;
}
public struct EntityAnimationWaitTriggerEventBuffer : IBufferElementData
{
    public int EventId;
    public float NormalizeTriggerTime;
}