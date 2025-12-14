using UnityEngine;

public class PlayerAnimatorManger : MonoBehaviour
{

    protected virtual void Awake()
    {
        
    }

    public void UpdateAnimatorMovement(float horizontalValue, float verticalValue)
    {
        Player.instance.animator.SetFloat("Horizontal", horizontalValue, 0.1f, Time.deltaTime);
        Player.instance.animator.SetFloat("Vertical", verticalValue, 0.1f, Time.deltaTime);
    }

    public void PlayerTargetActionAnimation(
        string targetAnimation,
        bool performingActionFlag,
        bool applyRootMotion = true,
        bool canRotate = false,
        bool canMove = false)
    {
        Player.instance.playerCurrentState.isPerformingAction = performingActionFlag;
        Player.instance.playerCurrentState.applyRootMotion = applyRootMotion;
        Player.instance.playerCurrentState.canRotate = canRotate;
        Player.instance.playerCurrentState.canMove = canMove;

        //เปิด Root Motion ใน Animator
        Player.instance.animator.applyRootMotion = applyRootMotion;

        Player.instance.animator.CrossFade(targetAnimation, 0.2f);
    }
}
