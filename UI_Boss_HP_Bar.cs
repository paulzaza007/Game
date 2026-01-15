using UnityEngine;
using TMPro;

public class UI_Boss_HP_Bar : UI_StatBar
{
    [SerializeField] AIBossManager bossCharacter;

    public void EnableBossHPBar(AIBossManager boss)
    {
        bossCharacter = boss;
        bossCharacter.aIStatManager.OnCurrentHealth += OnBossHPChange;
        SetMaxStat(bossCharacter.aIStatManager.maxHealth);
        SetStat(bossCharacter.aIStatManager.CurrentHealth);
        GetComponentInChildren<TextMeshProUGUI>().text = bossCharacter.characterName;
    }

    private void OnDestroy()
    {
        bossCharacter.aIStatManager.OnCurrentHealth -= OnBossHPChange;
    }

    private void OnBossHPChange(int oldInt, int newInt)
    {
        Debug.Log($"UI: รับค่าเลือดใหม่ = {newInt}");
        SetStat(newInt);

        if(newInt <= 0)
        {
            RemoveHPBar(2.5f);
        }
    }

    public void RemoveHPBar(float time)
    {
        Destroy(gameObject, time);
    }
}
