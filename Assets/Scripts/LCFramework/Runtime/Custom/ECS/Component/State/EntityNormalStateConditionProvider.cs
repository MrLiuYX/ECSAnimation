
using cfg;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public struct EntityNormalStateConditionProvider : IEntityStateConditionProvider
{
    public Entity Self;
    public LocalTransform TsData;
    public EntityStateComponentData StateData;
    public EntityTeam Team;
    public bool Dead;

    public EntityNormalStateConditionProvider(LocalTransform tsData, EntityStateComponentData stateData, Entity self, cfg.EntityTeam team, bool dead)
    {
        TsData = tsData;
        StateData = stateData;
        Self = self;
        Team = team;
        Dead = dead;
    }
    
    [BurstCompile]
    public bool CheckGoAttack(ref EntityState state)
    {
        if (!HasEnemy()) return false;
        if (!EnemyInAttackRange()) return false;
        if (StateData.CurrentAttackTime < StateData.AttackIntervalTime) return false;

        state = EntityState.Attack;
        return true;
    }
    
    [BurstCompile]
    public bool CheckGoDie(ref EntityState state)
    {
        if (Dead) state = EntityState.Die;
        return Dead;
    }
    
    [BurstCompile]
    public bool CheckGoIdle(ref EntityState state)
    {
        if(HasEnemy())
        {
            if (!EnemyInAttackRange()) return false;
        }
        else
        {
            if(!ArriveTarget())return false;
        }

        state = EntityState.Idel;
        return true;
    }
    
    [BurstCompile]
    public bool CheckGoMove(ref EntityState state)
    {
        if (HasEnemy())
        {
            if (EnemyInAttackRange()) return false;
        }
        else
        {
            if (ArriveTarget()) return false;
        }

        state = EntityState.Move;
        return true;
    }
    
    [BurstCompile]
    private bool HasEnemy()
    {
        if (StateData.SearchData.entity == Entity.Null) return false;
        if (StateData.SearchData.entity == Self) return false;
        if (StateData.SearchData.Team == Team && Team != EntityTeam.Single) return false;
        return true;
    }

    [BurstCompile]
    private bool EnemyInAttackRange()
    {
        return math.distance(TsData.Position, StateData.SearchData.tsData.Position) <= StateData.AttackRadius;
    }

    [BurstCompile]
    private bool ArriveTarget()
    {
        return math.distance(TsData.Position, StateData.SearchData.tsData.Position) <= 0.1f;
    }
}
