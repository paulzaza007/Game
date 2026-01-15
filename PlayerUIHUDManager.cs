using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHUDManager : MonoBehaviour
{
    [Header("STAT BAR")]
    [SerializeField] UI_StatBar healthBar;
    [SerializeField] UI_StatBar staminaBar;

    [Header("QUICK SLOT")]
    [SerializeField] Image rightWeaponQuickSlotIcon;
    [SerializeField] Image leftWeaponQuickSlotIcon;

    [Header("BOSS HEALTH BAR")]
    public GameObject bossHealthBarObject;  
    public Transform bossHealthBarParent;

    public void RefeshHUD()
    {
        healthBar.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(true);
        staminaBar.gameObject.SetActive(false);
        staminaBar.gameObject.SetActive(true);
    }
    
    public void SetNewHealthValue(int oldValue, int newValue)
    {
        healthBar.SetStat(newValue);
    }

    public void SetMaxHealthValue(int maxHealth)
    {
        healthBar.SetMaxStat((float)maxHealth);
        
    }
    
    public void SetNewStaminaValue(int oldValue, int newValue)
    {
        staminaBar.SetStat(newValue);
        //Debug.Log(newValue);
    }

    public void SetMaxStaminaValue(int maxStamina)
    {
        staminaBar.SetMaxStat((float)maxStamina);
        
    }

    public void SetRightWeaponQuickSlotIcon(int weaponID)
    {
        WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);

        if(weapon == null)
        {
            Debug.Log("ไม่มี Item");
            rightWeaponQuickSlotIcon.enabled = false;
            rightWeaponQuickSlotIcon.sprite = null;
            return;
        }

        if(weapon.itemIcon == null)
        {
            Debug.Log("ไม่มี Icon");
            rightWeaponQuickSlotIcon.enabled = false;
            rightWeaponQuickSlotIcon.sprite = null;
            return;
        }

        rightWeaponQuickSlotIcon.enabled = true;
        rightWeaponQuickSlotIcon.sprite = weapon.itemIcon;
    }

    public void SetLeftWeaponQuickSlotIcon(int weaponID)
    {
        WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);

        if (weapon == null)
        {
            Debug.Log("ไม่มี Item");
            leftWeaponQuickSlotIcon.enabled = false;
            leftWeaponQuickSlotIcon.sprite = null;
            return;
        }

        if (weapon.itemIcon == null)
        {
            Debug.Log("ไม่มี Icon");
            leftWeaponQuickSlotIcon.enabled = false;
            leftWeaponQuickSlotIcon.sprite = null;
            return;
        }

        leftWeaponQuickSlotIcon.enabled = true;
        leftWeaponQuickSlotIcon.sprite = weapon.itemIcon;
    }
}
