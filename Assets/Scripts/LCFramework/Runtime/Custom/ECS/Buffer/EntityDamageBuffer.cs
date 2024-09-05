
using cfg;
using Unity.Entities;

public struct EntityDamageBuffer : IBufferElementData
{
    public Entity DamageSrouce;
    public float DamageValue;
}