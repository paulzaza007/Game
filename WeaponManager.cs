using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public MeleeWeaponDamageCollider meleeWeaponDamageCollider;

    private void Awake()
    {
        meleeWeaponDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
    }

    public void SetWeaponDamage(Player characterWieldingWeapon, WeaponItem weaponItem)
    {
        meleeWeaponDamageCollider.playerCausingDamage = characterWieldingWeapon;
        meleeWeaponDamageCollider.physicalDamage = weaponItem.physicalDamage;
        meleeWeaponDamageCollider.magicDamage = weaponItem.magicDamage;
        meleeWeaponDamageCollider.fireDamage = weaponItem.fireDamage;
        meleeWeaponDamageCollider.lightningDamage = weaponItem.lightningDamage;
        meleeWeaponDamageCollider.holyDamage = weaponItem.holyDamage;

        meleeWeaponDamageCollider.light_Attack_01_Modifier = weaponItem.light_Attack_01_Modifier;
    }
}
