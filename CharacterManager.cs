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
    }

    protected virtual void FixedUpdate()
    {
        
    }
}
