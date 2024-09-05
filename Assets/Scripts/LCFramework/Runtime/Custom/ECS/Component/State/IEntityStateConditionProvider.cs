
using cfg;

public interface IEntityStateConditionProvider
{
    public bool CheckGoIdle(ref EntityState state);
    public bool CheckGoMove(ref EntityState state);
    public bool CheckGoAttack(ref EntityState state);
    public bool CheckGoDie(ref EntityState state);
}