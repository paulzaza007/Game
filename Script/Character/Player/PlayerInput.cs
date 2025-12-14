using Unity.VisualScripting;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private InputSystem_Actions playerInput;

    [SerializeField] Vector2 movementInput;
    [SerializeField] public float verticalInput;
    [SerializeField] public float horizontalInput;
    [SerializeField] public float moveAmout;

    [SerializeField] Vector2 cameraInput;
    public float cameraVerticalInput;
    public float cameraHorizontalInput;

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
            Player.instance.playerMovement.isRunning = false    ;
        };
        
        //Jump Input
        playerInput.Player.Jump.performed += context => preventJumpTwice();
        

        
        //Dodge Input
        playerInput.Player_Actions.Dodge.performed += context => Player.instance.playerDodge.dodgeInput = true;

    }
    
    private void Update()
    {
        MovementInput();
        CameraMovementInput();
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
}
