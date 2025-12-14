using UnityEngine;

public class ResetAnimation : MonoBehaviour
{
    [SerializeField] bool reset = false;

    private void Update()
    {
        ResetState();
    }

    private void ResetState()
    {
        if (reset)
        {
            reset = false;
            Player.instance.animator.CrossFade("Empty", 0.1f);

            Player.instance.playerCurrentState.isPerformingAction = false;
            Player.instance.playerCurrentState.canRotate = true;
            Player.instance.playerCurrentState.canMove = true;
            Player.instance.playerCurrentState.applyRootMotion = false;
            Player.instance.playerCurrentState.isDead = false;
            Player.instance.playerCurrentState.isJumping = false;

            Player.instance.playerStatManager.currentHealth = Player.instance.playerStatManager.baseHealth * Player.instance.playerStatManager.endurance;
        }
    }
}
