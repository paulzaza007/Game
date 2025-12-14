using UnityEngine;

[CreateAssetMenu(menuName = "Character Effect/Instant Effects/Take Stamina Damage")]

public class TakeStaminaEffect : InstantPlayerEffect
{
    public float staminaDamage;

    public override void ProcessEffect(Player player)
    {
        CalculateStaminaDamage(player);
    }

    private void CalculateStaminaDamage(Player player)
    {
        Player.instance.playerStatManager.currentStamina -= staminaDamage;
    }
}
