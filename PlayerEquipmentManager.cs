using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour
{
    PlayerManager player;
    
    public WeaponLocation rightHandSlot;
    public WeaponLocation leftHandSlot;

    [SerializeField] WeaponManager rightWeaponManager;
    [SerializeField] WeaponManager leftWeaponManager;

    public GameObject rightHandWeaponModel;
    public GameObject leftHandWeaponModel;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
        InitializeWeaponSlot();
    }

    private void Start()
    {
        LoadRightWeapon();
    }

    private void InitializeWeaponSlot()
    {
        WeaponLocation[] weaponSlots = GetComponentsInChildren<WeaponLocation>();

        foreach (var weaponSlot in weaponSlots)
        {
            if (weaponSlot.weaponSlot == WeaponModelSlot.RightHand)
            {
                rightHandSlot = weaponSlot;
            }
            else if (weaponSlot.weaponSlot == WeaponModelSlot.LeftHand)
            {
                leftHandSlot = weaponSlot;
            }
        }
    }
    public void LoadRightWeapon()
    {
        // 1. ทำลายอาวุธเก่าก่อนเสมอ เพื่อกันโมเดลซ้อน
        rightHandSlot.UnloadWeapon();

        if (player.playerInventoryManager.currentRightHandWeapon != null)
        {
            string weaponName = player.playerInventoryManager.currentRightHandWeapon.itemName;

            var weaponItem = player.playerInventoryManager.currentRightHandWeapon;

            // ถ้าเป็นดาบ ให้โหลดโมเดล
            if (player.playerInventoryManager.currentRightHandWeapon.weaponModel != null)
            {
                rightHandWeaponModel = rightHandSlot.LoadWeapon(weaponItem.weaponModel);
                
                rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();

                // Setup Damage Collider
                rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
                if(rightWeaponManager != null)
                {
                    rightWeaponManager.SetWeaponDamage(player, weaponItem);
                }
            }
        }
    }

    public void SwitchRightWeapon()
    {
        player.playerAnimatorManager.PlayerTargetActionAnimation("Swap_Right_Weapon_01", false, false, true, true);

        WeaponItem selectedWeapon = null;
        
        // ดึงข้อมูล Inventory มาให้สั้นลงเพื่อให้อ่านง่าย
        var inventory = player.playerInventoryManager;
        var weapons = inventory.weaponInRighthandSlots;
        
        // วนลูปเช็ค Weapon Slot ถัดไป (Logic แบบ Dark Souls)
        // วนสูงสุดแค่จำนวนช่องที่มี (เช่น 3 รอบ) เพื่อกัน Infinite Loop
        for (int i = 0; i < weapons.Length; i++)
        {
            // ขยับ Index ไปข้างหน้า 1 ช่อง
            inventory.rightHandWeaponIndex += 1;

            // ถ้าเกินจำนวนช่อง ให้กลับไปเริ่มที่ 0
            if (inventory.rightHandWeaponIndex >= weapons.Length)
            {
                inventory.rightHandWeaponIndex = 0;
            }

            // เช็คว่าช่องนี้มีอาวุธใส่ไว้ไหม และต้องไม่ใช่มือเปล่า (Unarmed)
            WeaponItem checkingWeapon = weapons[inventory.rightHandWeaponIndex];

            if (checkingWeapon != null && checkingWeapon.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
            {
                // เจออาวุธแล้ว!
                selectedWeapon = checkingWeapon;
                // อัปเดต ID เพื่อไป Trigger Event ใน Player.cs
                player.CurrentRightHandWeaponID = selectedWeapon.itemID; 
                return; // จบการทำงานทันที
            }
        }

        // --- ถ้าวนลูปครบทุกช่องแล้วไม่เจออาวุธเลย (หรือเจอแค่อันเดียวแล้ววนกลับมาที่เดิม) ---
        
        // กรณี: ผู้เล่นถอดอาวุธออกหมด หรือเหลือแค่อันเดียวแล้วอยากสลับเป็นมือเปล่า
        // ให้ใส่มือเปล่า (Unarmed)
        if (selectedWeapon == null)
        {
            player.CurrentRightHandWeaponID = WorldItemDatabase.instance.unarmedWeapon.itemID;
        }
    }

    public void OpenDamageCollider()
    {
        rightWeaponManager.meleeWeaponDamageCollider.EnableDamageCollider();
        //Debug.Log("ปิด Collider ข้างขวา");

    }

    public void CloseDamageCollider()
    {
        rightWeaponManager.meleeWeaponDamageCollider.DisableDamageCollider();
        //Debug.Log("ปิด Collider ข้างขวา");
        
    }
}