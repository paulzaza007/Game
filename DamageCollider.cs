using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Header("Collider")]
    [SerializeField] protected Collider damageCollider;
    [SerializeField] protected Collider[] damageColliders;

    int hitBoxLayer;
    int damageColliderLayer;

    [Header("Damage")]
    public float physicalDamage = 0;
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;

    [Header("Contact Point")]
    protected Vector3 contactPoint;

    [Header("Player Damaged")]
    protected List<CharacterManager> characterDamaged = new List<CharacterManager>();

    protected virtual void Awake()
    {
        hitBoxLayer = LayerMask.NameToLayer("HitBox");
        damageColliderLayer = LayerMask.NameToLayer("DamageCollider");
    }
 
    protected virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log("1. Collider Hit: " + other.name);
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();
        

        if(damageTarget != null)
        {
            contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            if (damageTarget.characterCurrentState.isInvulnerable)
            {
                return;
            }

            if(other.gameObject.layer == hitBoxLayer || other.gameObject.layer == damageColliderLayer)
            {
                return;
            }

            DamageTarget(damageTarget);
        }
    }

    protected virtual void DamageTarget(CharacterManager damageTarget)
    {
        if (characterDamaged.Contains(damageTarget))
        {
            return;
        }
        characterDamaged.Add(damageTarget);
        //Debug.Log("ตี");

        TakeDamageEffect damageEffect = Instantiate(WorldPlayerEffectManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.lightningDamage = lightningDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.contactPoint = contactPoint;

        damageTarget.characterEffectManager.ProcessInstantEffect(damageEffect);
    }

    public virtual void EnableDamageCollider()
    {
        if(damageCollider != null)
        {
            damageCollider.enabled = true;
        }
        foreach (var collider in damageColliders)
        {
            collider.enabled = true;
        }

    }

    public virtual void DisableDamageCollider()
    {
        if (damageCollider != null)
        {
            damageCollider.enabled = false;
        }
        foreach (var collider in damageColliders)
        {
            collider.enabled = false;
        }
        characterDamaged.Clear();


    }
}
