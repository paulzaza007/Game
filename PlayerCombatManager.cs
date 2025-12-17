using UnityEngine;

public class PlayerCombatManager : MonoBehaviour
{
    protected Player player;

    [Header("Attack Target")]
    public Player currentTarget;
    public AICharacterManager lockedOnEnemy;

    [Header("Attack Type")]
    public AttackType currentAttackType;

    [Header("Lock On Target Transform")]
    public Transform LockOnTargetTransform;

    public WeaponItem currentWeaponBeingUsed;


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
        Debug.Log("Working!");
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
        Debug.Log(staminaDeducted + "Stamina has been used");
    }

    public void SetLockOnTarget(AICharacterManager newTarget)
    {
        if(newTarget != null)
        {
            lockedOnEnemy = newTarget;
        }
        else
        {
            lockedOnEnemy = null;
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
 
}
