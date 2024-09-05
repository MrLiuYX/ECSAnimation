
using System;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// 动画数据
/// </summary>
[Serializable]
public struct EntityAnimationConfigBuffer : IBufferElementData, IEquatable<EntityAnimationConfigBuffer>
{
    /// <summary>
    /// 动画id
    /// </summary>
    [SerializeField]
    public int AnimationId;
    /// <summary>
    /// 动画loop状态
    /// </summary>
    [SerializeField]
    public bool AnimationLoop;
    /// <summary>
    /// 动画纹理开始行
    /// </summary>
    [SerializeField]
    public int StartLine;
    /// <summary>
    /// 动画纹理结束行
    /// </summary>
    [SerializeField]
    public int EndLine;
    /// <summary>
    /// 动画耗时
    /// </summary>
    [SerializeField]
    public float TotalSec;

    public static bool operator ==(EntityAnimationConfigBuffer lhs, EntityAnimationConfigBuffer rhs)
    {
        return lhs.AnimationId == rhs.AnimationId 
            && lhs.AnimationLoop == rhs.AnimationLoop
            && lhs.StartLine == rhs.StartLine
            && lhs.EndLine == rhs.EndLine
            && lhs.TotalSec == rhs.TotalSec;
    }

    public static bool operator !=(EntityAnimationConfigBuffer lhs, EntityAnimationConfigBuffer rhs)
    {
        return !(lhs == rhs);
    }

    public override bool Equals(object compare)
    {
        return compare is Entity compareEntity && Equals(compareEntity);
    }

    public override int GetHashCode()
    {
        return AnimationId;
    }

    public bool Equals(EntityAnimationConfigBuffer rhs)
    {
        return AnimationId == rhs.AnimationId
            && AnimationLoop == rhs.AnimationLoop
            && StartLine == rhs.StartLine
            && EndLine == rhs.EndLine
            && TotalSec == rhs.TotalSec;
    }


    public static EntityAnimationConfigBuffer Null => new EntityAnimationConfigBuffer();
}