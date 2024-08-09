
using Unity.Entities;
using UnityEngine;

public struct AnimationRuntimeComponentData : IComponentData
{
    //动态手动赋值
    public int NextAnimationId;
    public float Speed;
    public bool Stop;

    //动态自动赋值（别瞎动哈）
    public int MeshAndMatIndex;
    public int CurrentAnimationId;
    public float NormalizeTime;
    public float Time;
    public AnimationConfigBuffer CurrentConfig;
    public int CurrentRepeatCount;
}
