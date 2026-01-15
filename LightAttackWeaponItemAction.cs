using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Actions/Weapon Actions/Light Attack Action")]

public class LightAttackWeaponItemAction : WeaponItemAction
{
    [Header("Light Attack")]
    [SerializeField] string light_Attack_01 = "Main_light_Attack_01"; //MainHand
    [SerializeField] string light_Attack_02 = "Main_light_Attack_02";
    [SerializeField] string light_Attack_03 = "Main_light_Attack_03";

    [Header("Running Attack")]
    [SerializeField] string run_Attack_01 = "Main_Run_Attack_01";

    [Header("RollingAttack")]
    [SerializeField] string roll_Attack_01 = "Main_Roll_Attack_01";


    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
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
        if (playerPerformingAction.playerMovement.isRunning && !playerPerformingAction.playerCurrentState.isPerformingAction)
        {
            PerformRunningLightAttack(playerPerformingAction, weaponPerformingAction);
            return;
        }
        if (playerPerformingAction.playerCombatManager.canPerformRollingAttack)
        {
            PerformRollingLightAttack(playerPerformingAction, weaponPerformingAction);
            return;
        }

        PerformLightAttack(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction) //ตีปกติ
    {
        var lastAttack = playerPerformingAction.playerCombatManager.lastAttackAnimationPerform;
        var lastAttackType = playerPerformingAction.playerCombatManager.currentAttackType;
        var player = playerPerformingAction.playerAnimatorManager;

        if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.playerCurrentState.isPerformingAction)
        {
            playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

            if(lastAttack == light_Attack_01 && lastAttackType == AttackType.LightAttack01 || lastAttack == roll_Attack_01 || lastAttack == run_Attack_01)
            {
                player.PlayerTargetAttackActionAnimation(playerPerformingAction,AttackType.LightAttack02, light_Attack_02, true);
            }

            else if(lastAttack == light_Attack_02 && lastAttackType == AttackType.LightAttack02)
            {
                player.PlayerTargetAttackActionAnimation(playerPerformingAction,AttackType.LightAttack03, light_Attack_03, true);
            }

            
        }
        else if(!playerPerformingAction.playerCurrentState.isPerformingAction && playerPerformingAction.playerCombatManager.readyToPerformAttack)
        {
            player.PlayerTargetAttackActionAnimation(playerPerformingAction,AttackType.LightAttack01, light_Attack_01, true);

        }

    }

    private void PerformRunningLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction) //วิ่งตี
    {
        playerPerformingAction.playerAnimatorManager.PlayerTargetAttackActionAnimation(playerPerformingAction, AttackType.RunningAttack, run_Attack_01, true);

    }

    private void PerformRollingLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction) //กลิ้งตี
    {
        playerPerformingAction.playerCombatManager.canPerformRollingAttack = false;
        playerPerformingAction.playerAnimatorManager.PlayerTargetAttackActionAnimation(playerPerformingAction, AttackType.RollingAttack, roll_Attack_01, true);
      
    }




}
