using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{
    [Header("Attacking Character")]
    public Player playerCausingDamage;

    [Header("Weapon Attack Modifiers")]
    public float light_Attack_01_Modifier;

    protected override void Awake()
    {
        base.Awake();

        if(damageCollider == null)
        {
            damageCollider = GetComponent<Collider>();
        }

        damageCollider.enabled = false;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        Player damageTarget = other.GetComponentInParent<Player>();

        if(damageTarget == playerCausingDamage)
            return;

        if(damageTarget != null)
        {
            contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            DamageTarget(damageTarget);
        }
    }

    protected override void DamageTarget(Player damageTarget)
    {
        if (playerDamaged.Contains(damageTarget))
        {
            return;
        }
        playerDamaged.Add(damageTarget);

        TakeDamageEffect damageEffect = Instantiate(WorldPlayerEffectManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.lightningDamage = lightningDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.contactPoint = contactPoint;
        damageEffect.angleHitFrom = Vector3.SignedAngle(playerCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);

        switch (playerCausingDamage.playerCombatManager.currentAttackType)
        {
            case AttackType.LightAttack01:
                ApplyAttackDamageModifier(light_Attack_01_Modifier, damageEffect);
                break;
            default:
                break;
        }

        //Player.instance.playerEffectManager.ProcessInstantEffect(damageEffect);
    }

    private void ApplyAttackDamageModifier(float modifier, TakeDamageEffect damage)
    {
        damage.physicalDamage *= modifier;
        damage.magicDamage *= modifier;
        damage.fireDamage *= modifier;
        damage.holyDamage *= modifier;
        damage.lightningDamage *= modifier;
        damage.poiseDamage *= modifier;


    }
}
