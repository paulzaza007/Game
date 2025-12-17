using UnityEngine;

[CreateAssetMenu(menuName = "Player Actions/Weapon Actions/Light Attack Action")]

public class LightAttackWeaponItemAction : WeaponItemAction
{
    [SerializeField] string light_Attack_01 = "Main_light_Attack_01"; //MainHand

    public override void AttemptToPerformAction(Player playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

        if(playerPerformingAction.playerStatManager.currentStamina <= 0)
        {
            return;
        }
        if (!playerPerformingAction.characterController.isGrounded)
        {
            return;
        }

        PerformLightAttack(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformLightAttack(Player playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        if(playerPerformingAction.isUsingRightHand)
        {
            playerPerformingAction.playerAnimatorManger.PlayerTargetAttackActionAnimation(AttackType.LightAttack01, light_Attack_01, true);
        }
        if (playerPerformingAction.isUsingLeftHand)
        {
            
        }
    }
}
