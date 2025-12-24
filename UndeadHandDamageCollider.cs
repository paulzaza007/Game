using UnityEngine;

public class UndeadHandDamageCollider : DamageCollider
{
    [SerializeField] AICharacterManager undeadCausingDamage;

    protected override void Awake()
    {
        base.Awake();
 
        damageCollider = GetComponent<Collider>();
        undeadCausingDamage = GetComponentInParent<AICharacterManager>();
    }

    protected override void DamageTarget(CharacterManager damageTarget)
    {
        if(damageTarget == undeadCausingDamage)
        {
            return;
        }

        if (characterDamaged.Contains(damageTarget))
        {
            return;
        }
        characterDamaged.Add(damageTarget);

        TakeDamageEffect damageEffect = Instantiate(WorldPlayerEffectManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.lightningDamage = lightningDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.contactPoint = contactPoint;
        //damageEffect.angleHitFrom = Vector3.SignedAngle(undeadCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);
        damageTarget.playerEffectManager.ProcessInstantEffect(damageEffect);


        //Player.instance.playerEffectManager.ProcessInstantEffect(damageEffect);
    }
}
