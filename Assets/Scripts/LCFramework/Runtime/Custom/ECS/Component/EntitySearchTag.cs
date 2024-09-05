
using cfg;
using Unity.Entities;
using static EntitySearchHelper;

public partial struct EntitySearchTag : IComponentData, IEnableableComponent
{
    //自身所属Team
    public EntityTeam Team;

    public bool NeedSearch;
    public float SearchInterval;
}