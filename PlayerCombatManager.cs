using UnityEngine;
using System.Collections;

public class PlayerCombatManager : CharacterCombatManager
{
    [Header("Attack Cooldown")]
    [SerializeField] public bool readyToPerformAttack = true;
    [SerializeField] float waitingTime = 2;

    [Header("Attack Flags")]
    public bool canPerformRollingAttack = false;

    public void DrainStaminaBasedOnAttack()
    {
        if (currentWeaponBeingUsed == null)
        {
            return;
        }

        float staminaDeducted = 0;



        switch (currentAttackType)
        {
            case AttackType.LightAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightattackStaminaCostMultiplier;
                break;
            case AttackType.LightAttack02:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightattackStaminaCostMultiplier;
                break;
            case AttackType.LightAttack03:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightattackStaminaCostMultiplier;
                break;
            case AttackType.HeavyAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyattackStaminaCostMultiplier;
                break;
            case AttackType.ChargeAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargeattackStaminaCostMultiplier;
                break;
            case AttackType.ChargeAttack02:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargeattackStaminaCostMultiplier;
                break;
            case AttackType.RunningAttack:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.runningattackStaminaCostMultiplier;
                break;
            case AttackType.RollingAttack:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.rollingattackStaminaCostMultiplier;
                break;
        }
        player.playerStatManager.CurrentStamina -= Mathf.RoundToInt(staminaDeducted);
        //Debug.Log(staminaDeducted + "Stamina has been used");
    }


    public void AttackCoolDown()
    {
        StartCoroutine(WaitForAnotherAttack01());
    }

    public IEnumerator WaitForAnotherAttack01()
    {
        readyToPerformAttack = false;

        yield return new WaitForSeconds(waitingTime);

        readyToPerformAttack = true;
    }

    private Coroutine closeComboCoroutine;
    [HideInInspector] public bool canWeDoAnotherAttack = false;

    public void EnableCanDoCombo()
    {
        canWeDoAnotherAttack = true;
        PlayerManager.instance.playerCombatManager.canComboWithMainHandWeapon = true;
        if (closeComboCoroutine != null)
        {
            StopCoroutine(closeComboCoroutine);
        }

        closeComboCoroutine = StartCoroutine(CloseCombo());
        

    }

    private IEnumerator CloseCombo()
    {
        float timer = 0;
        float duration = 1.5f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        DisableCanDoCombo();
        closeComboCoroutine = null;
    }

    public void DisableCanDoCombo()
    {
        PlayerManager.instance.playerCombatManager.canComboWithMainHandWeapon = false;
        canWeDoAnotherAttack = false;
        //Debug.Log("ยกเลิกคอมโบ");
    }


    public void CheckingCombo()
    {
        if (PlayerManager.instance.playerCombatManager.doAnotherAttack) //รอรับจากLC
        {
            PlayerManager.instance.playerCombatManager.canComboWithMainHandWeapon = true;
            PlayerManager.instance.playerCombatManager.PerformWeaponBasedAction(PlayerManager.instance.playerInventoryManager.currentRightHandWeapon.oh_LC_Action, PlayerManager.instance.playerInventoryManager.currentRightHandWeapon);
            Debug.Log("รันคอมโบอีกครั้ง");
        }
        PlayerManager.instance.playerCombatManager.doAnotherAttack = false;
    }

    public void EnableCanDoRollingAttack()
    {
        canPerformRollingAttack = true;
    }

    public void DisableCanDoRollingAttack()
    {
        canPerformRollingAttack = false;
    }


}
