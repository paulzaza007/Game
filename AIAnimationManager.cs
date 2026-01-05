using UnityEngine;

public class AIAnimationManager : MonoBehaviour
{
    private AICharacterManager aICharacter;

    protected virtual void Awake()
    {
        aICharacter = GetComponent<AICharacterManager>();
    }

    public void PlayerTargetActionAnimation(
        string targetAnimation,
        bool performingActionFlag,
        bool applyRootMotion = true,
        bool canRotate = false,
        bool canMove = false)
    {
        aICharacter.aICurrentState.isPerformingAction = performingActionFlag;
        aICharacter.aICurrentState.applyRootMotion = applyRootMotion;
        aICharacter.aICurrentState.canRotate = canRotate;
        aICharacter.aICurrentState.canMove = canMove;

        //เปิด Root Motion ใน Animator
        aICharacter.animator.applyRootMotion = applyRootMotion;

        aICharacter.animator.CrossFade(targetAnimation, 0.2f);
    }

    protected virtual void OnAnimatorMove()
    {
        if (aICharacter.navMeshAgent == null || aICharacter.characterController == null) return;

        Vector3 animationVelocity = aICharacter.animator.deltaPosition;

        // แทนที่จะ return เฉยๆ ให้ใส่แรงโน้มถ่วงอ่อนๆ ไว้เสมอ
        // เพื่อให้ Character Controller ทำงาน (Stay Active)
        float verticalVelocity = 0;
        if (aICharacter.characterController.isGrounded)
        {
            verticalVelocity = -0.05f;
        }
        else
        {
            verticalVelocity -= aICharacter.gravity * Time.deltaTime;
        }
        Vector3 finalVelocity = animationVelocity;
        finalVelocity.y += verticalVelocity;

        aICharacter.characterController.Move(finalVelocity);
        aICharacter.transform.rotation *= aICharacter.animator.deltaRotation;
        aICharacter.navMeshAgent.nextPosition = aICharacter.transform.position;
    }

    public void PlayerTargetAttackActionAnimation(
        AttackType attackType,
        string targetAnimation,
        bool performingActionFlag,
        bool applyRootMotion = true,
        bool canRotate = false,
        bool canMove = false)
    {
        //Player.instance.playerCombatManager.currentAttackType = attackType;
        //Player.instance.playerCombatManager.lastAttackAnimationPerform = targetAnimation;
        aICharacter.aICurrentState.isPerformingAction = performingActionFlag;
        aICharacter.aICurrentState.applyRootMotion = applyRootMotion;
        aICharacter.aICurrentState.canRotate = canRotate;
        aICharacter.aICurrentState.canMove = canMove;

        //เปิด Root Motion ใน Animator
        aICharacter.animator.applyRootMotion = applyRootMotion;

        aICharacter.animator.CrossFade(targetAnimation, 0.2f);
    }
}
