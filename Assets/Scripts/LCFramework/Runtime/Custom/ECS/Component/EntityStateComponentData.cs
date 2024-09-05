using cfg;
using Unity.Entities;
using static EntitySearchHelper;

public partial struct EntityStateComponentData : IComponentData
{
    public EntityState CurrentEntityState;

    //拥有的状态
    public long HasState;

    //目标实体
    public SearchData SearchData;
}

/// <summary>
/// Attack
/// </summary>
public partial struct EntityStateComponentData
{
    public float AttackSpeed;
    public float AttackIntervalTime;
    public float CurrentAttackTime;
    public float AttackRadius;
}

public partial struct EntityStateComponentData
{
    public float CurrentDeadTime;
}