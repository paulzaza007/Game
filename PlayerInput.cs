using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private InputSystem_Actions playerInput;

    [Header("MOVEMENT INPUT VALUE")]
    [SerializeField] Vector2 movementInput;
    [SerializeField] public float verticalInput;
    [SerializeField] public float horizontalInput;
    [SerializeField] public float moveAmount;
    public float accelerationHorizontalInput;

    [Header("CAMERA INPUT VALUE")]
    [SerializeField] Vector2 cameraInput;
    public float cameraVerticalInput;
    public float cameraHorizontalInput;

    [Header("PLAYER ACTION INPUT")]
    [SerializeField] bool LC_Input = false;
    [SerializeField] bool switchRightWeapon_Input = false;
    [SerializeField] bool switchLeftWeapon_Input = false;
    [SerializeField] bool interaction_Input = false;

    [Header("LOCK ON TARGET INPUT")]
    [SerializeField] bool lockTarget_Input;
    private Coroutine lockOnCoroutine;

    [Header("Trigger Inputs")]
    [SerializeField] bool RC_Input = false;
    [SerializeField] bool HoldRC_Input = false;

    private void Awake()
    {
        playerInput = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        playerInput.Player.Enable();
        playerInput.Player_Actions.Enable();
        AssignInputEvents();
    }

    private void OnDisable()
    {
        playerInput.Player.Disable();
        playerInput.Player_Actions.Disable();
    }

    private void AssignInputEvents()
    {
        //Move Input
        playerInput.Player.Move.performed += context => movementInput = context.ReadValue<Vector2>();
        playerInput.Player.Move.canceled += context => movementInput = Vector2.zero;

        //Camera Input
        playerInput.PlayerCamera.CameraControls.performed += context => cameraInput = context.ReadValue<Vector2>();

        //Sprint Input
        playerInput.Player.Sprint.performed += context =>
        {
            PlayerManager.instance.playerMovement.isRunning = true;
        };

        playerInput.Player.Sprint.canceled += context =>
        {
            PlayerManager.instance.playerMovement.isRunning = false;
        };

        //Jump Input
        playerInput.Player.Jump.performed += context => preventJumpTwice();

        //Interaction
        playerInput.Player_Actions.Interact.performed += context => interaction_Input = true;

        //Dodge Input
        playerInput.Player_Actions.Dodge.performed += context => PlayerManager.instance.playerDodge.dodgeInput = true;

        //Attack Input
        playerInput.Player_Actions.LC.performed += context => LC_Input = true;

        //Lock Target Input
        playerInput.Player_Actions.LockTarget.performed += context => lockTarget_Input = true;

        //Heavy Attack Input
        playerInput.Player_Actions.RC.performed += context => RC_Input = true;

        //Hold Heavy Attack Input
        playerInput.Player_Actions.HoldRC.performed += context => HoldRC_Input = true;
        playerInput.Player_Actions.HoldRC.canceled += context => 
        {
            HoldRC_Input = false;
        };

        //Switch Right Weapon
        playerInput.Player_Actions.SwitchRightWeapon.performed += context => switchRightWeapon_Input = true;

    }

    private void Update()
    {
        //ควบคุมทุก input
        MovementInput();
        CameraMovementInput();
        HandleLCInput();
        HandleLockOnInput();
        HandleRCInput();
        HandleChargeRCInput();
        HandleSwitchRightWeaponInput();
        HandleInteractionInput();
    }

    private void MovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        accelerationHorizontalInput = Mathf.Lerp(accelerationHorizontalInput, movementInput.x, Time.deltaTime * 5f);

        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
        

    }

    private void CameraMovementInput()
    {
        cameraVerticalInput = cameraInput.y * Time.deltaTime;
        cameraHorizontalInput = cameraInput.x * Time.deltaTime;
    }

    private void preventJumpTwice()
    {
        if (!PlayerManager.instance.playerCurrentState.isJumping
        && !PlayerManager.instance.playerCurrentState.isPerformingAction
        && PlayerManager.instance.playerStatManager.currentStamina >= PlayerManager.instance.playerMovement.jumpStaminaCost)
        {
            StartCoroutine(PlayerManager.instance.playerMovement.DelayJump()); ;
        }
    }

    private void HandleLCInput()
    {
        if (LC_Input)
        {
            LC_Input = false;

            if (PlayerManager.instance.playerCombatManager.canWeDoAnotherAttack)
            {
                PlayerManager.instance.playerCombatManager.doAnotherAttack = true;
            }
            else
            {
                PlayerManager.instance.playerCombatManager.PerformWeaponBasedAction(PlayerManager.instance.playerInventoryManager.currentRightHandWeapon.oh_LC_Action, PlayerManager.instance.playerInventoryManager.currentRightHandWeapon);

            }
        }
    }

    private void HandleLockOnInput()
    {
        // เช็คว่าสถานะล็อคเปิดอยู่ไหม
        if (PlayerManager.instance.playerCurrentState.isLockTarget)
        {

            var combatManager = PlayerManager.instance.playerCombatManager;
            // 1. ดึงตัวจัดการการต่อสู้มาก่อน (เพื่อความชัวร์)
            if (combatManager == null) return; // กันเหนียว

            // 2. เช็คว่ามีเป้าหมายศัตรูไหม?
            if (combatManager.lockedOnEnemy == null)
            {
                // ถ้าไม่มีเป้า -> สั่งปลดล็อค
                Debug.Log("Unlock: No Enemy Found");
                PlayerManager.instance.playerCurrentState.isLockTarget = false;
                combatManager.SetLockOnTarget(null);
                CameraPlayer.instance.nearestLockOnTarget = null;
                return;
            }

            // 3. *** จุดตรวจสอบผู้ต้องสงสัย ***
            // เช็คว่าศัตรูตัวนั้น มี Component "playerCurrentState" หรือไม่?
            if (combatManager.lockedOnEnemy.aICurrentState == null)
            {
                // ถ้าเจอข้อความนี้ใน Console แสดงว่าคุณลืมใส่ Script State ให้ศัตรู!
                Debug.LogError("ERROR: ศัตรูชื่อ " + combatManager.lockedOnEnemy.name + " ไม่มีตัวแปร playerCurrentState (เป็น Null)!");

                // สั่งปลดล็อคฉุกเฉินเพื่อกันเกมค้าง
                PlayerManager.instance.playerCurrentState.isLockTarget = false;
                combatManager.SetLockOnTarget(null);
                return;
            }

            // 4. ถ้าผ่านข้อ 3 มาได้ ค่อยเช็คว่าตายหรือยัง
            if (combatManager.lockedOnEnemy.aICurrentState.isDead)
            {
                
                Debug.Log("Unlock: Enemy is Dead");
                PlayerManager.instance.playerCurrentState.isLockTarget = false;
                combatManager.SetLockOnTarget(null);
                CameraPlayer.instance.nearestLockOnTarget = null;
                if (lockOnCoroutine != null)
                {
                    StopCoroutine(lockOnCoroutine);
                }
                lockOnCoroutine = StartCoroutine(CameraPlayer.instance.WaitThenFindNewTarget());
                return;
            }
        }

        // -------------------------------------------------------------
        // ส่วนกดปุ่ม Manual Unlock / Lock (เหมือนเดิม)
        // -------------------------------------------------------------
        if (lockTarget_Input && PlayerManager.instance.playerCurrentState.isLockTarget)
        {
            lockTarget_Input = false;
            PlayerManager.instance.playerCurrentState.isLockTarget = false;
            PlayerManager.instance.playerCombatManager.SetLockOnTarget(null);
            CameraPlayer.instance.nearestLockOnTarget = null;
            return;
        }

        if (lockTarget_Input && !PlayerManager.instance.playerCurrentState.isLockTarget)
        {
            lockTarget_Input = false;

            CameraPlayer.instance.HandleLocatingLockOnTarget();

            if (CameraPlayer.instance.nearestLockOnTarget != null)
            {
                PlayerManager.instance.playerCombatManager.SetLockOnTarget(CameraPlayer.instance.nearestLockOnTarget);
                PlayerManager.instance.playerCurrentState.isLockTarget = true;
            }
        }
    }

    private void HandleRCInput()
    {
        if (RC_Input)
        {
            RC_Input = false;

            PlayerManager.instance.playerCombatManager.PerformWeaponBasedAction(PlayerManager.instance.playerInventoryManager.currentRightHandWeapon.oh_RC_Action, PlayerManager.instance.playerInventoryManager.currentRightHandWeapon);

            PlayerManager.instance.IsChargingAttack = false;
        }
    }

    private void HandleChargeRCInput()
    {
        if (PlayerManager.instance.playerCurrentState.isPerformingAction)
        {
            if (HoldRC_Input)
            {
                PlayerManager.instance.IsChargingAttack = true;
            }
            else
            {
                PlayerManager.instance.IsChargingAttack = false;
            }
        }
    }

    public float switchingWeaponTime = 1f;
    private void HandleSwitchRightWeaponInput()
    {
        if (switchRightWeapon_Input)
        {
            switchRightWeapon_Input = false;
            PlayerManager.instance.playerEquipmentManager.SwitchRightWeapon();
        }
    }

    private void HandleInteractionInput()
    {
        if (interaction_Input)
        {
            interaction_Input = false;
            
            PlayerManager.instance.playerInteractionManager.Interact();
        }
    }
}
