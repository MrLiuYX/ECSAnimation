
using cfg;
using Unity.Burst;

[BurstCompile]
public struct EntityAttackState : IEntityState<EntityNormalStateConditionProvider>
{
    [BurstCompile]
    public void EntityStateUpdate(EntityNormalStateConditionProvider condition, ref EntityState state)
    {
        state = EntityState.Attack;

        if (condition.CheckGoDie(ref state)) return;

        if (condition.CheckGoIdle(ref state)) return;

        if (condition.CheckGoMove(ref state)) return;
    }
}
