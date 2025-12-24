using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Actions/Weapon Actions/Light Attack Action")]

public class LightAttackWeaponItemAction : WeaponItemAction
{
    [SerializeField] string light_Attack_01 = "Main_light_Attack_01"; //MainHand
    [SerializeField] string light_Attack_02 = "Main_light_Attack_02";
    [SerializeField] string light_Attack_03 = "Main_light_Attack_03";


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
        var lastAttack = playerPerformingAction.playerCombatManager.lastAttackAnimationPerform;
        var lastAttackType = playerPerformingAction.playerCombatManager.currentAttackType;
        var player = playerPerformingAction.playerAnimatorManager;

        if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.playerCurrentState.isPerformingAction)
        {
            playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

            if(lastAttack == light_Attack_01 && lastAttackType == AttackType.LightAttack01)
            {
                Debug.Log("1. ชื่อไฟล์ Action ที่ใช้: " + this.name); // จะโชว์ชื่อไฟล์ ScriptableObject
                Debug.Log("2. ชื่อ String ที่จะส่งไปสั่ง Animator: " + light_Attack_02);
                player.PlayerTargetAttackActionAnimation(playerPerformingAction,AttackType.LightAttack02, light_Attack_02, true);
            }

            else if(lastAttack == light_Attack_02 && lastAttackType == AttackType.LightAttack02)
            {
                player.PlayerTargetAttackActionAnimation(playerPerformingAction,AttackType.LightAttack03, light_Attack_03, true);
            }

            else
            {
                player.PlayerTargetAttackActionAnimation(playerPerformingAction,AttackType.LightAttack01, light_Attack_01, true);
            }
        }
        else if(!playerPerformingAction.playerCurrentState.isPerformingAction && playerPerformingAction.playerCombatManager.readyToPerformAttack)
        {
            player.PlayerTargetAttackActionAnimation(playerPerformingAction,AttackType.LightAttack01, light_Attack_01, true);

        }

    }


}
