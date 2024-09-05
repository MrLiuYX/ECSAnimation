using cfg;
using Unity.Burst;

[BurstCompile]
public struct EntityIdleState : IEntityState<EntityNormalStateConditionProvider>
{
    [BurstCompile]
    public void EntityStateUpdate(EntityNormalStateConditionProvider condition, ref EntityState state)
    {
        state = EntityState.Idel;

        if (condition.CheckGoDie(ref state)) return;

        if (condition.CheckGoAttack(ref state)) return;

        if (condition.CheckGoMove(ref state)) return;
    }
}