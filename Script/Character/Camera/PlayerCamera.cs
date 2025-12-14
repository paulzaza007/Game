using UnityEngine;
using UnityEngine.InputSystem;

public class CameraPlayer : MonoBehaviour
{
    public static CameraPlayer instance;
    public Camera cameraObject;
    [SerializeField] Transform player;
    [SerializeField] Transform cameraPivotTransform;

    private InputSystem_Actions inputActions;
    private Vector2 cameraInput;

    [Header("Camera Settings")]
    [SerializeField] float cameraSmoothSpeed = 0.05f;
    [SerializeField] float sensitivityX = 0.75f;   
    [SerializeField] float sensitivityY = 0.45f;
    [SerializeField] float minimumPivot = -30f;
    [SerializeField] float maximumPivot = 60f;
    [SerializeField] float cameraCollisionRadius = 0.3f;
    [SerializeField] LayerMask collideWithLayers;

    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    private float leftAndRightLookAngle;
    private float upAndDownLookAngle;
    private float cameraZPosition;
    private float targetCameraZPosition;
    private float cameraZVelocity;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        inputActions = new InputSystem_Actions();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void LateUpdate()
    {
        HandleAllCameraActions();
    }

    private void HandleAllCameraActions()
    {
        if (player == null)
            return;

        cameraInput = inputActions.PlayerCamera.CameraControls.ReadValue<Vector2>();
        FollowTarget();
        Rotation();
        Collision();
    }

    private void FollowTarget()
    {
        Vector3 targetPos = Vector3.SmoothDamp(transform.position, player.position, ref cameraVelocity, cameraSmoothSpeed);
        transform.position = targetPos;
    }

    private void Rotation()
    {
        float mouseX = cameraInput.x * sensitivityX;
        float mouseY = cameraInput.y * sensitivityY;

        leftAndRightLookAngle += mouseX;
        upAndDownLookAngle -= mouseY;
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

        transform.rotation = Quaternion.Euler(0f, leftAndRightLookAngle, 0f);
        cameraPivotTransform.localRotation = Quaternion.Euler(upAndDownLookAngle, 0f, 0f);
    }

    private void Collision()
    {
        targetCameraZPosition = cameraZPosition;
        RaycastHit hit;

        Vector3 pivotPos = cameraPivotTransform.position;
        pivotPos.y = cameraObject.transform.position.y;
        Vector3 direction = (cameraObject.transform.position - pivotPos).normalized;

        if (Physics.SphereCast(pivotPos, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
        {
            float distanceFromHit = Vector3.Distance(pivotPos, hit.point);
            targetCameraZPosition = -(distanceFromHit - cameraCollisionRadius);
        }

        float minDistance = -cameraZPosition;
        if (targetCameraZPosition > minDistance)
            targetCameraZPosition = minDistance;

        Vector3 localPos = cameraObject.transform.localPosition;
        localPos.z = Mathf.SmoothDamp(localPos.z, targetCameraZPosition, ref cameraZVelocity, 0.05f);
        cameraObject.transform.localPosition = localPos;
    }
    

}
