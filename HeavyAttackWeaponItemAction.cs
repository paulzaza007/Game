using UnityEngine;

[CreateAssetMenu(menuName = "Player Actions/Weapon Actions/Heavy Attack Action")]

public class HeavyAttackWeaponItemAction : WeaponItemAction
{
    [SerializeField] string heavy_Attack_01 = "Main_Heavy_Attack_01_Charge"; //MainHand

    public override void AttemptToPerformAction(Player playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

        if (playerPerformingAction.playerStatManager.currentStamina <= 0)
        {
            return;
        }
        if (!playerPerformingAction.characterController.isGrounded)
        {
            return;
        }

        PerformHeavyAttack(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformHeavyAttack(Player playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        if (playerPerformingAction.isUsingRightHand)
        {
            playerPerformingAction.playerAnimatorManager.PlayerTargetAttackActionAnimation(playerPerformingAction,AttackType.HeavyAttack01, heavy_Attack_01, true);
        }
        if (playerPerformingAction.isUsingLeftHand)
        {

        }
    }
}
