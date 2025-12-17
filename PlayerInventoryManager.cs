using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{
    public WeaponItem currentRightHandWeapon;
    public WeaponItem currentLeftHandWeapon;

    [Header("Quick Slots")]
    public WeaponItem[] weaponInRighthandSlots = new WeaponItem[3];
    public int rightHandWeaponIndex = 0;
    public WeaponItem[] weaponInLefthandSlots = new WeaponItem[3];
    public int leftHandWeaponIndex = 1;

}
