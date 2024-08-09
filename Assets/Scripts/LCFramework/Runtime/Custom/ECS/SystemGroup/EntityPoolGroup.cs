using Unity.Transforms;
using Unity.Entities;

[UpdateBefore(typeof(TransformSystemGroup))]
public partial class EntityPoolGroup : ComponentSystemGroup
{
    protected override void OnCreate()
    {
        base.OnCreate();
        Enabled = false;
    }
}
