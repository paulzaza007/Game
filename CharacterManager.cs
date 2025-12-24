using UnityEngine;

public class CharacterManager : MonoBehaviour
{
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

    [HideInInspector] public AICharacterManager aICharacterManager;
    [HideInInspector] public Player player;

    [HideInInspector] public CharacterCurrentStat characterCurrentStat;
    [HideInInspector] public CharacterStatManager characterStatManager;

    [Header("Character Group")]
    public CharacterGroup characterGroup;
    
    protected virtual void Awake()
    {
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

        aICharacterManager = GetComponent<AICharacterManager>();
        player = GetComponent<Player>();


        characterCurrentStat = GetComponent<CharacterCurrentStat>();
        characterStatManager = GetComponent<CharacterStatManager>();
        
    }

    protected virtual void FixedUpdate()
    {
        
    }
}
