
using Unity.Entities;
using System;
using UnityEngine;

[Serializable]
public struct AnimationConfigComponentData : IComponentData
{
    /// <summary>
    /// 贴图宽度
    /// </summary>
    [SerializeField]
    public int Width;
    /// <summary>
    /// 贴图长度
    /// </summary>
    [SerializeField]
    public int Height;
}