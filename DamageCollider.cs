using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Header("Collider")]
    [SerializeField] protected Collider damageCollider;

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
        
    }
 
    protected virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log("1. Collider Hit: " + other.name);
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();
        

        if(damageTarget != null)
        {
            Debug.Log("2. Target Found, HP: " + damageTarget.characterStatManager.currentHealth);
            contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            DamageTarget(damageTarget);
        }
    }

    protected virtual void DamageTarget(CharacterManager damageTarget)
    {
        if (characterDamaged.Contains(damageTarget))
        {
            Debug.Log("3. Skipping: Target already in list!");
            return;
        }
        characterDamaged.Add(damageTarget);
        Debug.Log("ตี");

        TakeDamageEffect damageEffect = Instantiate(WorldPlayerEffectManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.lightningDamage = lightningDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.contactPoint = contactPoint;

        //damageTarget.playerEffectManager.ProcessInstantEffect(damageEffect);//bug
    }

    public virtual void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public virtual void DisableDamageCollider()
    {
        damageCollider.enabled = false;
        characterDamaged.Clear();
        Debug.Log("เครียร์ target");
    }
}
