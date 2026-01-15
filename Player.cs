using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerAnimatorManger playerAnimatorManager;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerDodge playerDodge;
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public PlayerStatManager playerStatManager;
    [HideInInspector] public PlayerCurrentState playerCurrentState;
    [HideInInspector] public PlayerEffectsManager playerEffectManager;
    [HideInInspector] public PlayerUIPopUpManager playerUIPopUpManager;
    [HideInInspector] public PlayerInventoryManager playerInventoryManager;
    [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
    [HideInInspector] public PlayerCombatManager playerCombatManager;
    [HideInInspector] public PlayerInteractionManager playerInteractionManager;

    public static PlayerManager instance;
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;

    // EVENT
    public int currentRightHandWeaponID;
    public bool isChargingAttack = false;

    public int CurrentRightHandWeaponID
    {
        get => currentRightHandWeaponID;
        set
        {
            if (currentRightHandWeaponID == value) return;

            int old = currentRightHandWeaponID;
            currentRightHandWeaponID = value;

            OnRightHandWeaponIDChanged?.Invoke(old, value);
        }
    }

    public bool IsChargingAttack
    {
        get => isChargingAttack;
        set
        {
            if (isChargingAttack == value) return;

            bool old = isChargingAttack;
            isChargingAttack = value;

            OnChargingAttack?.Invoke(old, value);
        }
    }

    public event System.Action<int, int> OnRightHandWeaponIDChanged;
    public event System.Action<bool, bool> OnChargingAttack;


    protected override void Awake()
    {
        base.Awake();
        instance = this;

        //รับ component จากระบบควบคุมการเคลื่อนไหว กับ animator
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        //รับ component จากทุกplayer เพื่อให้เรียกหากันสะดวก
        playerAnimatorManager = GetComponent<PlayerAnimatorManger>();
        playerMovement = GetComponent<PlayerMovement>();
        playerDodge = GetComponent<PlayerDodge>();
        playerInput = GetComponent<PlayerInput>();
        playerStatManager = GetComponent<PlayerStatManager>();
        playerCurrentState = GetComponent<PlayerCurrentState>();
        playerEffectManager = GetComponent<PlayerEffectsManager>();
        playerUIPopUpManager = GetComponent<PlayerUIPopUpManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerInteractionManager = GetComponent<PlayerInteractionManager>();
        characterSFXManager = GetComponent<CharacterSFXManager>();

        //subscibe ให้event
        OnRightHandWeaponIDChanged += OnCurrentRightHandWeaponIDChange;
        OnChargingAttack += OnChargingAttackBoolChange;


    }

    // สั่งจาก WorldSaveGame ฟังก์ชั่น save()
    public void SaveGameToCurrentCharacterData(ref PlayerSaveData currentCharacterData)
    {
        //เก็บข้อมูลผู้เล่น
        currentCharacterData.xPosition = transform.position.x;
        currentCharacterData.yPosition = transform.position.y;
        currentCharacterData.zPosition = transform.position.z;

        currentCharacterData.currentHealth = playerStatManager.CurrentHealth;
        currentCharacterData.currentStamina = (int)playerStatManager.CurrentStamina;

        currentCharacterData.vitality = playerStatManager.vitality;
        currentCharacterData.endurance = playerStatManager.endurance;
    }

    // สั่งจาก WorldSaveGame ฟังก์ชั่น LoadWorldScene() 
    public void LoadGameToCurrentCharacterData(ref PlayerSaveData currentCharacterData)
    {
        //ให้ผู้เล่นอยู่ในตำแหนงเซฟ
        Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
        transform.position = myPosition;

        //เซ็ต stat ให้ผู้เล่น
        playerStatManager.vitality = currentCharacterData.vitality;
        playerStatManager.endurance = currentCharacterData.endurance;

        playerStatManager.CurrentHealth = currentCharacterData.currentHealth;
        playerStatManager.CurrentStamina = currentCharacterData.currentStamina;
    }


    //ผู้เล่นตาย สั่งจาก OnCurrentHealthChange ที่ไว้เช็คเลือด
    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        playerStatManager.CurrentHealth = 0;
        playerCurrentState.isDead = true;

        if (!manuallySelectDeathAnimation)
        {
            playerAnimatorManager.PlayerTargetActionAnimation("Death", true, false);
        }

        //ขึ้นข้อความว่าตาย
        PlayerUIManager.instance.playerUIPopUpManager.SendYouDiedPopUp();
        yield return new WaitForSeconds(5);
    }

    public void OnCurrentRightHandWeaponIDChange(int oldID, int newID) //เมื่อ ID อาวุธเปลี่ยน ฟังชั่นนี้ทำงาน 
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));
        playerInventoryManager.currentRightHandWeapon = newWeapon;
        StartCoroutine(WaitAndLoadRightWeapon());

        PlayerUIManager.instance.playerUIHUDManager.SetRightWeaponQuickSlotIcon(newID);
    }

    private IEnumerator WaitAndLoadRightWeapon() //มีไว้เพื่อให้เสกอาวุธมาให้ตรงจังหวะกับอนิเมชั่นหยิบดาบ
    {
        yield return new WaitForSeconds(playerInput.switchingWeaponTime);

        playerEquipmentManager.LoadRightWeapon();
    }


    public void OnChargingAttackBoolChange(bool oldBool, bool newBool)
    {
        animator.SetBool("isCharging", newBool);
    }

}
