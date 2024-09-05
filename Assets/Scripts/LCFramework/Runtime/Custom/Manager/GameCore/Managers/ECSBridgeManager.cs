
using Native.Component;
using Native.Event;
using System;
using Unity.Entities;

public class ECSBridgeManager : GameManagerBase<ECSBridgeManager, NullData>
{
    public static bool ECSInitDone = false;

    public EntityPoolGroup EntityPoolGroup;
    public EntityAnimationGroup EntityAnimationGroup;
    public EntityLogicGroup EntityLogicGroup;
    public PlayerInputSystem PlayerInputSystem;
    public EntityManagerSystem EntityManagerSystem;

    public override void InitManager()
    {
        base.InitManager();
    }

    public void GetSystem()
    {
        EntityPoolGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<EntityPoolGroup>();
        EntityAnimationGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<EntityAnimationGroup>();
        EntityLogicGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<EntityLogicGroup>();
        PlayerInputSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PlayerInputSystem>();
        EntityManagerSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<EntityManagerSystem>();

        EntityManagerSystem.Enabled = true;
    }

    public override void OnEnterGame()
    {
        base.OnEnterGame();
        EntityPoolGroup.Enabled = true;
        EntityAnimationGroup.Enabled = true;
        EntityLogicGroup.Enabled = true;
        PlayerInputSystem.Enabled = true;
    }

    public override void OnExitGame()
    {
        base.OnExitGame();
        EntityPoolGroup.Enabled = false;
        EntityAnimationGroup.Enabled = false;
        EntityLogicGroup.Enabled = false;
        PlayerInputSystem.Enabled = false;
    }

    public void ClearAllEntity()
    {
        EntityManagerSystem.ClearAllEntity();
    }
}