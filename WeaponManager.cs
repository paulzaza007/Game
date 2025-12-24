using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public MeleeWeaponDamageCollider meleeWeaponDamageCollider;

    private void Awake()
    {
        meleeWeaponDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
    }

    public void SetWeaponDamage(CharacterManager characterWieldingWeapon, WeaponItem weaponItem)
    {
        meleeWeaponDamageCollider.characterCausingDamage = characterWieldingWeapon;
        meleeWeaponDamageCollider.physicalDamage = weaponItem.physicalDamage;
        meleeWeaponDamageCollider.magicDamage = weaponItem.magicDamage;
        meleeWeaponDamageCollider.fireDamage = weaponItem.fireDamage;
        meleeWeaponDamageCollider.lightningDamage = weaponItem.lightningDamage;
        meleeWeaponDamageCollider.holyDamage = weaponItem.holyDamage;

        meleeWeaponDamageCollider.light_Attack_01_Modifier = weaponItem.light_Attack_01_Modifier;
        meleeWeaponDamageCollider.light_Attack_02_Modifier = weaponItem.light_Attack_02_Modifier;
        meleeWeaponDamageCollider.light_Attack_03_Modifier = weaponItem.light_Attack_03_Modifier;
        meleeWeaponDamageCollider.heavy_Attack_01_Modifier = weaponItem.heavy_Attack_01_Modifier;
        meleeWeaponDamageCollider.charge_Attack_01_Modifier = weaponItem.charge_Attack_01_Modifier;
    }
}
