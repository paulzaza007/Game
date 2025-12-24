using UnityEngine;

public class WeaponLocation : MonoBehaviour
{
    public WeaponModelSlot weaponSlot;
    public GameObject currentWeaponModel;

    public void UnloadWeapon()
    {
        if(currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }
    }

    public GameObject LoadWeapon(GameObject weaponModel)
    {
        UnloadWeapon();

        if (weaponModel == null)
        {
            return null;
        }
        currentWeaponModel = Instantiate(weaponModel, transform);

        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localRotation = Quaternion.identity;
        weaponModel.transform.localScale = Vector3.one;

        return currentWeaponModel;
    }
}
