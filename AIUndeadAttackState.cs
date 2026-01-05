using UnityEngine;

[CreateAssetMenu(menuName = "A.I/States/Undead/UndeadAttack")]

public class AIUndeadAttackState : AIAttackState
{

    public override AIState Tick(AICharacterManager aICharacter)
    {
        if (aICharacter.aICharacterCombatManager.currentTarget == null) //ไม่เจอผู้เล่น กลับไปState idle
        {
            return SwitchState(aICharacter, aICharacter.idle);
        }

        if (aICharacter.aICharacterCombatManager.currentTarget.playerCurrentState.isDead) //ผู้เล่นตาย กลับไปState idle
        {
            return SwitchState(aICharacter, aICharacter.idle);
        }

        aICharacter.aICharacterCombatManager.RotateTowardsTargetWhileAttacking(aICharacter); //หันตามผู้เล่นขณะโจมตี

        if (willPerformCombo && !hasPerformAttack) // เอาไว้คอมโบ **ตอนนี้ยังไม่มี**
        {
            if (currentUndeadAttack.comboAction != null) //****
            {
                hasPerformCombo = true;
                currentUndeadAttack.comboAction.AttemptToPerformAction(aICharacter); //****
            }
        }

        if (aICharacter.aICurrentState.isPerformingAction) //ถ้า AI เล่นอนิเมชั่นอยู่ ให้วนmedthod นี้
        {
            return this;
        }

        if (!hasPerformAttack) //ยังไม่ได้โจมตี
        {
            if (aICharacter.aICharacterCombatManager.actionRecoveryTimer > 0) // ถ้าคูลดาวหลังโจมตียังไม่เหลือ 0 ให้วนmedthod
            {
                //Debug.Log("ยังคูลดาวอยู่");
                return this;
            }

            if (aICharacter.IsMoving) // ถ้า AI ขยับ ให้กลับไป Statecombat
            {
                return SwitchState(aICharacter, aICharacter.combatStance);
            }

            PerformAttack(aICharacter); //โจมตี

            return this; //วน
        }

        if (pivotAfterAttack) //หันหาผู้เล่นหลังโจมตี **ยังไม่มี**
        {
            aICharacter.aICharacterCombatManager.PivotTowardTarget(aICharacter);
        }
        Debug.Log("AttackState ทำงานเสร็จแล้ว");
        return SwitchState(aICharacter, aICharacter.combatStance);
    }

    protected override void PerformAttack(AICharacterManager aICharacter)
    {
        hasPerformAttack = true;
        currentUndeadAttack.AttemptToPerformAction(aICharacter); //เล่นanimation ของUndead //****

        aICharacter.aICharacterCombatManager.actionRecoveryTimer = currentUndeadAttack.actionRecoveryTime; //ตั้งคูลดาวตามท่าที่ใช้โจมตี ของUndead //****
    }

    protected override void ResetStateFlags(AICharacterManager aICharacter)
    {
        base.ResetStateFlags(aICharacter);

        hasPerformAttack = false;
        hasPerformCombo = false;
    }
}
