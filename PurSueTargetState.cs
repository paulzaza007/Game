using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "A.I/States/Pursues Target")]

public class PurSueTargetState : AIState
{
    public override AIState Tick(AICharacterManager aICharacter)
    {
        //Debug.Log(aICharacter.navMeshAgent.isOnNavMesh);
        if (aICharacter.aICurrentState.isPerformingAction)
        {
            Debug.Log("AI ติด Action อยู่");
            return this;
        }

        if(aICharacter.aICharacterCombatManager.currentTarget == null)
        {
            return SwitchState(aICharacter, aICharacter.idle);
        }

        if (!aICharacter.navMeshAgent.enabled)
        {
            aICharacter.navMeshAgent.enabled = true;
        }

        if(aICharacter.aICharacterCombatManager.viewableAngle < aICharacter.aICharacterCombatManager.minimumFOV || aICharacter.aICharacterCombatManager.viewableAngle > aICharacter.aICharacterCombatManager.maximumFOV)
        {
            aICharacter.aICharacterCombatManager.PivotTowardTarget(aICharacter);
        }

        aICharacter.aIMovementManager.RotateTowardAgent(aICharacter);

        if(aICharacter.aICharacterCombatManager.distanceFromTarget <= aICharacter.navMeshAgent.stoppingDistance)
        {
            return SwitchState(aICharacter, aICharacter.combatStance);
        }
        //aICharacter.navMeshAgent.SetDestination(aICharacter.aICharacterCombatManager.currentTarget.transform.position);

        NavMeshPath path = new NavMeshPath();
        aICharacter.navMeshAgent.CalculatePath(aICharacter.aICharacterCombatManager.currentTarget.transform.position, path);
        aICharacter.navMeshAgent.SetPath(path);

        //aICharacter.navMeshAgent.SetDestination(aICharacter.aICharacterCombatManager.currentTarget.transform.position);

        return this;
    }
}
