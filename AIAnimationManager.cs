using UnityEngine;

public class AIAnimationManager : MonoBehaviour
{
    private AICharacterManager aICharacter;

    private void Awake()
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

    private void OnAnimatorMove()
    {
        if (aICharacter.navMeshAgent == null || aICharacter.characterController == null) return;

        Vector3 velocity = aICharacter.animator.deltaPosition;

        // แทนที่จะ return เฉยๆ ให้ใส่แรงโน้มถ่วงอ่อนๆ ไว้เสมอ
        // เพื่อให้ Character Controller ทำงาน (Stay Active)
        if (!aICharacter.IsMoving && aICharacter.characterController.isGrounded)
        {
            // แรงกดพื้นเล็กน้อยเพื่อให้ระบบฟิสิกส์ยังตรวจสอบการชนอยู่
            return;
        }
        else
        {
            velocity.y -= aICharacter.gravity * Time.deltaTime;
        }

        aICharacter.characterController.Move(velocity);
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
