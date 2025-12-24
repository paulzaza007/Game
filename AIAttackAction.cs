using UnityEngine;

[CreateAssetMenu(menuName = "A.I/Actions/Attack")]

public class AIAttackAction : ScriptableObject
{
    [Header("Attack")]
    [SerializeField] private string attackAnimation;

    [Header("Combo Action")]
    public bool actionHasComboAction = false;
    public AIAttackAction comboAction;

    [Header("Action Values")]
    public int AttackWeight = 50;
    [SerializeField] AttackType attackType;
    public float actionRecoveryTime = 1.5f;
    public float minimumAttackAngle = -35f;
    public float maximumAttackAngle = 35f;
    public float minimumDistance = 0;
    public float maximumDistance = 2;

    public void AttemptToPerformAction(AICharacterManager aICharacter)
    {
        aICharacter.aIAnimationManager.PlayerTargetAttackActionAnimation(attackType, attackAnimation, true);
    }
}
