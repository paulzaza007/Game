using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [Header("UI")]
    public UI_Character_HP_Bar characterHPBar;

    [HideInInspector] public AICharacterManager aICharacterManager;
    [HideInInspector] public PlayerManager player;

    [HideInInspector] public CharacterCurrentState characterCurrentState;
    [HideInInspector] public CharacterStatManager characterStatManager;
    [HideInInspector] public CharacterSFXManager characterSFXManager;
    [HideInInspector] public CharacterEffectManager characterEffectManager;
    [HideInInspector] public CharacterCombatManager characterCombatManager;
    [HideInInspector] public CharacterUIManager characterUIManager;

    [Header("Character Group")]
    public CharacterGroup characterGroup;
    
    protected virtual void Awake()
    {
        
        

        aICharacterManager = GetComponent<AICharacterManager>();
        player = GetComponent<PlayerManager>();

        characterCurrentState = GetComponent<CharacterCurrentState>();
        characterStatManager = GetComponent<CharacterStatManager>();
        characterSFXManager = GetComponent<CharacterSFXManager>();
        characterEffectManager = GetComponent<CharacterEffectManager>();
        characterCombatManager = GetComponent<CharacterCombatManager>();
        characterUIManager = GetComponent<CharacterUIManager>();

        characterStatManager.OnCurrentHealth += OnCurrentHealthChange;

    }

    protected virtual void FixedUpdate()
    {
        
    }

    protected virtual void Start()
    {
        IgnoreMyOwnColliders();
    }

    protected virtual void OnEnable()
    {
        
    }

    protected virtual void OnDisable()
    {

    }

    protected virtual void OnCurrentHealthChange(int oldFloat, int newFloat)
    {
        if (characterStatManager.CurrentHealth <= 0)
        {
            StartCoroutine(ProcessDeathEvent());
            characterStatManager.OnCurrentHealth -= OnCurrentHealthChange;
        }
    }

    public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        yield return null;
    }

    protected virtual void IgnoreMyOwnColliders()
    {
        Collider playerControllerCollider = GetComponent<Collider>();
        Collider[] damageablePlayerColliders = GetComponentsInChildren<Collider>();

        List<Collider> ignoreCollider = new List<Collider>();

        foreach (var collider in damageablePlayerColliders)
        {
            ignoreCollider.Add(collider);
        }

        ignoreCollider.Add(playerControllerCollider);

        foreach (var collider in ignoreCollider)
        {
            foreach (var otherCollider in ignoreCollider)
            {
                Physics.IgnoreCollision(collider, otherCollider, true);
            }
        }
    }
}
