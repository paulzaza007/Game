using UnityEngine;
using System.Collections;

public class PlayerDodge : MonoBehaviour
{
    [Header("PLAYER ACTIONS INPUT")]
    [HideInInspector] public bool dodgeInput = false;
    
    private Vector3 rollDirection;

    private float verticalVelocity; 
    [SerializeField] private float gravity = -9.81f; // แรงโน้มถ่วง (ปรับได้)
    [SerializeField] private float dodgeStaminaCost = 4f;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
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
        if (Player.instance.playerCurrentState.isPerformingAction)
            return;

        if (Player.instance.playerStatManager.currentStamina < dodgeStaminaCost)
            return;

        if (Player.instance.playerInput.moveAmout > 0 && Player.instance.characterController.isGrounded)
        {
            Player.instance.playerCurrentState.isPerformingAction = true;
            rollDirection = CameraPlayer.instance.cameraObject.transform.forward * Player.instance.playerInput.verticalInput;
            rollDirection += CameraPlayer.instance.cameraObject.transform.right * Player.instance.playerInput.horizontalInput;
            rollDirection.y = 0;
            rollDirection.Normalize();

            transform.rotation = Quaternion.LookRotation(rollDirection);

            // ✅ ใช้ Root Motion ตอนกลิ้งไปข้างหน้า
            Player.instance.playerAnimatorManger.PlayerTargetActionAnimation("Rolling", true, true);
            Player.instance.playerStatManager.currentStamina -= dodgeStaminaCost;
            Player.instance.playerStatManager.ResetStaminaRegenTimer();


        }
        else
        {
            // ✅ ใช้ Root Motion ตอนกลิ้งถอยหลังด้วย
            //PlayerTargetActionAnimation("Roll_Backward_01", true, true);
        }
    }


    private void OnAnimatorMove()
    {
        if (!Player.instance.playerCurrentState.applyRootMotion) return;

        // 1. รับค่าการเคลื่อนที่จาก Animation (ซ้าย-ขวา-หน้า-หลัง)
        Vector3 velocity = Player.instance.animator.deltaPosition;

        // 2. คำนวณแรงโน้มถ่วง (บน-ล่าง)
        if (Player.instance.characterController.isGrounded)
        {
            // ถ้าอยู่บนพื้น ให้กดไว้เบาๆ เพื่อให้ IsGrounded ทำงานแม่นยำ
            verticalVelocity = -2f; 
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
        Player.instance.characterController.Move(velocity);

        // 5. ส่วน Rotation (แก้ไปแล้วจากข้อก่อนหน้า)
        Quaternion deltaRot = Player.instance.animator.deltaRotation;
        Vector3 eulerRot = deltaRot.eulerAngles;
        Quaternion yRotation = Quaternion.Euler(0, eulerRot.y, 0);
        transform.rotation *= yRotation;
    }
}
