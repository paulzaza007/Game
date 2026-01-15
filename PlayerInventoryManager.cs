using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{
    public WeaponItem currentRightHandWeapon;

    [Header("Quick Slots")]
    public WeaponItem[] weaponInRighthandSlots = new WeaponItem[3];
    public int rightHandWeaponIndex = 0;
}
