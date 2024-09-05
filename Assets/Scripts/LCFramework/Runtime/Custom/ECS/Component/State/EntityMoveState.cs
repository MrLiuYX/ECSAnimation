
using cfg;
using Unity.Burst;

[BurstCompile]
public struct EntityMoveState : IEntityState<EntityNormalStateConditionProvider>
{
    [BurstCompile]
    public void EntityStateUpdate(EntityNormalStateConditionProvider condition, ref EntityState state)
    {
        state = EntityState.Move;

        if (condition.CheckGoDie(ref state)) return;

        if (condition.CheckGoAttack(ref state)) return;

        if (condition.CheckGoIdle(ref state)) return;
    }
}