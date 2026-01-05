using UnityEngine;

public class AICyclopCombatManager : AICharacterCombatManager
{
    
    private AICyclopCombatManager cyclop;

    [Header("Damage Colliders")]
    [SerializeField] CyclopDamageCollider hammerDamageCollider;
    [SerializeField] CyclopDamageCollider leftHandDamageCollider;
    [SerializeField] CyclopDamageCollider rightHandDamageCollider;

    [Header("Damage")]
    [SerializeField] int baseDamage = 25;
    [SerializeField] float attack01DamageModifier = 1.0f;
    [SerializeField] float attack02DamageModifier = 1.4f;
    [SerializeField] float attack03DamageModifier = 1.0f;
    [SerializeField] float attack04DamageModifier = 1.4f;
    [SerializeField] float attack05DamageModifier = 1.0f;
    [SerializeField] float attack06DamageModifier = 1.4f;

    protected override void Awake()
    {
        cyclop = GetComponent<AICyclopCombatManager>();
    }

    private void Update()
    {
        
    }

    public void Set_Cyclop_Attack_01_Damage() //01
    {
        hammerDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
    }

    public void Set_Cyclop_JumpToSmash_Attack_Damage() //02
    {
        hammerDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
    }

    public void Set_Cyclop_Swing_2Hit_Attack_Damage() //03
    {
        hammerDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
    }

    public void Set_Cyclop_360_Attack_Damage() //04
    {
        hammerDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
    }

    public void Set_Cyclop_Smash_Attack_Damage() //05
    {
        hammerDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
    }
    
    public void Set_Cyclop_Sweep_Attack_Damage() //06
    {
        hammerDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
    }

    public void OpenHammerDamageCollider()
    {
        hammerDamageCollider.EnableDamageCollider();
    }

    public void CloseHammerDamageCollider()
    {
        hammerDamageCollider.DisableDamageCollider();
    }

    public void OpenLeftHandDamageCollider()
    {
        leftHandDamageCollider.EnableDamageCollider();
    }

    public void CloseLeftHandDamageCollider()
    {
        leftHandDamageCollider.DisableDamageCollider();
    }

    public void OpenRightHandDamageCollider()
    {
        rightHandDamageCollider.EnableDamageCollider();
    }

    public void CloseRightHandDamageCollider()
    {
        rightHandDamageCollider.DisableDamageCollider();
    }

    public void OpenBothHandDamageCollider()
    {
        OpenLeftHandDamageCollider();
        OpenRightHandDamageCollider();
    }

    public void CloseBothHandDamageCollider()
    {
        CloseLeftHandDamageCollider();
        CloseRightHandDamageCollider();
    }
}


   