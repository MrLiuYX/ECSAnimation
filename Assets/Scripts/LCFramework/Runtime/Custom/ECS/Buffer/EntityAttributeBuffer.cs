
using Unity.Entities;

public struct EntityAttributeBuffer: IBufferElementData
{
    public cfg.AttributeType AttributeType;
    public float Value;
}