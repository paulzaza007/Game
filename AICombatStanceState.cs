using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "A.I/States/Combat Stance")]

public class AICombatStanceState : AIState
{
    [Header("Attacks")]
    public List<AIAttackAction> aIAttack;
    private List<AIAttackAction> potentialAttacks;
    private AIAttackAction chooseAttack;
    private AIAttackAction previosAttack;
    protected bool hasAttack = false;

    [Header("Combo")]
    [SerializeField] protected bool canPerformCombo = false;
    [SerializeField] protected int chanceToPerformCombo = 25;
    protected bool hasRollForComboChance = false;

    [Header("Engagement Distance")]
    [SerializeField] public  float maximumEngagementDistance = 5;

    public override AIState Tick(AICharacterManager aICharacter)
    {
        if (aICharacter.aICurrentState.isPerformingAction)
        {
            return this;
        }
        
        if (!aICharacter.navMeshAgent.enabled)
        {
            aICharacter.navMeshAgent.enabled = true;
        }

        if (!aICharacter.IsMoving)
        {
            if(aICharacter.aICharacterCombatManager.viewableAngle < -30 || aICharacter.aICharacterCombatManager.viewableAngle > 30)
            {
                aICharacter.aICharacterCombatManager.PivotTowardTarget(aICharacter);
            }

            
        }

        aICharacter.aICharacterCombatManager.RotateTowardsAgent(aICharacter);

        if (aICharacter.aICharacterCombatManager.currentTarget == null)
        {
            return SwitchState(aICharacter, aICharacter.idle);
        }

        if (!hasAttack)
        {
            GetNewAttack(aICharacter);
        }
        else
        {
            aICharacter.attack.currentAttack = chooseAttack;

            return SwitchState(aICharacter, aICharacter.attack);
        }

        if(aICharacter.aICharacterCombatManager.distanceFromTarget > maximumEngagementDistance)
        {
            return SwitchState(aICharacter, aICharacter.purSueTarget);
        }

        NavMeshPath path = new NavMeshPath();
        aICharacter.navMeshAgent.CalculatePath(aICharacter.aICharacterCombatManager.currentTarget.transform.position, path);
        aICharacter.navMeshAgent.SetPath(path);

        return this;
    }

    protected virtual void GetNewAttack(AICharacterManager aICharacter)
    {
        potentialAttacks = new List<AIAttackAction>();

        foreach (var potentialAttack in aIAttack)
        {
            if(potentialAttack.minimumDistance > aICharacter.aICharacterCombatManager.distanceFromTarget)
            {
                continue;
            }
            if (potentialAttack.maximumDistance < aICharacter.aICharacterCombatManager.distanceFromTarget)
            {
                continue;
            }
            if(potentialAttack.minimumAttackAngle > aICharacter.aICharacterCombatManager.viewableAngle)
            {
                continue;
            }
            if (potentialAttack.maximumAttackAngle < aICharacter.aICharacterCombatManager.viewableAngle)
            {
                continue;
            }

            potentialAttacks.Add(potentialAttack);
        }

        if(potentialAttacks.Count <= 0)
        {
            return;
        } 

        var totalWeight = 0;

        foreach (var attack in potentialAttacks)
        {
            totalWeight += attack.AttackWeight;
        }

        var randomWeightValue = Random.Range(1, totalWeight + 1);
        var processWeight = 0;

        foreach (var attack in potentialAttacks)
        {
            processWeight += attack.AttackWeight;

            if(randomWeightValue <= processWeight)
            {
                chooseAttack = attack;
                previosAttack = chooseAttack;
                hasAttack = true;
                return;
            }
        }
    }

    protected virtual bool RollForOutComeChance(int chance)
    {
        bool outcomeWillBePerform = false;

        int randomPercentage = Random.Range(0, 100);

        if(randomPercentage < chance)
        {
            outcomeWillBePerform = true;
        }

        return outcomeWillBePerform;
    }

    protected override void ResetStateFlags(AICharacterManager aICharacter)
    {
        base.ResetStateFlags(aICharacter);

        hasAttack = false;
        hasRollForComboChance = false;
    }
}