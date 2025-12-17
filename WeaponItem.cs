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

    [Header("Stamina Cost Modifiers")]
    public int baseStaminaCost = 5;
    public float lightattackStaminaCostMultiplier = 0.9f;

    [Header("Actions")]
    public WeaponItemAction oh_LC_Action;



}
