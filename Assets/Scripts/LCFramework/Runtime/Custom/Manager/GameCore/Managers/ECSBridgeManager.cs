
using Native.Component;
using Native.Event;
using System;
using Unity.Entities;

public class ECSBridgeManager : GameManagerBase<ECSBridgeManager, NullData>
{
    public static bool ECSInitDone = false;

    public EntityPoolGroup EntityPoolGroup;
    public EntityAnimationGroup EntityAnimationGroup;
    public PlayerInputSystem PlayerInputSystem;

    public override void InitManager()
    {
        base.InitManager();
    }

    public void GetSystem()
    {
        EntityPoolGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<EntityPoolGroup>();
        EntityAnimationGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<EntityAnimationGroup>();
        PlayerInputSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PlayerInputSystem>();
    }

    public override void OnEnterGame()
    {
        base.OnEnterGame();
        EntityPoolGroup.Enabled = true;
        EntityAnimationGroup.Enabled = true;
        PlayerInputSystem.Enabled = true;
    }

    public override void OnExitGame()
    {
        base.OnExitGame();
        EntityPoolGroup.Enabled = false;
        EntityAnimationGroup.Enabled = false;
        PlayerInputSystem.Enabled = false;
    }
}