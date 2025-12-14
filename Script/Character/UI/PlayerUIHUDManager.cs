using UnityEngine;

public class PlayerUIHUDManager : MonoBehaviour
{
    [SerializeField] UI_StatBar healthBar;
    [SerializeField] UI_StatBar staminaBar;

    public void RefeshHUD()
    {
        healthBar.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(true);
        staminaBar.gameObject.SetActive(false);
        staminaBar.gameObject.SetActive(true);
    }
    
    public void SetNewHealthValue(float oldValue, float newValue)
    {
        healthBar.SetStat(newValue);
    }

    public void SetMaxHealthValue(float maxHealth)
    {
        healthBar.SetMaxStat(maxHealth);
        
    }
    
    public void SetNewStaminaValue(float oldValue, float newValue)
    {
        staminaBar.SetStat(newValue);
        //Debug.Log(newValue);
    }

    public void SetMaxStaminaValue(float maxStamina)
    {
        staminaBar.SetMaxStat(maxStamina);
        
    }
}
