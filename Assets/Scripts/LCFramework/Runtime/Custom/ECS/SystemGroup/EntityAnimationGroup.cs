using Unity.Entities;
using Unity.Transforms;


[UpdateAfter(typeof(TransformSystemGroup))]
[UpdateBefore(typeof(LateSimulationSystemGroup))]
public partial class EntityAnimationGroup : ComponentSystemGroup
{
    protected override void OnCreate()
    {
        base.OnCreate();
        Enabled = false;
    }
}
