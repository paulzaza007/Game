using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour
{
    CharacterManager character;
    
    public WeaponLocation rightHandSlot;
    public WeaponLocation leftHandSlot;

    [SerializeField] WeaponManager rightWeaponManager;
    [SerializeField] WeaponManager leftWeaponManager;

    public GameObject rightHandWeaponModel;
    public GameObject leftHandWeaponModel;

    private void Awake()
    {
        character = GetComponent<CharacterManager>();
        InitializeWeaponSlot();
    }

    private void Start()
    {
        LoadWeaponOnBothHands();
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

    public void LoadWeaponOnBothHands()
    {
        LoadLeftWeapon();
        LoadRightWeapon();
    }

    // ---------------------------------------------------------
    // แก้ไขส่วน RIGHT WEAPON
    // ---------------------------------------------------------
    public void LoadRightWeapon()
    {
        // 1. ทำลายอาวุธเก่าก่อนเสมอ เพื่อกันโมเดลซ้อน
        rightHandSlot.UnloadWeapon();

        if (Player.instance.playerInventoryManager.currentRightHandWeapon != null)
        {
            string weaponName = Player.instance.playerInventoryManager.currentRightHandWeapon.itemName;

            var weaponItem = Player.instance.playerInventoryManager.currentRightHandWeapon;

            // ถ้าเป็นดาบ ให้โหลดโมเดล
            if (Player.instance.playerInventoryManager.currentRightHandWeapon.weaponModel != null)
            {
                rightHandWeaponModel = rightHandSlot.LoadWeapon(weaponItem.weaponModel);
                
                rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();

                // Setup Damage Collider
                rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
                if(rightWeaponManager != null)
                {
                    rightWeaponManager.SetWeaponDamage(character, weaponItem);
                }
            }
        }
    }

    public void SwitchRightWeapon()
    {
        Player.instance.playerAnimatorManager.PlayerTargetActionAnimation("Swap_Right_Weapon_01", false, false, true, true);

        WeaponItem selectedWeapon = null;
        
        // ดึงข้อมูล Inventory มาให้สั้นลงเพื่อให้อ่านง่าย
        var inventory = Player.instance.playerInventoryManager;
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
                Player.instance.CurrentRightHandWeaponID = selectedWeapon.itemID; 
                return; // จบการทำงานทันที
            }
        }

        // --- ถ้าวนลูปครบทุกช่องแล้วไม่เจออาวุธเลย (หรือเจอแค่อันเดียวแล้ววนกลับมาที่เดิม) ---
        
        // กรณี: ผู้เล่นถอดอาวุธออกหมด หรือเหลือแค่อันเดียวแล้วอยากสลับเป็นมือเปล่า
        // ให้ใส่มือเปล่า (Unarmed)
        if (selectedWeapon == null)
        {
            Player.instance.CurrentRightHandWeaponID = WorldItemDatabase.instance.unarmedWeapon.itemID;
        }
    }

    // ---------------------------------------------------------
    // แก้ไขส่วน LEFT WEAPON (ทำเหมือนขวา)
    // ---------------------------------------------------------
    public void LoadLeftWeapon()
    {
        // 1. Unload ก่อน
        leftHandSlot.UnloadWeapon();

        if (Player.instance.playerInventoryManager.currentLeftHandWeapon != null)
        {
            if (Player.instance.playerInventoryManager.currentLeftHandWeapon.weaponModel != null)
            {
                leftHandWeaponModel = Instantiate(Player.instance.playerInventoryManager.currentLeftHandWeapon.weaponModel);
                leftHandSlot.LoadWeapon(leftHandWeaponModel);
                
                leftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
                if(leftWeaponManager != null)
                {
                    leftWeaponManager.SetWeaponDamage(character, Player.instance.playerInventoryManager.currentLeftHandWeapon);
                }
            }
        }
    }

    public void SwitchLeftWeapon()
    {
        Player.instance.playerAnimatorManager.PlayerTargetActionAnimation("Swap_Left_Weapon_01", false, false, true, true);

        WeaponItem selectedWeapon = null;

        // ดึงข้อมูล Inventory มาให้สั้นลงเพื่อให้อ่านง่าย
        var inventory = Player.instance.playerInventoryManager;
        var weapons = inventory.weaponInLefthandSlots;

        // วนลูปเช็ค Weapon Slot ถัดไป (Logic แบบ Dark Souls)
        // วนสูงสุดแค่จำนวนช่องที่มี (เช่น 3 รอบ) เพื่อกัน Infinite Loop
        for (int i = 0; i < weapons.Length; i++)
        {
            // ขยับ Index ไปข้างหน้า 1 ช่อง
            inventory.leftHandWeaponIndex += 1;

            // ถ้าเกินจำนวนช่อง ให้กลับไปเริ่มที่ 0
            if (inventory.leftHandWeaponIndex >= weapons.Length)
            {
                inventory.leftHandWeaponIndex = 0;
            }

            // เช็คว่าช่องนี้มีอาวุธใส่ไว้ไหม และต้องไม่ใช่มือเปล่า (Unarmed)
            WeaponItem checkingWeapon = weapons[inventory.leftHandWeaponIndex];

            if (checkingWeapon != null && checkingWeapon.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
            {
                // เจออาวุธแล้ว!
                selectedWeapon = checkingWeapon;
                // อัปเดต ID เพื่อไป Trigger Event ใน Player.cs
                Player.instance.CurrentLeftHandWeaponID = selectedWeapon.itemID;
                return; // จบการทำงานทันที
            }
        }

        // --- ถ้าวนลูปครบทุกช่องแล้วไม่เจออาวุธเลย (หรือเจอแค่อันเดียวแล้ววนกลับมาที่เดิม) ---

        // กรณี: ผู้เล่นถอดอาวุธออกหมด หรือเหลือแค่อันเดียวแล้วอยากสลับเป็นมือเปล่า
        // ให้ใส่มือเปล่า (Unarmed)
        if (selectedWeapon == null)
        {
            Player.instance.CurrentLeftHandWeaponID = WorldItemDatabase.instance.unarmedWeapon.itemID;
        }
    }

    public void OpenDamageCollider()
    {
        if (Player.instance.isUsingRightHand)
        {
            rightWeaponManager.meleeWeaponDamageCollider.EnableDamageCollider();
            character.playerSFXManager.PlaySFX(WorldSFXManager.instance.ChooseRandomSFXFromArray(character.playerInventoryManager.currentRightHandWeapon.whoosh), 0.3f);
            //Debug.Log("เปิด Collider ข้างขวา");
        }
        else if (Player.instance.isUsingLeftHand)
        {
            leftWeaponManager.meleeWeaponDamageCollider.EnableDamageCollider();
            character.playerSFXManager.PlaySFX(WorldSFXManager.instance.ChooseRandomSFXFromArray(character.playerInventoryManager.currentRightHandWeapon.whoosh));
        }
    }

    public void CloseDamageCollider()
    {
        if (Player.instance.isUsingRightHand)
        {
            rightWeaponManager.meleeWeaponDamageCollider.DisableDamageCollider();
            //Debug.Log("ปิด Collider ข้างขวา");
        }
        else if (Player.instance.isUsingLeftHand)
        {
            leftWeaponManager.meleeWeaponDamageCollider.DisableDamageCollider();
        }
    }
}