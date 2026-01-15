using UnityEngine;

public class CyclopDamageCollider : DamageCollider
{
    [SerializeField] CharacterManager CyclopCausingDamage;

    protected override void Awake()
    {
        base.Awake();
        CyclopCausingDamage = GetComponentInParent<CharacterManager>();
    }

    protected override void DamageTarget(CharacterManager damageTarget)
    {
        if (damageTarget == CyclopCausingDamage)
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
        damageTarget.characterEffectManager.ProcessInstantEffect(damageEffect);
    }
}
