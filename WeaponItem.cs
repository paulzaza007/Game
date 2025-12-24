using UnityEngine;

public class WeaponItem : Item
{
    [Header("Weapon Model")]
    public GameObject weaponModel;

    [Header("Weapon Requirements")]
    public int strengthREQ = 0;
    public int dexREQ = 0;
    public int intREQ = 0;
    public int faithREQ = 0;

    [Header("Weapon Base Damage")]
    public int physicalDamage = 0;
    public int magicDamage = 0;
    public int fireDamage = 0;
    public int holyDamage = 0;
    public int lightningDamage = 0;

    [Header("Weapon Base Poise Damage")]
    public int poiseDamage = 10;

    [Header("Attack Modifiers")]
    public float light_Attack_01_Modifier = 1.1f;
    public float light_Attack_02_Modifier = 0.9f;
    public float light_Attack_03_Modifier = 1.3f;
    public float heavy_Attack_01_Modifier = 1.5f;
    public float charge_Attack_01_Modifier = 2f;

    [Header("Stamina Cost Modifiers")]
    public int baseStaminaCost = 5;
    public float lightattackStaminaCostMultiplier = 0.9f;

    [Header("Actions")]
    public WeaponItemAction oh_LC_Action;
    public WeaponItemAction oh_RC_Action;
 
    [Header("Whoosh")]
    public AudioClip[] whoosh;

}
