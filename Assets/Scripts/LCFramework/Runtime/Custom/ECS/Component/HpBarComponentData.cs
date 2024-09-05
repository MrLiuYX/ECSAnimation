
using cfg;
using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct HpBarComponentData : IComponentData, IComparable<HpBarComponentData>
{
    public float3 color;
    public float heightOffset; //血条高度偏移

    public float CurrentHp;
    public float MaxHp;
    public float width;
    public float height;

    public float3 pos;
    public float EntityIndex;

    [BurstCompile]
    public static HpBarComponentData Create(float maxHp, float currentHp, float3 pos, int entityIndex, EntityTeam team, float sizeRatio)
    {
        var color = float3.zero;
        if(team == EntityTeam.Team1)
        {
            color = new float3(0, 1, 0);
        }
        else if (team == EntityTeam.Team2)
        {
            color = new float3(0.8f, 0.8f, 0.2f);
        }
        else if(team == EntityTeam.Team3)
        {
            color = new float3(1, 0, 0);
        }
        else if (team == EntityTeam.Team4)
        {
            color = new float3(0, 0, 1);
        }

        return new HpBarComponentData()
        {
            color = color,
            heightOffset = 2,
            MaxHp = maxHp,
            CurrentHp = currentHp,
            width = 0.9f * sizeRatio,
            height = 0.8f * sizeRatio,
            pos = pos,
            EntityIndex = entityIndex,
        };
    }

    [BurstCompile]
    public int CompareTo(HpBarComponentData other)
    {
        return (int)EntityIndex - (int)other.EntityIndex;
    }
}