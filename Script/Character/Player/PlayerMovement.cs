using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
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
            if (Player.instance.playerStatManager.currentStamina <= 0)
            {
                speed = walkingSpeed;
                return;
            }
            speed = sprintSpeed;
            Player.instance.playerStatManager.currentStamina -= sprintingStaminaCost * Time.deltaTime;
            Player.instance.playerStatManager.ResetStaminaRegenTimer();
        }
        else
        {
            speed = walkingSpeed;
        }
    }
    
    private void GetHorizontalAndVerticalInput()
    {
        horizontalMovement = Player.instance.playerInput.horizontalInput;
        verticalMovement = Player.instance.playerInput.verticalInput;
    }

    private void GroundMovement()
    {
        FallingCheck();
        ApplyGravity();
        if (!Player.instance.playerCurrentState.canMove)
            return;
        Vector3 horizontalMove = CameraPlayer.instance.transform.forward * verticalMovement;
        horizontalMove += CameraPlayer.instance.transform.right * horizontalMovement;
        horizontalMove.Normalize();
        horizontalMove *= speed;

        horizontalMove.y = moveDirection.y;
        if (isRunning)
        {
            Player.instance.playerAnimatorManger.UpdateAnimatorMovement(0, Player.instance.playerInput.moveAmout + 0.5f);
        }
        else
        {
            Player.instance.playerAnimatorManger.UpdateAnimatorMovement(0, Player.instance.playerInput.moveAmout);
        }
        
        Player.instance.characterController.Move(horizontalMove * Time.deltaTime);
    }

    private void RotationMovement()
    {
        if (!Player.instance.playerCurrentState.canRotate)
            return;
        targetRotationDirection = Vector3.zero;
        targetRotationDirection = CameraPlayer.instance.cameraObject.transform.forward * verticalMovement;
        targetRotationDirection += CameraPlayer.instance.cameraObject.transform.right * horizontalMovement;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0;

        if (targetRotationDirection == Vector3.zero)
            targetRotationDirection = transform.forward;

        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);

        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
    }

    public IEnumerator DelayJump()
    {
        if (!Player.instance.playerCurrentState.isJumping)
        {
            Player.instance.playerCurrentState.isJumping = true;
            if (Player.instance.characterController.isGrounded)
            {
                Player.instance.animator.CrossFade("Jump",0.2f);
                Player.instance.playerCurrentState.canMove = false;
                Player.instance.playerCurrentState.canRotate = false;
                yield return new WaitForSeconds(delayJump);
                if (Player.instance.characterController.isGrounded)
                {
                    jumpRequest = true;
                    Player.instance.playerCurrentState.canMove = true;
                    Player.instance.playerCurrentState.canRotate = true;
                }

            }
        }
        yield return new WaitForSeconds(1.4f);
        Player.instance.playerCurrentState.isJumping = false;
    }
    
    private void ApplyGravity()
    {
        if (Player.instance.characterController.isGrounded)
        {
            if (jumpRequest 
            && Time.time - lastJumpTime >= jumpCooldown 
            && !Player.instance.playerCurrentState.isPerformingAction 
            && Player.instance.playerStatManager.currentStamina >= jumpStaminaCost)
            {
                verticalVelocity = Mathf.Sqrt(2 * jumpHeight * gravity);
                lastJumpTime = Time.time;
                
                Player.instance.playerStatManager.currentStamina -= jumpStaminaCost;
                Player.instance.playerStatManager.ResetStaminaRegenTimer();
                
            }
            else
            {
                verticalVelocity = -2f; 
            }
        }
        else
        {
            if (Player.instance.characterController.collisionFlags == CollisionFlags.Above && verticalVelocity > 0)
                verticalVelocity = 0;

            verticalVelocity -= gravity * Time.deltaTime;
            
        }
        jumpRequest = false;
        moveDirection.y = verticalVelocity;
        
    }
    private void FallingCheck()
    {
        bool isGrounded = Player.instance.characterController.isGrounded;
        Player.instance.animator.SetBool("isGround", isGrounded);
        Player.instance.animator.SetFloat("inAirTimer", inAirTimer);

        if (!isGrounded)
        {
            inAirTimer += Time.deltaTime;

            if (inAirTimer >= landingHardTimer)
            {
                Player.instance.animator.CrossFade("Jump_Idle",0.2f);
            }
        }

        // ตกลงพื้น "เฟรมแรก"
        if (isGrounded)
        {
            if (inAirTimer < landingSoftTimer && inAirTimer >= 0.0001f)
            {
                Player.instance.animator.CrossFade("Empty",0.2f);
                inAirTimer = 0;
            }
            
            
            if (inAirTimer > landingSoftTimer && inAirTimer <= landingHardTimer)
            {
                Player.instance.playerAnimatorManger.PlayerTargetActionAnimation("LandingSoft", true);
                inAirTimer = 0;
            }


            if (inAirTimer > landingHardTimer)
            {
                Player.instance.playerAnimatorManger.PlayerTargetActionAnimation("Landing_Hard", true);
                inAirTimer = 0;
            }
        }

        wasGroundedLastFrame = isGrounded;
    }
    

    public void ForceFaceForward()
    {
        Vector3 forward = CameraPlayer.instance.cameraObject.transform.forward;
        forward.y = 0;

        Quaternion targetRot = Quaternion.LookRotation(forward);

        // หมุนแบบค่อย ๆ หัน (ปรับค่า 10f ได้ตามชอบ)
        transform.rotation = Quaternion.Slerp(transform.rotation,targetRot,Time.deltaTime * 10f);
    }

}
