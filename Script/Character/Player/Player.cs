using System.Collections;
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
    [HideInInspector] public PlayerEffectManager playerEffectManager;
    [HideInInspector] public PlayerUIPopUpManager playerUIPopUpManager;
    [HideInInspector] public PlayerInventoryManager playerInventoryManager;

    private void Awake()
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
        playerEffectManager = GetComponent<PlayerEffectManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();


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

}
