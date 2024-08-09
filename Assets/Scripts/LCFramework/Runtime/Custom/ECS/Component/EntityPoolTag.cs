using Unity.Entities;

public struct EntityPoolTag : IComponentData 
{
    public Entity Owner;
}
