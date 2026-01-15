using UnityEngine;

public class PlayerStatManager : CharacterStatManager
{
    [Header("Stamin Stats")]
    public int endurance = 1;
    public float currentStamina = 100;
    public int baseStamina = 100;

    [Header("Stamina Regeneration")]
    private float staminaRegenerationTimer = 0;
    private float staminaTickTimer = 0;
    [SerializeField] float staminaRegenerationDelay = 2;
    [SerializeField] int staminaRegenerateAmount = 1;

    public float CurrentStamina
    {
        get => currentStamina;
        set
        {
            if (currentStamina == value) return;

            float old = currentStamina;
            currentStamina = value;

            OnCurrentStamina?.Invoke(old, value);
        }
    }

    public event System.Action<float, float> OnCurrentStamina;

    protected override void Awake()
    {
        OnCurrentHealth += OnCurrentHealthChange;
        OnCurrentStamina += OnCurrentStaminaChange;
    }

    protected override void Start()
    {
        float maxHealth = baseHealth * vitality;
        PlayerUIManager.instance.playerUIHUDManager.SetMaxHealthValue((int)maxHealth);

        int maxStamina = baseStamina * endurance;
        PlayerUIManager.instance.playerUIHUDManager.SetMaxStaminaValue(maxStamina);


    }

    private void Update()
    {
        RegenerateStamina();
    }

    protected override void OnCurrentHealthChange(int oldValue, int newValue)
    {
        PlayerUIManager.instance.playerUIHUDManager.SetNewHealthValue(0, currentHealth);

        float maxHealth = baseHealth * vitality;
        PlayerUIManager.instance.playerUIHUDManager.SetMaxHealthValue((int)maxHealth);

        if (currentHealth <= 0)
        {
            if (PlayerManager.instance.playerCurrentState.isDead)
            {
                return;
            }
            StartCoroutine(PlayerManager.instance.ProcessDeathEvent());
        }

        if (currentHealth > baseHealth * vitality)
        {
            CurrentHealth = baseHealth * vitality;
        }
    }

    private void OnCurrentStaminaChange(float oldValue, float newValue)
    {
        PlayerUIManager.instance.playerUIHUDManager.SetNewStaminaValue(0, (int)currentStamina);

        int maxStamina = baseStamina * endurance;
        PlayerUIManager.instance.playerUIHUDManager.SetMaxStaminaValue(maxStamina);

        if (currentStamina > baseStamina * endurance)
        {
            currentStamina = baseStamina * endurance;
        }
    }

    public void RegenerateStamina()
    {
        if (PlayerManager.instance.playerMovement.isRunning)
        {
            return;
        }

        if (PlayerManager.instance.playerCurrentState.isPerformingAction)
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
                    CurrentStamina += staminaRegenerateAmount;

                }
            }
        }
    }

    public void ResetStaminaRegenTimer()
    {
        staminaRegenerationTimer = 0;        
    }

}
