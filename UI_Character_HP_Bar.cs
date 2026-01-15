using UnityEngine;
using UnityEngine.TextCore.Text;
using TMPro;

public class UI_Character_HP_Bar : UI_StatBar
{
    private CharacterManager character;
    private AICharacterManager aICharacter;

    [SerializeField] bool displayCharacterNameOnDamage = false;
    [SerializeField] float defaultTimeBeforeBarHide = 3;
    [SerializeField] float hideTimer = 0;
    [SerializeField] int currentDamageTaken = 0;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI characterDamage;
    [HideInInspector] public int oldHealthValue = 0;

    protected override void Awake()
    {
        base.Awake();

        character = GetComponentInParent<CharacterManager>();

        if(character != null)
        {
            aICharacter = character as AICharacterManager;
        }
    }

    protected override void Start()
    {
        base.Start();

        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward);

        if(hideTimer > 0) 
        {
            hideTimer -= Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        currentDamageTaken = 0;
    }

    public override void SetStat(int newValue)
    {
        if (displayCharacterNameOnDamage)
        {
            characterName.enabled = true;

            if(aICharacter != null)
            {
                characterName.text = aICharacter.characterName;
            }
        }

        slider.maxValue = character.characterStatManager.maxHealth;
        
        currentDamageTaken = Mathf.RoundToInt(currentDamageTaken + (oldHealthValue - newValue));

        if(currentDamageTaken < 0) //เลือดเพิ่ม
        {
            currentDamageTaken = Mathf.Abs(currentDamageTaken);
            characterDamage.text = "+ " + currentDamageTaken.ToString();
        }
        else //เลือดลด
        {
            characterDamage.text = "- " + currentDamageTaken.ToString();
        }

        slider.value = newValue;

        if(character.characterStatManager.currentHealth != character.characterStatManager.maxHealth)
        {
            hideTimer = defaultTimeBeforeBarHide;
            gameObject.SetActive(true);
        }
    }
}
