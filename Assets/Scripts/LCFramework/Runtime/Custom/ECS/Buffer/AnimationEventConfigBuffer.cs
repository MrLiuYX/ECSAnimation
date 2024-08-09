
using System;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// 动画事件数据
/// </summary>
[Serializable]
public struct AnimationEventConfigBuffer : IBufferElementData
{
    /// <summary>
    /// 动画事件触发id
    /// </summary>
    [SerializeField]
    public int AnimationId;
    /// <summary>
    /// 动画事件id
    /// </summary>
    [SerializeField]
    public int EventId;
    /// <summary>
    /// 动画事件触发事件
    /// </summary>
    [SerializeField]
    public float NormalizeTriggerTime;
}