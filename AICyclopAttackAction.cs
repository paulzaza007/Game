using UnityEngine;

[CreateAssetMenu(menuName = "A.I/Actions/Cyclop/CyclopAttack")]

public class AICyclopAttackAction : AIAttackAction
{
    public override void AttemptToPerformAction(AICharacterManager aICharacter)
    {
        if (attackAnimation == "Undead_Attack01")
        {
            aICharacter.aIAnimationManager.PlayerTargetAttackActionAnimation(attackType, attackAnimation, true);
            aICharacter.characterSFXManager.StopAllAISounds();
            aICharacter.characterSFXManager.PlaySFX(WorldSFXManager.instance.ChooseRandomSFXFromArray(WorldSFXManager.instance.UndeadAttackAlertSFX), 0.1f);
        }
        else if (attackAnimation == "Undead_Attack02")
        {
            aICharacter.aIAnimationManager.PlayerTargetAttackActionAnimation(attackType, attackAnimation, true);
            aICharacter.characterSFXManager.StopAllAISounds();
            aICharacter.characterSFXManager.PlaySFX(WorldSFXManager.instance.ChooseRandomSFXFromArray(WorldSFXManager.instance.UndeadAttackAlertSFX), 0.1f);

        }
        else if (attackAnimation == "Undead_Scream")
        {
            aICharacter.aIAnimationManager.PlayerTargetActionAnimation(attackAnimation, true);
            aICharacter.characterSFXManager.StopAllAISounds();
            aICharacter.characterSFXManager.PlaySFX(WorldSFXManager.instance.ChooseRandomSFXFromArray(WorldSFXManager.instance.UndeadAttackAlertSFX), 0.7f, true, 0.1f, 0.9f);
        }
        else
        {
            aICharacter.aIAnimationManager.PlayerTargetAttackActionAnimation(attackType, attackAnimation, true);
        }
    }
}
