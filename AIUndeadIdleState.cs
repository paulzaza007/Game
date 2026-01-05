using UnityEngine;

[CreateAssetMenu(menuName = "A.I/States/Undead/Undead Idle")]

public class AIUndeadIdleState : IdleState
{
    public override AIState Tick(AICharacterManager aICharacter)
    {
        if (aICharacter.playerCombatManager.currentTarget != null) //เจอผู้เล่น ให้เข้าstateเข้าหาผู้เล่น
        {
            /*if(aICharacter.aICharacterCombatManager.distanceFromTarget > 10)
            {
                //เล่น attck03
            }
            else if(aICharacter.aICharacterCombatManager.distanceFromTarget > 3 && aICharacter.aICharacterCombatManager.distanceFromTarget < 10)
            {
                //เล่น attack02
                aICharacter.attack.currentUndeadAttack = attack02;
                aICharacter.attack.currentUndeadAttack.AttemptToPerformAction(aICharacter);
            }*/
            aICharacter.aICurrentState.IsTargeting = true;
            aICharacter.characterSFXManager.StopAllAISounds();
            aICharacter.characterSFXManager.PlaySFX(WorldSFXManager.instance.UndeadIdleSFX, 0.1f);
            return SwitchState(aICharacter, aICharacter.purSueTarget);

        }
        else //ไม่เจอ ให้เรียกmedthod หาผู้เล่น
        {
            //Debug.Log("WE HAVE NO TARGET");
            aICharacter.aICurrentState.IsTargeting = false;
            aICharacter.aICharacterCombatManager.FindTargetViaLineOfSight(aICharacter);

            return this;
        }
    }

}
