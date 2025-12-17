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

        if (player.playerCurrentState.isDead)
        {
            return;
        }

        CalculateDamage(player);
        PlayDirectionalBasedDamageAnimation(player);
        PlayDamageSFX(player);
        PlayDamageVFX(player);

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

        player.playerStatManager.currentHealth -= finalDamageDealt;


    }

    private void PlayDamageVFX(Player player)
    {
        player.playerEffectManager.PlayBloodSplatterVFX(contactPoint);
    }

    private void PlayDamageSFX(Player player)
    {
        AudioClip physicalDamageSFX = WorldSFXManager.instance.ChooseRandomSFXFromArray(WorldSFXManager.instance.physicalDamageSFX);

        player.playerSFXManager.PlaySFX(physicalDamageSFX);
    }

    private void PlayDirectionalBasedDamageAnimation(Player player)
    {
        if (player.playerCurrentState.isDead)
        {
            return;
        }
        
        poiseIsBroken = true;

        if(angleHitFrom >= 145 && angleHitFrom <= 180)//หน้า
        {
            damageAnimation = player.playerAnimatorManger.GetRandomAnimationFromList(player.playerAnimatorManger.forward_Midium_Damage);
            //player.playerAnimatorManger.PlayerTargetActionAnimation(player.playerAnimatorManger.hit_Forward_Medium_01, true);
        }
        else if(angleHitFrom <= -145 && angleHitFrom >= -180)//หน้า
        {
            damageAnimation = player.playerAnimatorManger.GetRandomAnimationFromList(player.playerAnimatorManger.forward_Midium_Damage);
            //player.playerAnimatorManger.PlayerTargetActionAnimation(player.playerAnimatorManger.hit_Forward_Medium_01, true);
        }
        else if(angleHitFrom >= -45 && angleHitFrom <= 180)//หลัง
        {
            damageAnimation = player.playerAnimatorManger.hit_Backward_Medium_01;
            //player.playerAnimatorManger.PlayerTargetActionAnimation(player.playerAnimatorManger.hit_Backward_Medium_01, true);
        }
        else if(angleHitFrom >= -144 && angleHitFrom <= -45)//ซ้าย
        {
            damageAnimation = player.playerAnimatorManger.GetRandomAnimationFromList(player.playerAnimatorManger.left_Midium_Damage);
            //player.playerAnimatorManger.PlayerTargetActionAnimation(player.playerAnimatorManger.hit_Left_Medium_01, true);
        }
        else if(angleHitFrom >= 45 && angleHitFrom <= 144)//ขวา
        {
            damageAnimation = player.playerAnimatorManger.GetRandomAnimationFromList(player.playerAnimatorManger.right_Midium_Damage);
            //player.playerAnimatorManger.PlayerTargetActionAnimation(player.playerAnimatorManger.hit_Right_Medium_01, true);
        }

        if (poiseIsBroken)
        {
            player.playerAnimatorManger.lastDamageAnimationPlayed = damageAnimation;
            player.playerAnimatorManger.PlayerTargetActionAnimation(damageAnimation, true);
        }
    }
    
}
