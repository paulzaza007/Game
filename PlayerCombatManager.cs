using UnityEngine;
using System.Collections;

public class PlayerCombatManager : MonoBehaviour
{
    protected Player player;

    [Header("Last Attack Animation Perform")]
    public string lastAttackAnimationPerform;

    [Header("Attack Target")]
    public Player currentTarget;
    public AICharacterManager lockedOnEnemy;

    [Header("Attack Type")]
    public AttackType currentAttackType;

    [Header("Lock On Target Transform")]
    public Transform LockOnTargetTransform;

    public WeaponItem currentWeaponBeingUsed;

    [Header("Flags")]
    public bool canComboWithMainHandWeapon = false;
    //public bool canComboWithOffHandWeapon = false;

    [Header("Attack Cooldown")]
    [SerializeField] public bool readyToPerformAttack = true;
    [SerializeField] float waitingTime = 2;


    protected virtual void Awake()
    {
        player = GetComponent<Player>();
    }

    public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
    {
        currentWeaponBeingUsed = weaponPerformingAction;
        
        weaponAction.AttemptToPerformAction(Player.instance, weaponPerformingAction);   

        Player.instance.PlayerWeaponAction(weaponAction.actionID, weaponPerformingAction.itemID);
    }

    public void DrainStaminaBasedOnAttack()
    {
        if(currentWeaponBeingUsed == null)
        {
            return;
        }

        float staminaDeducted = 0;

        switch (currentAttackType)
        {
            case AttackType.LightAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.light_Attack_01_Modifier;
                break;
            default:
                break;
        }

        player.playerStatManager.currentStamina -= Mathf.RoundToInt(staminaDeducted);
        //Debug.Log(staminaDeducted + "Stamina has been used");
    }

    public void SetLockOnTarget(AICharacterManager newTarget)
    {
        if(newTarget != null)
        {
            lockedOnEnemy = newTarget;
            CameraPlayer.instance.SetLockCameraHeight();
        }
        else
        {
            lockedOnEnemy = null;
            CameraPlayer.instance.SetLockCameraHeight();
        }
    }

    // 2. ฟังก์ชันสำหรับ "AI" ใช้เล็งเป้า "ผู้เล่น"
    public void SetAttackTarget(Player newTarget)
    {
        if(newTarget != null)
        {
            currentTarget = newTarget;
        }
        else
        {
            currentTarget = null;
        }
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


}
