using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CharacterManager
{
    public static Player instance;
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;

    public int currentRightHandWeaponID;
    public int currentLeftHandWeaponID;
    public int currentWeaponBeingUsed;
    public bool isUsingRightHand;
    public bool isUsingLeftHand;
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

    public int CurrentLeftHandWeaponID
    {
        get => currentLeftHandWeaponID;
        set
        {
            if (currentLeftHandWeaponID == value) return;

            int old = currentLeftHandWeaponID;
            currentLeftHandWeaponID = value;

            OnLeftHandWeaponIDChanged?.Invoke(old, value);
        }
    }

    public int CurrentWeaponBeingUsed
    {
        get => currentWeaponBeingUsed;
        set
        {
            if (currentWeaponBeingUsed == value) return;

            int old = currentWeaponBeingUsed;
            currentWeaponBeingUsed = value;

            OnWeaponBeingUsed?.Invoke(old, value);
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
    public event System.Action<int, int> OnLeftHandWeaponIDChanged;
    public event System.Action<int, int> OnWeaponBeingUsed;
    public event System.Action<bool, bool> OnChargingAttack;


    protected override void Awake()
    {
        instance = this;

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

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
        characterSFXManager = GetComponent<CharacterSFXManager>();

        OnRightHandWeaponIDChanged += OnCurrentRightHandWeaponIDChange;
        OnLeftHandWeaponIDChanged += OnCurrentLeftHandWeaponIDChange;
        OnWeaponBeingUsed += OnCurrentWeaponBeingUsedIDChange;
        OnChargingAttack += OnChargingAttackBoolChange;


    }

    private void Start()
    {
        IgnoreMyOwnColliders();
    }

    private void Update()
    {
        playerDodge.HandleDodge();

        playerMovement.HandleMovement();

        playerStatManager.RegenerateStamina();
    }

    public void SaveGameToCurrentCharacterData(ref PlayerSaveData currentCharacterData)
    {
        currentCharacterData.xPosition = transform.position.x;
        currentCharacterData.yPosition = transform.position.y;
        currentCharacterData.zPosition = transform.position.z;

        currentCharacterData.currentHealth = playerStatManager.CurrentHealth;
        currentCharacterData.currentStamina = playerStatManager.currentStamina;

        currentCharacterData.vitality = playerStatManager.vitality;
        currentCharacterData.endurance = playerStatManager.endurance;
    }

    public void LoadGameToCurrentCharacterData(ref PlayerSaveData currentCharacterData)
    {
        Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
        transform.position = myPosition;

        playerStatManager.vitality = currentCharacterData.vitality;
        playerStatManager.endurance = currentCharacterData.endurance;

        playerStatManager.CurrentHealth = currentCharacterData.currentHealth;
        playerStatManager.currentStamina = currentCharacterData.currentStamina;
    }

    public IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        playerStatManager.CurrentHealth = 0f;
        playerCurrentState.isDead = true;

        if (!manuallySelectDeathAnimation)
        {
            playerAnimatorManager.PlayerTargetActionAnimation("Death", true, false);
        }

        PlayerUIManager.instance.playerUIPopUpManager.SendYouDiedPopUp();
        yield return new WaitForSeconds(5);
    }

    public void OnCurrentRightHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));
        playerInventoryManager.currentRightHandWeapon = newWeapon;
        StartCoroutine(WaitAndLoadRightWeapon());

        PlayerUIManager.instance.playerUIHUDManager.SetRightWeaponQuickSlotIcon(newID);
    }
    private IEnumerator WaitAndLoadRightWeapon()
    {
        yield return new WaitForSeconds(playerInput.switchingWeaponTime);

        playerEquipmentManager.LoadRightWeapon();
    }

    public void OnCurrentLeftHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));
        playerInventoryManager.currentLeftHandWeapon = newWeapon;
        StartCoroutine(WaitAndLoadLeftWeapon());

        PlayerUIManager.instance.playerUIHUDManager.SetLeftWeaponQuickSlotIcon(newID);
    }
    private IEnumerator WaitAndLoadLeftWeapon()
    {
        yield return new WaitForSeconds(playerInput.switchingWeaponTime);

        playerEquipmentManager.LoadLeftWeapon();
    }

    public void OnChargingAttackBoolChange(bool oldBool, bool newBool)
    {
        animator.SetBool("isCharging", newBool);
    }

    private void IgnoreMyOwnColliders()
    {
        Collider playerControllerCollider = GetComponent<Collider>();
        Collider[] damageablePlayerColliders = GetComponentsInChildren<Collider>();

        List<Collider> ignoreCollider = new List<Collider>();

        foreach (var collider in damageablePlayerColliders)
        {
            ignoreCollider.Add(collider);
        }

        ignoreCollider.Add(playerControllerCollider);

        foreach(var collider in ignoreCollider)
        {
            foreach(var otherCollider in ignoreCollider)
            {
                Physics.IgnoreCollision(collider, otherCollider, true);
            }
        }
    }
    public void OnCurrentWeaponBeingUsedIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));
        playerCombatManager.currentWeaponBeingUsed = newWeapon;

    }

    public void SetPlayerActionHand(bool rightHandAction)
    {
        if (rightHandAction)
        {
            isUsingLeftHand = false;
            isUsingRightHand = true;
        }
        else
        {
            isUsingLeftHand = true;
            isUsingRightHand = false;
        }
    }

    /*public void PlayerWeaponAction(int actionID, int weaponID)
    {
        WeaponItemAction weaponActionItem = WorldActionManager.instance.GetWeaponActionItemByID(actionID);

        if(weaponActionItem != null)
        {
            weaponActionItem.AttemptToPerformAction(this, WorldItemDatabase.instance.GetWeaponByID(weaponID));
        }
        else
        {
            Debug.Log("ACTION IS NULL");
        }
    }*/


    



}
