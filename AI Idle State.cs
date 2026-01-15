using UnityEngine;

[CreateAssetMenu(menuName = "A.I/States/Idle")]

public class IdleState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {

        if(aiCharacter.aICharacterCombatManager.currentTarget != null) //เจอผู้เล่น ให้เข้าstateเข้าหาผู้เล่น
        {
            return SwitchState(aiCharacter, aiCharacter.purSueTarget);

        }
        else //ไม่เจอ ให้เรียกmedthod หาผู้เล่น
        {
            //Debug.Log("WE HAVE NO TARGET");
            aiCharacter.aICharacterCombatManager.FindTargetViaLineOfSight(aiCharacter);

            return this;
        }

    }


}
