using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
    private Player player;
    [SerializeField ] CameraPlayer playerCamera;

    [SerializeField] float verticalMovement;
    [SerializeField] float horizontalMovement;

    private Vector3 moveDirection;

    [Header("Movement Setting")]
    [SerializeField] float walkingSpeed = 4;
    [SerializeField] float sprintSpeed = 6.5f;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float gravity = 19.81f;
    [SerializeField] float jumpCooldown = 0.5f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float rotationSpeedWhenLockOn = 30f;

    [Header("Stat Setting")]
    [SerializeField] float sprintingStaminaCost = 20f;
    [SerializeField] public float jumpStaminaCost = 2f;
    
    private float speed;
    [HideInInspector] public bool isRunning;
    
    private Vector3 targetRotationDirection;


    [Header("JumpSetting")]
    [SerializeField] private float delayJump = 2f;
    [SerializeField] private float landingSoftTimer = 0.1f;
    [SerializeField] private float landingHardTimer = 1f;

    private float verticalVelocity;
    [HideInInspector] public bool jumpRequest = false;
    private float lastJumpTime = -10f;
    private bool wasGroundedLastFrame = false;

    
    private float inAirTimer = 0;       

    private void Awake()
    {
        speed = walkingSpeed; 
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void HandleMovement()
    {
        GroundMovement();
        GetHorizontalAndVerticalInput();
        RotationMovement();
        GetSprint();
    }

    public void GetSprint()
    {
        if (isRunning)
        {
            if (player.playerStatManager.currentStamina <= 0)
            {
                speed = walkingSpeed;
                return;
            }
            player.playerCurrentState.isSprinting = true;
            speed = sprintSpeed;
            player.playerStatManager.currentStamina -= sprintingStaminaCost * Time.deltaTime;
            player.playerStatManager.ResetStaminaRegenTimer();
        }
        else
        {
            player.playerCurrentState.isSprinting = false;
            speed = walkingSpeed;
        }
    }
    
    private void GetHorizontalAndVerticalInput()
    {
        horizontalMovement = player.playerInput.horizontalInput;
        verticalMovement = player.playerInput.verticalInput;
    }

    private void GroundMovement()
    {
        FallingCheck();
        ApplyGravity();
        if (!player.playerCurrentState.canMove)
            return;
        Vector3 horizontalMove = playerCamera.transform.forward * verticalMovement;
        horizontalMove += playerCamera.transform.right * horizontalMovement;
        horizontalMove.Normalize();
        horizontalMove *= speed;

        var playerInput = player.playerInput;

        horizontalMove.y = moveDirection.y;
        if (!player.playerCurrentState.isLockTarget)
        {
            if (isRunning)
            {
                player.playerAnimatorManager.UpdateAnimatorMovement(0, playerInput.moveAmount + 1);
            }
            else
            {
                player.playerAnimatorManager.UpdateAnimatorMovement(0, playerInput.moveAmount);
            }
        }
        else if(player.playerCurrentState.isLockTarget)
        {
            if (isRunning)
            {
                player.playerAnimatorManager.UpdateAnimatorMovement(0, playerInput.moveAmount + 1);
            }
            else
            {
                player.playerAnimatorManager.UpdateAnimatorMovement(playerInput.horizontalInput, playerInput.verticalInput);
            }
        }
               
        
        
        player.characterController.Move(horizontalMove * Time.deltaTime);
    }

    private void RotationMovement()
    {
        if(player.playerCurrentState.isDead)
            return;

        if (!player.playerCurrentState.canRotate)
            return;

        if (player.playerCurrentState.isLockTarget)
        {
            if (player.playerCurrentState.isSprinting || player.playerCurrentState.isRolling)
            {
                Vector3 targetDirection = Vector3.zero;
                targetDirection = playerCamera.cameraObject.transform.forward * verticalMovement;
                targetDirection += playerCamera.cameraObject.transform.right * horizontalMovement;
                targetDirection.Normalize();
                targetDirection.y = 0;
                
                if(targetDirection == Vector3.zero)
                {
                    targetDirection = transform.forward;
                }

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = finalRotation;
            }
            else
            {
                if(player.playerCombatManager.lockedOnEnemy == null)
                {
                    return;
                }

                Vector3 targetDirection;
                targetDirection = player.playerCombatManager.lockedOnEnemy.transform.position - transform.position;
                targetDirection.y = 0;
                targetDirection.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeedWhenLockOn * Time.deltaTime);
                transform.rotation = finalRotation;
            }
        }
        
        else
        {
            targetRotationDirection = Vector3.zero;
            targetRotationDirection = playerCamera.cameraObject.transform.forward * verticalMovement;
            targetRotationDirection += playerCamera.cameraObject.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0;

            if (targetRotationDirection == Vector3.zero)
                targetRotationDirection = transform.forward;

            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
 
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        }
        
    }

    public IEnumerator DelayJump()
    {
        if (!player.playerCurrentState.isJumping)
        {
            player.playerCurrentState.isJumping = true;
            if (player.characterController.isGrounded)
            {
                player.animator.CrossFade("Jump",0.2f);
                player.playerCurrentState.canMove = false;
                player.playerCurrentState.canRotate = false;
                yield return new WaitForSeconds(delayJump);
                if (player.characterController.isGrounded)
                {
                    jumpRequest = true;
                    player.playerCurrentState.canMove = true;
                    player.playerCurrentState.canRotate = true;
                }

            }
        }
        yield return new WaitForSeconds(1.4f);
        player.playerCurrentState.isJumping = false;
    }
    
    private void ApplyGravity()
    {
        if (player.characterController.isGrounded)
        {
            if (jumpRequest 
            && Time.time - lastJumpTime >= jumpCooldown 
            && !player.playerCurrentState.isPerformingAction 
            && player.playerStatManager.currentStamina >= jumpStaminaCost)
            {
                verticalVelocity = Mathf.Sqrt(2 * jumpHeight * gravity);
                lastJumpTime = Time.time;
                
                player.playerStatManager.currentStamina -= jumpStaminaCost;
                player.playerStatManager.ResetStaminaRegenTimer();
                
            }
            else
            {
                verticalVelocity = -2f; 
            }
        }
        else
        {
            if (player.characterController.collisionFlags == CollisionFlags.Above && verticalVelocity > 0)
                verticalVelocity = 0;

            verticalVelocity -= gravity * Time.deltaTime;
            
        }
        jumpRequest = false;
        moveDirection.y = verticalVelocity;
        
    }
    private void FallingCheck()
    {
        bool isGrounded = player.characterController.isGrounded;
        player.animator.SetBool("isGround", isGrounded);
        player.animator.SetFloat("inAirTimer", inAirTimer);

        if (!isGrounded)
        {
            inAirTimer += Time.deltaTime;

            if (inAirTimer >= landingHardTimer)
            {
                player.animator.CrossFade("Jump_Idle",0.2f);
            }
        }

        // ตกลงพื้น "เฟรมแรก"
        if (isGrounded)
        {
            if (inAirTimer < landingSoftTimer && inAirTimer >= 0.0001f)
            {
                player.animator.CrossFade("Empty",0.2f);
                inAirTimer = 0;
            }
            
            
            if (inAirTimer > landingSoftTimer && inAirTimer <= landingHardTimer)
            {
                player.playerAnimatorManager.PlayerTargetActionAnimation("LandingSoft", true);
                inAirTimer = 0;
            }


            if (inAirTimer > landingHardTimer)
            {
                player.playerAnimatorManager.PlayerTargetActionAnimation("Landing_Hard", true);
                inAirTimer = 0;
            }
        }

        wasGroundedLastFrame = isGrounded;
    }
    

    public void ForceFaceForward()
    {
        Vector3 forward = playerCamera.cameraObject.transform.forward;
        forward.y = 0;

        Quaternion targetRot = Quaternion.LookRotation(forward);

        transform.rotation = Quaternion.Slerp(transform.rotation,targetRot,Time.deltaTime * 10f);
    }

}
