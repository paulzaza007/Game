using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "A.I/States/Undead/Undead Pursues Target")]

public class UndeadPurSueTargetState : PurSueTargetState
{
    public override AIState Tick(AICharacterManager aICharacter)
    {
        if (aICharacter.aICurrentState.isPerformingAction) //AI ติดอนิเมชั่นอยู่ วนmethodนี้
        {
            return this;
        }

        if (aICharacter.aICharacterCombatManager.currentTarget == null) //ไม่เจอผู้เล่น กลับไป State idle
        {
            aICharacter.characterSFXManager.StopAllAISounds();
            aICharacter.characterSFXManager.PlaySFX(WorldSFXManager.instance.UndeadIdleSFX, 0.1f);
            return SwitchState(aICharacter, aICharacter.idle);
        }

        if (!aICharacter.navMeshAgent.enabled) //ไม่มีnavmesh กำหนด เปิดnavmeshให้
        {
            aICharacter.navMeshAgent.enabled = true;
        }

        if (aICharacter.aICharacterCombatManager.viewableAngle < aICharacter.aICharacterCombatManager.minimumFOV || aICharacter.aICharacterCombatManager.viewableAngle > aICharacter.aICharacterCombatManager.maximumFOV)
        {
            aICharacter.aICharacterCombatManager.PivotTowardTarget(aICharacter); //ซ้ายเกินหรือขวาเกิน ให้AIหันหาผู้เล่น แบบอนิเมชั่น
        }

        aICharacter.aIMovementManager.RotateTowardAgent(aICharacter); //หันตามผู้เล่น

        if (aICharacter.aICharacterCombatManager.distanceFromTarget <= aICharacter.navMeshAgent.stoppingDistance) //ถ้าระยะทางระหว่างผู้เล่นกับAIน้อยกว่าระยะหยุดของAIให้เปลี่ยนเป็นStateCombat
        {
            return SwitchState(aICharacter, aICharacter.combatStance);
        }
        //aICharacter.navMeshAgent.SetDestination(aICharacter.aICharacterCombatManager.currentTarget.transform.position);

        // คำนวนเส้นทางAI ไป ผู้เล่น
        NavMeshPath path = new NavMeshPath();
        aICharacter.navMeshAgent.CalculatePath(aICharacter.aICharacterCombatManager.currentTarget.transform.position, path);
        aICharacter.navMeshAgent.SetPath(path);

        //aICharacter.navMeshAgent.SetDestination(aICharacter.aICharacterCombatManager.currentTarget.transform.position);

        return this; //วน
    }
}
