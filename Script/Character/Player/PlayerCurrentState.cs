using UnityEngine;

public class PlayerCurrentState : MonoBehaviour
{
    [Header("Flags")]
    public bool isPerformingAction = false;
    public bool canRotate = true;
    public bool canMove = true;
    public bool applyRootMotion = false;
    public bool isJumping = false;
    public bool isGrounded = true;
    public bool isDead = false;

    
}

