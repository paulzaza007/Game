using UnityEngine;

public class PlayerStatManager : MonoBehaviour
{
    [Header("Stamin Stats")]
    public float endurance = 1f;
    public float currentStamina = 100f;
    public float baseStamina = 100f;

    [Header("Stamina Regeneration")]
    private float staminaRegenerationTimer = 0;
    private float staminaTickTimer = 0;
    [SerializeField] float staminaRegenerationDelay = 2;
    [SerializeField] float staminaRegenerateAmount = 0.5f;

    [Header("Health Stats")]
    public float vitality = 1;
    public float currentHealth = 100;
    public float baseHealth = 100;

    private void Start()
    {
        RefreshStats();
    }

    private void Update()
    {
        passCurrentStaminaValue();
        passCurrentHealthValue();
        RefreshStats();
        CheckHP();
    }

    public void RefreshStats()
    {
        calculateStaminaBasedOnEnduranceLevel();
        calculateHealthBasedOnVitalityLevel();
    }
    
    public void calculateStaminaBasedOnEnduranceLevel()
    {
        float maxStamina = baseStamina * endurance;

        PlayerUIManager.instance.playerUIHUDManager.SetMaxStaminaValue(maxStamina);
    }

    public void passCurrentStaminaValue()
    {
        PlayerUIManager.instance.playerUIHUDManager.SetNewStaminaValue(0,currentStamina);
    }

    public void passCurrentHealthValue()
    {
        PlayerUIManager.instance.playerUIHUDManager.SetNewHealthValue(0,currentHealth);
    }

    public void RegenerateStamina()
    {
        if (Player.instance.playerMovement.isRunning)
        {
            return;
        }

        if (Player.instance.playerCurrentState.isPerformingAction)
        {
            return;
        }

        staminaRegenerationTimer += Time.deltaTime;

        if (staminaRegenerationTimer >= staminaRegenerationDelay)
        {
            if (currentStamina < baseStamina * endurance)
            {
                staminaTickTimer += Time.deltaTime;

                if (staminaTickTimer >= 0.1)
                {
                    staminaTickTimer = 0;
                    currentStamina += staminaRegenerateAmount;

                }
            }
        }
    }

    public void ResetStaminaRegenTimer()
    {
        staminaRegenerationTimer = 0;        
    }

    public void calculateHealthBasedOnVitalityLevel()
    {
        float maxHealth = baseHealth * vitality;

        PlayerUIManager.instance.playerUIHUDManager.SetMaxHealthValue(maxHealth);
    }

    public void CheckHP()
    {
        if(currentHealth <= 0)
        {
            if (Player.instance.playerCurrentState.isDead)
            {
                return;
            }
            StartCoroutine(Player.instance.ProcessDeathEvent());
        }

        if(currentHealth > baseHealth * vitality)
        {
            currentHealth = baseHealth * endurance;
        }
    }

    public void CheckStamina()
    {
        if (currentStamina > baseStamina * endurance)
        {
            currentStamina = baseStamina * endurance;
        }
    }
}
