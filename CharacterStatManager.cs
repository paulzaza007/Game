using UnityEngine;

public class CharacterStatManager : MonoBehaviour
{
    protected CharacterManager character;

    [Header("Health Stats")]
    public int vitality = 1;
    public int currentHealth = 100;
    public int baseHealth = 100;
    public int maxHealth;
    

    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            if (currentHealth == value) return;

            int old = currentHealth;
            currentHealth = value;

            OnCurrentHealth?.Invoke(old, value);
        }
    }

    public event System.Action<int, int> OnCurrentHealth;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Start()
    {
        maxHealth = baseHealth * vitality;
        currentHealth = maxHealth;
    }

    protected virtual void OnCurrentHealthChange(int oldValue, int newValue)
    {
        
    }


}
