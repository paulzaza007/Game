using UnityEngine;

[CreateAssetMenu(menuName = "A.I/States/Cyclop/Cyclop Idle")]

public class AICyclopIdleState : IdleState
{
    public override AIState Tick(AICharacterManager aICharacter)
    {
        if (aICharacter.aICharacterCombatManager.currentTarget != null) //เจอผู้เล่น ให้เข้าstateเข้าหาผู้เล่น
        {
            
            aICharacter.aICurrentState.IsTargeting = true;
            //aICharacter.characterSFXManager.StopAllAISounds();
            //aICharacter.characterSFXManager.PlaySFX(WorldSFXManager.instance.UndeadIdleSFX, 0.1f); // ****
            return SwitchState(aICharacter, aICharacter.purSueTarget);

        }
        else //ไม่เจอ ให้เรียกmedthod หาผู้เล่น
        {
            aICharacter.aICurrentState.IsTargeting = false;
            aICharacter.aICharacterCombatManager.FindTargetViaLineOfSight(aICharacter);
            //Debug.Log("ยังไม่เจอผู้เล่น IDLE");

            return this;
        }
    }
}
