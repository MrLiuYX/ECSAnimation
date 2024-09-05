
using Unity.Entities;
using Unity.Transforms;

[UpdateAfter(typeof(TransformSystemGroup))]
[UpdateBefore(typeof(EntityAnimationGroup))]
public partial class EntityLogicGroup : ComponentSystemGroup
{
    protected override void OnCreate()
    {
        base.OnCreate();
        Enabled = false;
    }
}