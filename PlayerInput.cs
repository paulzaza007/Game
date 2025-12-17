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
    [SerializeField] public float moveAmout;

    [Header("CAMERA INPUT VALUE")]
    [SerializeField] Vector2 cameraInput;
    public float cameraVerticalInput;
    public float cameraHorizontalInput;

    [Header("PLAYER ACTION INPUT")]
    [SerializeField] bool LC_Input = false;
    
    [Header("LOCK ON TARGET INPUT")]
    [SerializeField] bool lockTarget_Input;

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
            Player.instance.playerMovement.isRunning = true;
        };

        playerInput.Player.Sprint.canceled += context =>
        {
            Player.instance.playerMovement.isRunning = false;
        };
        
        //Jump Input
        playerInput.Player.Jump.performed += context => preventJumpTwice();
        

        
        //Dodge Input
        playerInput.Player_Actions.Dodge.performed += context => Player.instance.playerDodge.dodgeInput = true;

        //Attack Input
        playerInput.Player_Actions.LC.performed += context => LC_Input = true;

        //Lock Target Input
        playerInput.Player_Actions.LockTarget.performed += context => lockTarget_Input = true;

    }
    
    private void Update()
    {
        MovementInput();
        CameraMovementInput();
        HandleLCInput();
        HandleLockOnInput();
    }

    private void MovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        moveAmout = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
        
        if (moveAmout <= 0.5 && moveAmout > 0)
        {
            moveAmout = 0.5f;
        }
        else if (moveAmout > 0.5 && moveAmout <= 1)
        {
            moveAmout = 0.5f;
        }
        if(Player.instance == null){
            return;
        }

    }

    private void CameraMovementInput()
    {
        cameraVerticalInput = cameraInput.y * Time.deltaTime;
        cameraHorizontalInput = cameraInput.x * Time.deltaTime;
    }

    private void preventJumpTwice()
    {
        if (!Player.instance.playerCurrentState.isJumping 
        && !Player.instance.playerCurrentState.isPerformingAction 
        && Player.instance.playerStatManager.currentStamina >= Player.instance.playerMovement.jumpStaminaCost)
        {
            StartCoroutine(Player.instance.playerMovement.DelayJump());;
        }
    }

    private void HandleLCInput()
    {
        if (LC_Input)
        {
            LC_Input = false;

            Player.instance.SetPlayerActionHand(true);

            Player.instance.playerCombatManager.PerformWeaponBasedAction(Player.instance.playerInventoryManager.currentRightHandWeapon.oh_LC_Action, Player.instance.playerInventoryManager.currentRightHandWeapon);
        }
    }

    private void HandleLockOnInput()
{
    // เช็คว่าสถานะล็อคเปิดอยู่ไหม
    if (Player.instance.playerCurrentState.isLockTarget)
    {
        // 1. ดึงตัวจัดการการต่อสู้มาก่อน (เพื่อความชัวร์)
        var combatManager = Player.instance.playerCombatManager;
        if (combatManager == null) return; // กันเหนียว

        // 2. เช็คว่ามีเป้าหมายศัตรูไหม?
        if (combatManager.lockedOnEnemy == null)
        {
            // ถ้าไม่มีเป้า -> สั่งปลดล็อค
            Debug.Log("Unlock: No Enemy Found");
            Player.instance.playerCurrentState.isLockTarget = false;
            combatManager.SetLockOnTarget(null);
            CameraPlayer.instance.nearestLockOnTarget = null;
            return;
        }

        // 3. *** จุดตรวจสอบผู้ต้องสงสัย ***
        // เช็คว่าศัตรูตัวนั้น มี Component "playerCurrentState" หรือไม่?
        if (combatManager.lockedOnEnemy.playerCurrentState == null)
        {
            // ถ้าเจอข้อความนี้ใน Console แสดงว่าคุณลืมใส่ Script State ให้ศัตรู!
            Debug.LogError("ERROR: ศัตรูชื่อ " + combatManager.lockedOnEnemy.name + " ไม่มีตัวแปร playerCurrentState (เป็น Null)!");
            
            // สั่งปลดล็อคฉุกเฉินเพื่อกันเกมค้าง
            Player.instance.playerCurrentState.isLockTarget = false;
            combatManager.SetLockOnTarget(null);
            return;
        }

        // 4. ถ้าผ่านข้อ 3 มาได้ ค่อยเช็คว่าตายหรือยัง
        if (combatManager.lockedOnEnemy.playerCurrentState.isDead)
        {
            Debug.Log("Unlock: Enemy is Dead");
            Player.instance.playerCurrentState.isLockTarget = false;
            combatManager.SetLockOnTarget(null);
            CameraPlayer.instance.nearestLockOnTarget = null;
            return;
        }
    }

    // -------------------------------------------------------------
    // ส่วนกดปุ่ม Manual Unlock / Lock (เหมือนเดิม)
    // -------------------------------------------------------------
    if (lockTarget_Input && Player.instance.playerCurrentState.isLockTarget)
    {
        lockTarget_Input = false;
        Player.instance.playerCurrentState.isLockTarget = false;
        Player.instance.playerCombatManager.SetLockOnTarget(null);
        CameraPlayer.instance.nearestLockOnTarget = null;
        return;
    }

    if (lockTarget_Input && !Player.instance.playerCurrentState.isLockTarget)
    {
        lockTarget_Input = false;
        CameraPlayer.instance.HandleLocatingLockOnTarget();

        if (CameraPlayer.instance.nearestLockOnTarget != null)
        {
            Player.instance.playerCombatManager.SetLockOnTarget(CameraPlayer.instance.nearestLockOnTarget);
            Player.instance.playerCurrentState.isLockTarget = true;
        }
    }
}
}
