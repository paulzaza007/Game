using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour
{
    public WeaponLocation rightHandSlot;
    public WeaponLocation leftHandSlot;

    public GameObject rightHandWeaponModel;
    public GameObject leftHandWeaponModel;

    private void Awake()
    {
        InitializeWeaponSlot();
    }

    private void Start()
    {
        LoadWeaponOnBothHands();
    }

    private void InitializeWeaponSlot()
    {
        WeaponLocation[] weaponSlots = GetComponentsInChildren<WeaponLocation>();

        foreach(var weaponSlot in weaponSlots)
        {
            if(weaponSlot.weaponSlot == WeaponModelSlot.RightHand)
            {
                rightHandSlot = weaponSlot;
            }
            else if(weaponSlot.weaponSlot == WeaponModelSlot.LeftHand)
            {
                leftHandSlot = weaponSlot;
            }
        }
    }

    public void LoadWeaponOnBothHands()
    {
        LoadLeftWeapon();
        LoadRightWeapon();
    }

    private void LoadRightWeapon()
    {
        if (Player.instance.playerInventoryManager.currentRightHandWeapon != null)
        {
            rightHandWeaponModel = Instantiate(Player.instance.playerInventoryManager.currentRightHandWeapon.weaponModel);
            rightHandSlot.LoadWeapon(rightHandWeaponModel);
        }
    }

    private void LoadLeftWeapon()
    {
        if (Player.instance.playerInventoryManager.currentLeftHandWeapon != null)
        {
            leftHandWeaponModel = Instantiate(Player.instance.playerInventoryManager.currentLeftHandWeapon.weaponModel);
            leftHandSlot.LoadWeapon(leftHandWeaponModel);
        }
    }
}
