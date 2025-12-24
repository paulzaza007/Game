using UnityEngine;

[CreateAssetMenu(menuName = "A.I/States/Attack")]

public class AIAttackState : AIState
{
    [Header("Current Attack")]
    [HideInInspector] public AIAttackAction currentAttack;
    [HideInInspector] public bool willPerformCombo = false;

    [Header("Flags")]
    protected bool hasPerformAttack = false;
    protected bool hasPerformCombo = false;

    [Header("Pivot After Attack")]
    [SerializeField] protected bool pivotAfterAttack = false;

    public override AIState Tick(AICharacterManager aICharacter)
    {
        if(aICharacter.aICharacterCombatManager.currentTarget == null)
        {
            return SwitchState(aICharacter, aICharacter.idle);
        }

        if (aICharacter.aICharacterCombatManager.currentTarget.playerCurrentState.isDead)
        {
            return SwitchState(aICharacter, aICharacter.idle);
        }

        aICharacter.aICharacterCombatManager.RotateTowardsTargetWhileAttacking(aICharacter);

        if(willPerformCombo && !hasPerformAttack)
        {
            if(currentAttack.comboAction != null)
            {
                hasPerformCombo = true;
                currentAttack.comboAction.AttemptToPerformAction(aICharacter);
            }
        }

        if (aICharacter.aICurrentState.isPerformingAction)
        {
            return this;
        }

        if (!hasPerformAttack)
        {
            if(aICharacter.aICharacterCombatManager.actionRecoveryTimer > 0)
            {
                return this;
            }

            if (aICharacter.IsMoving)
            {
                return SwitchState(aICharacter, aICharacter.combatStance);
            }

            PerformAttack(aICharacter);

            return this;
        }

        if (pivotAfterAttack)
        {
            aICharacter.aICharacterCombatManager.PivotTowardTarget(aICharacter);
        }

        return SwitchState(aICharacter, aICharacter.combatStance);

    }

    protected void PerformAttack(AICharacterManager aICharacter)
    {
        hasPerformAttack = true;
        currentAttack.AttemptToPerformAction(aICharacter);

        aICharacter.aICharacterCombatManager.actionRecoveryTimer = currentAttack.actionRecoveryTime;
    }

    protected override void ResetStateFlags(AICharacterManager aICharacter)
    {
        base.ResetStateFlags(aICharacter);

        hasPerformAttack = false;
        hasPerformCombo = false;
    }
}
