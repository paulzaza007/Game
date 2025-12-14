using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] MeleeWeaponDamageCollider meleeWeaponDamageCollider;

    private void Awake()
    {
        meleeWeaponDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
    }

    public void SetWeaponDamage(WeaponItem weaponItem)
    {
        meleeWeaponDamageCollider.physicalDamage = weaponItem.physicalDamage;
        meleeWeaponDamageCollider.magicDamage = weaponItem.magicDamage;
        meleeWeaponDamageCollider.fireDamage = weaponItem.fireDamage;
        meleeWeaponDamageCollider.lightningDamage = weaponItem.lightningDamage;
        meleeWeaponDamageCollider.holyDamage = weaponItem.holyDamage;
    }
}
