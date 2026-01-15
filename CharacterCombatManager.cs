using System.Collections;
using UnityEngine;

public class CharacterCombatManager : MonoBehaviour
{
    protected PlayerManager player;

    [Header("Last Attack Animation Perform")]
    public string lastAttackAnimationPerform;

    [Header("Attack Target")]
    public PlayerManager currentTarget;
    public AICharacterManager lockedOnEnemy;

    [Header("Attack Type")]
    public AttackType currentAttackType;

    [Header("Lock On Target Transform")]
    public Transform LockOnTargetTransform;

    public WeaponItem currentWeaponBeingUsed;

    [Header("Flags")]
    public bool canComboWithMainHandWeapon = false;
    //public bool canComboWithOffHandWeapon = false;

    [SerializeField] public bool doAnotherAttack = false;


    protected virtual void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    // ตอนนี้สั่งจาก playerInput กับ playerCombat
    // อนาคตสั่งจาก AI ได้
    public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
    {
        currentWeaponBeingUsed = weaponPerformingAction;

        if (weaponAction != null)
        {
            //weaponAction รองรับการสั่งโจมตี
            //weaponPerformingAction ส่งอาวุธที่ใช้โจมตี
            weaponAction.AttemptToPerformAction(player, weaponPerformingAction);
        }
    }

    public void SetLockOnTarget(AICharacterManager newTarget)
    {
        if (newTarget != null)
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
    public void SetAttackTarget(PlayerManager newTarget)
    {
        if (newTarget != null)
        {
            currentTarget = newTarget;
        }
        else
        {
            currentTarget = null;
        }
    }

    public void EnableIsInvulnerable()
    {
        player.playerCurrentState.isInvulnerable = true;
    }

    public void DisableIsInvulnerable()
    {
        player.playerCurrentState.isInvulnerable = false;
    }

}
