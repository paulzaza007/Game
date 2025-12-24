using UnityEngine;

[CreateAssetMenu(menuName = "A.I/States/Idle")]

public class IdleState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {

        if(aiCharacter.playerCombatManager.currentTarget != null)
        {
            return SwitchState(aiCharacter, aiCharacter.purSueTarget);

        }
        else
        {
            //Debug.Log("WE HAVE NO TARGET");
            aiCharacter.aICharacterCombatManager.FindTargetViaLineOfSight(aiCharacter);

            return this;
        }

    }


}
