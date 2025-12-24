using UnityEngine;

[CreateAssetMenu(menuName = "Character Effect/Instant Effects/Take Stamina Damage")]

public class TakeStaminaEffect : InstantPlayerEffect
{
    public float staminaDamage;

    public override void ProcessEffect(CharacterManager character)
    {
        CalculateStaminaDamage(character);
    }

    private void CalculateStaminaDamage(CharacterManager character)
    {
        character.playerStatManager.currentStamina -= staminaDamage;
    }
}
