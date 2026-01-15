using UnityEngine;
using System.Collections;

public class PlayerDodge : MonoBehaviour
{
    private PlayerManager player;

    [Header("PLAYER ACTIONS INPUT")]
    [HideInInspector] public bool dodgeInput = false;
    
    private Vector3 rollDirection;

    private float verticalVelocity; 

    [Header("Dodge Settings")]
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float dodgeStaminaCost = 4f;
    [SerializeField] private float dodgeDistant = 1;
    [SerializeField] private int backStepStartTime = 1;

    [Header("Debug")]
    [SerializeField] float dotValue;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void Update()
    {
        HandleDodge();
    }

    public void HandleDodge()
    {
        if (dodgeInput)
        {
            dodgeInput = false;
            AttemptToPerformDodge();
        }
    }

    public void AttemptToPerformDodge()
    {
        if (PlayerManager.instance.playerCurrentState.isPerformingAction)
            return;

        if (PlayerManager.instance.playerStatManager.currentStamina < dodgeStaminaCost)
            return;

        CheckDirectionBetweenCamera();
        if (dotValue > 0 && player.playerMovement.verticalMovement <= -0.8 && PlayerManager.instance.characterController.isGrounded)
        {
            PlayerManager.instance.playerAnimatorManager.PlayerTargetActionAnimation("BackStep", true, true);
            PlayerManager.instance.playerStatManager.CurrentStamina -= dodgeStaminaCost;
            PlayerManager.instance.playerStatManager.ResetStaminaRegenTimer();
        }

        else if (PlayerManager.instance.playerInput.moveAmount > 0 && PlayerManager.instance.characterController.isGrounded)
        {
            Debug.Log(dotValue +",,"  +player.playerMovement.verticalMovement);
            PlayerManager.instance.playerCurrentState.isPerformingAction = true;
            PlayerManager.instance.playerCurrentState.isRolling = true;
            rollDirection = CameraPlayer.instance.cameraObject.transform.forward * PlayerManager.instance.playerInput.verticalInput;
            rollDirection += CameraPlayer.instance.cameraObject.transform.right * PlayerManager.instance.playerInput.horizontalInput;
            rollDirection.y = 0;
            rollDirection.Normalize();

            transform.rotation = Quaternion.LookRotation(rollDirection);

            // ✅ ใช้ Root Motion ตอนกลิ้งไปข้างหน้า
            PlayerManager.instance.playerAnimatorManager.PlayerTargetActionAnimation("Rolling", true, true);
            PlayerManager.instance.playerStatManager.CurrentStamina -= dodgeStaminaCost;
            PlayerManager.instance.playerStatManager.ResetStaminaRegenTimer();


        }
    }

    private void CheckDirectionBetweenCamera()
    {
        Vector3 camera = CameraPlayer.instance.transform.forward;
        Vector3 player = PlayerManager.instance.transform.forward;
 
        dotValue = Vector3.Dot(player, camera);
        
        
    }


    private void OnAnimatorMove()
    {
        if (!PlayerManager.instance.playerCurrentState.applyRootMotion)
        {
            return;
        } 

        // 1. รับค่าการเคลื่อนที่จาก Animation (ซ้าย-ขวา-หน้า-หลัง)
        Vector3 velocity = PlayerManager.instance.animator.deltaPosition;

        // 2. คำนวณแรงโน้มถ่วง (บน-ล่าง)
        if (PlayerManager.instance.characterController.isGrounded)
        {
            // ถ้าอยู่บนพื้น ให้กดไว้เบาๆ เพื่อให้ IsGrounded ทำงานแม่นยำ
            verticalVelocity = -0.2f; 
        }
        else
        {
            // ถ้าลอยอยู่ ให้เพิ่มแรงดึงลงตามเวลา
            verticalVelocity += gravity * Time.deltaTime;
        }

        // 3. เอาแรงโน้มถ่วงใส่เข้าไปในแกน Y ของการเคลื่อนที่
        // หมายเหตุ: deltaPosition คือระยะทางต่อเฟรมอยู่แล้ว แต่ gravity คือความเร่ง ต้องคูณ Time.deltaTime
        velocity.y = verticalVelocity * Time.deltaTime;

        // 4. สั่งเคลื่อนที่รวมกันทีเดียว
        PlayerManager.instance.characterController.Move(velocity * dodgeDistant);

        // 5. ส่วน Rotation
        Quaternion deltaRot = PlayerManager.instance.animator.deltaRotation;
        Vector3 eulerRot = deltaRot.eulerAngles;
        Quaternion yRotation = Quaternion.Euler(0, eulerRot.y, 0);
        transform.rotation *= yRotation;
    }
}
