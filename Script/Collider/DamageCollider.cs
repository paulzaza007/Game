using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Header("Collider")]
    protected Collider damageCollider;

    [Header("Damage")]
    public float physicalDamage = 0;
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;

    [Header("Contact Point")]
    private Vector3 contactPoint;

    [Header("Player Damaged")]
    protected List<Player> playerDamaged = new List<Player>();
 
    private void OnTriggerEnter(Collider other)
    {
        //if(other.gameObject.layer = LayerMask.NameToLayer("Player"))
        Player damageTarget = other.GetComponent<Player>();

        if(damageTarget != null)
        {
            contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            DamageTarget(damageTarget);
        }
    }

    protected virtual void DamageTarget(Player damageTarget)
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

        Player.instance.playerEffectManager.ProcessInstantEffect(damageEffect);
    }

    public virtual void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public virtual void DisableDamageCollider()
    {
        damageCollider.enabled = false;
        playerDamaged.Clear();
    }
}
