using UnityEngine;

[CreateAssetMenu(menuName = "Character Effect/Instant Effects/Take Damage")]


public class TakeDamageEffect : InstantPlayerEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager characterCausingDamage;

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

    public override void ProcessEffect(CharacterManager character)
    {
        if (character.characterCurrentState.isInvulnerable)
        {
            return;
        }

        base.ProcessEffect(character);

        if (character.characterCurrentState.isDead)
        {
            return;
        }

        CalculateDamage(character);
        if (character.player != null)
        {
            PlayDirectionalBasedDamageAnimation(character);
        }
        PlayDamageSFX(character);
        PlayDamageVFX(character);

    }

    private void CalculateDamage(CharacterManager character)
    {
        if(characterCausingDamage != null)
        {
            
        }

        finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

        if(finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }

        character.characterStatManager.CurrentHealth -= (int)finalDamageDealt;
    }

    private void PlayDamageVFX(CharacterManager character)
    {
        character.characterEffectManager.PlayBloodSplatterVFX(contactPoint);
    }

    private void PlayDamageSFX(CharacterManager character)
    {
        AudioClip physicalDamageSFX = WorldSFXManager.instance.ChooseRandomSFXFromArray(WorldSFXManager.instance.physicalDamageSFX);

        character.characterSFXManager.PlaySFX(physicalDamageSFX);
    }

    private void PlayDirectionalBasedDamageAnimation(CharacterManager character)
    {
        if (character.characterCurrentState.isDead)
        {
            return;
        }
        

        poiseIsBroken = true;

        if(angleHitFrom >= 145 && angleHitFrom <= 180)//หน้า
        {
            damageAnimation = character.player.playerAnimatorManager.GetRandomAnimationFromList(character.player.playerAnimatorManager.forward_Midium_Damage);
            //player.playerAnimatorManger.PlayerTargetActionAnimation(player.playerAnimatorManger.hit_Forward_Medium_01, true);
        }
        else if(angleHitFrom <= -145 && angleHitFrom >= -180)//หน้า
        {
            damageAnimation = character.player.playerAnimatorManager.GetRandomAnimationFromList(character.player.playerAnimatorManager.forward_Midium_Damage);
            //player.playerAnimatorManger.PlayerTargetActionAnimation(player.playerAnimatorManger.hit_Forward_Medium_01, true);
        }
        else if(angleHitFrom >= -45 && angleHitFrom <= 180)//หลัง
        {
            damageAnimation = character.player.playerAnimatorManager.hit_Backward_Medium_01;
            //player.playerAnimatorManger.PlayerTargetActionAnimation(player.playerAnimatorManger.hit_Backward_Medium_01, true);
        }
        else if(angleHitFrom >= -144 && angleHitFrom <= -45)//ซ้าย
        {
            damageAnimation = character.player.playerAnimatorManager.GetRandomAnimationFromList(character.player.playerAnimatorManager.left_Midium_Damage);
            //player.playerAnimatorManger.PlayerTargetActionAnimation(player.playerAnimatorManger.hit_Left_Medium_01, true);
        }
        else if(angleHitFrom >= 45 && angleHitFrom <= 144)//ขวา
        {
            damageAnimation = character.player.playerAnimatorManager.GetRandomAnimationFromList(character.player.playerAnimatorManager.right_Midium_Damage);
            //player.playerAnimatorManger.PlayerTargetActionAnimation(player.playerAnimatorManger.hit_Right_Medium_01, true);
        }

        if (poiseIsBroken)
        {
            character.player.playerAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
            character.player.playerAnimatorManager.PlayerTargetActionAnimation(damageAnimation, true);
        }
    }
    
}
