using cfg;

public interface IEntityState<T> where T : struct, IEntityStateConditionProvider
{
    public void EntityStateUpdate(T condition, ref EntityState state);
}