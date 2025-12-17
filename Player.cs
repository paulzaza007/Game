using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;

    [HideInInspector] public PlayerAnimatorManger playerAnimatorManger;
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
    [HideInInspector] public PlayerSFXManager playerSFXManager;

    [Header("Character Group")]
    public CharacterGroup characterGroup;

    public int currentRightHandWeaponID;
    public int currentLeftHandWeaponID;
    public int currentWeaponBeingUsed;
    public bool isUsingRightHand;
    public bool isUsingLeftHand;

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

    public event System.Action<int, int> OnRightHandWeaponIDChanged;
    public event System.Action<int, int> OnLeftHandWeaponIDChanged;
    public event System.Action<int, int> OnWeaponBeingUsed;
    

    protected virtual void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        playerAnimatorManger = GetComponent<PlayerAnimatorManger>();
        playerMovement = GetComponent<PlayerMovement>();
        playerDodge = GetComponent<PlayerDodge>();
        playerInput = GetComponent<PlayerInput>();
        playerStatManager = GetComponent<PlayerStatManager>();
        playerCurrentState = GetComponent<PlayerCurrentState>();
        playerEffectManager = GetComponent<PlayerEffectsManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerSFXManager = GetComponent<PlayerSFXManager>();

        OnRightHandWeaponIDChanged += OnCurrentRightHandWeaponIDChange;
        OnLeftHandWeaponIDChanged += OnCurrentLeftHandWeaponIDChange;
        OnWeaponBeingUsed += OnCurrentWeaponBeingUsedIDChange;


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

    protected virtual void FixedUpdate()
    {
        
    }

    public void SaveGameToCurrentCharacterData(ref PlayerSaveData currentCharacterData)
    {
        currentCharacterData.xPosition = transform.position.x;
        currentCharacterData.yPosition = transform.position.y;
        currentCharacterData.zPosition = transform.position.z;

        currentCharacterData.currentHealth = playerStatManager.currentHealth;
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

        playerStatManager.currentHealth = currentCharacterData.currentHealth;
        playerStatManager.currentStamina = currentCharacterData.currentStamina;
    }

    public IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        playerStatManager.currentHealth = 0f;
        playerCurrentState.isDead = true;

        if (!manuallySelectDeathAnimation)
        {
            playerAnimatorManger.PlayerTargetActionAnimation("Death", true, false);
        }

        PlayerUIManager.instance.playerUIPopUpManager.SendYouDiedPopUp();
        yield return new WaitForSeconds(5);
    }

    public void OnCurrentRightHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));
        playerInventoryManager.currentRightHandWeapon = newWeapon;
        playerEquipmentManager.LoadRightWeapon();
    }

    public void OnCurrentLeftHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newID));
        playerInventoryManager.currentLeftHandWeapon = newWeapon;
        playerEquipmentManager.LoadLeftWeapon();
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

    public void PlayerWeaponAction(int actionID, int weaponID)
    {
        WeaponItemAction weaponActionItem = WorldActionManager.instance.GetWeaponActionItemByID(actionID);

        if(weaponActionItem != null)
        {
            weaponActionItem.AttemptToPerformAction(Player.instance, WorldItemDatabase.instance.GetWeaponByID(weaponID));
        }
        else
        {
            Debug.Log("ACTION IS NULL");
        }
    }

    

}
