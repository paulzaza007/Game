using UnityEngine;

public class CharacterCurrentState : MonoBehaviour
{
    [Header("Flags")]
    public bool isPerformingAction = false;
    public bool canRotate = true;
    public bool canMove = true;
    public bool applyRootMotion = false;
    public bool isJumping = false;
    public bool isGrounded = true;
    public bool isDead = false;
    public bool isLockTarget = false;
    public bool isSprinting = false;
    public bool isRolling = false;
    public bool isInvulnerable = false;
}
