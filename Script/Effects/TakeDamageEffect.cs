using UnityEngine;

[CreateAssetMenu(menuName = "Character Effect/Instant Effects/Take Damage")]


public class TakeDamageEffect : InstantPlayerEffect
{
    [Header("Character Causing Damage")]
    public Player playerCausingDamage;

    [Header("Damage")]
    public float physicalDamage = 0;
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;
    //public float 

    [Header("Final Damage")]
    private float finalDamageDealt = 0;

    [Header("Animation")]
    public bool playDamageAnimation = true;
    public bool manuallySelectDamageAnimation = false;
    public string damageAnimation;

    [Header("Poise")]
    public float poiseDamage = 0;
    public bool poiseIsBroken = false;

    [Header("Sound FX")]
    public bool willPlayDamagedSFX = true;
    public AudioClip elementalDamageSoundFX;

    [Header("Direct Damage Taken From")]
    public float angleHitFrom;
    public Vector3 contactPoint; 

    public override void ProcessEffect(Player player)
    {
        base.ProcessEffect(player);

        if (Player.instance.playerCurrentState.isDead)
        {
            return;
        }

        CalculateDamage(player);

    }

    private void CalculateDamage(Player player)
    {
        if(playerCausingDamage != null)
        {
            
        }

        finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

        if(finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }

        Player.instance.playerStatManager.currentHealth -= finalDamageDealt;


    }
    
}
