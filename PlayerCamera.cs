using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] float upAndDownLookAngle;
    private float cameraZPosition;
    private float targetCameraZPosition;
    private float cameraZVelocity;

    [Header("Lock On Target")]
    [SerializeField] float lockOnRadius = 20;
    [SerializeField] float minimumViewAngle = -50;
    [SerializeField] float maximumViewAngle = 50;
    [SerializeField] float unlockCameraHeight = 0;
    [SerializeField] float lockCameraHeight = 2;
    [SerializeField] float lockOnTargetFollowSpeed = 10f;
    [SerializeField] float setCameraHeightSpeed = 1;
    [SerializeField] float lockOnAngleX = 15f;
    [SerializeField] float unlockOnAngleX = 0;
    [SerializeField] float lockOnCameradistance;
    private Coroutine cameraLockOnHeightCoroutine;
    
    // เปลี่ยน Type เป็น AICharacterManager เพื่อให้รองรับศัตรู
    private List<AICharacterManager> avalibleTargets = new List<AICharacterManager>();
    public AICharacterManager nearestLockOnTarget;

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
        // 1. ถ้ามีเป้าหมายที่ล็อคอยู่ ให้กล้องหันหาเป้าหมาย
        if (nearestLockOnTarget != null) 
        {
            // หาตำแหน่งของศัตรู (พยายามหาจุด LockOnTargetTransform ถ้าไม่มีใช้ position ปกติ)
            // หมายเหตุ: ตรงนี้กูใช้ GetComponentInChildren เผื่อไว้ก่อน เพราะกูไม่รู้ว่ามึงเก็บ CombatManager ไว้ตรงไหน
            Transform targetTransform = nearestLockOnTarget.transform;
            var aiCombatManager = nearestLockOnTarget.GetComponentInChildren<AICharacterCombatManager>();
            
            if(aiCombatManager != null && aiCombatManager.LockOnTargetTransform != null)
            {
                targetTransform = aiCombatManager.LockOnTargetTransform;
            }

            Vector3 targetDir = targetTransform.position - transform.position;
            targetDir.Normalize();
            targetDir.y = 0; // ล็อคแกน Y ไม่ให้กล้องเงย/ก้ม มั่วซั่ว

            if (targetDir == Vector3.zero) targetDir = transform.forward; // กันบั๊ก

            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, cameraSmoothSpeed);
            
            // อัปเดตค่ามุมมองให้ตรงกัน เพื่อให้พอกดปลดล็อคแล้วกล้องไม่ดีด
            Vector3 rotationEulers = transform.rotation.eulerAngles;
            leftAndRightLookAngle = rotationEulers.y;
            
            return; // จบฟังก์ชันเลย ไม่ต้องคำนวณเมาส์ต่อ
        }

        // 2. ถ้าไม่มีเป้าหมาย ให้หมุนตามเมาส์ปกติ
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

    public void HandleLocatingLockOnTarget()
    {
        float shortestDistance = Mathf.Infinity;
        
        // เคลียร์ค่าเก่าทิ้ง
        avalibleTargets.Clear(); 
        nearestLockOnTarget = null;

        // **จุดสำคัญ**: ตรวจสอบ Layer ให้ดีว่ารวม Enemy แล้วหรือยัง
        Collider[] colliders = Physics.OverlapSphere(player.position, lockOnRadius, WorldUtilityManager.instance.GetPlayerLayer());

        for (int i = 0; i < colliders.Length; i++)
        {
            // เปลี่ยนมาหา AICharacterManager แทน Player
            AICharacterManager character = colliders[i].GetComponent<AICharacterManager>();

            if(character != null && !character.aICurrentState.isDead)
            {
                // ตรวจสอบมุมมอง
                Vector3 lockOntargetsDirection = character.transform.position - player.position;
                float distanceFromTarget = Vector3.Distance(player.position, character.transform.position);
                
                // ใช้ Forward ของกล้อง เพื่อดูว่าศัตรูอยู่หน้ากล้องไหม
                float viewableAngle = Vector3.Angle(lockOntargetsDirection, cameraObject.transform.forward);

                // ตรวจสอบว่าศัตรูตายหรือยัง (ถ้ามีตัวแปร isDead ให้เปิดบรรทัดนี้)
                // if (character.isDead) continue;

                if(viewableAngle > minimumViewAngle && viewableAngle < maximumViewAngle)
                {
                    RaycastHit hit;
                    
                    // ตรวจสอบสิ่งกีดขวาง
                    Transform targetLockOnTransform = character.transform;
                    var aiCombat = character.GetComponentInChildren<AICharacterCombatManager>();
                    if(aiCombat != null && aiCombat.LockOnTargetTransform != null)
                    {
                        targetLockOnTransform = aiCombat.LockOnTargetTransform;
                    }

                    if(Physics.Linecast(player.GetComponent<PlayerCombatManager>().LockOnTargetTransform.position,
                        targetLockOnTransform.position, 
                        out hit, 
                        WorldUtilityManager.instance.GetEnviroLayers()))
                    {
                        continue; // โดนบัง
                    }
                    else
                    {
                        avalibleTargets.Add(character);
                    }
                }
            }
        }

        // เลือกตัวที่ใกล้ที่สุด
        for (int k = 0; k < avalibleTargets.Count; k++)
        {
            float distanceFromTarget = Vector3.Distance(player.position, avalibleTargets[k].transform.position);

            if(distanceFromTarget < shortestDistance)
            {
                shortestDistance = distanceFromTarget;
                nearestLockOnTarget = avalibleTargets[k];
            }
        }
    }

    public void SetLockCameraHeight()
    {
        if(cameraLockOnHeightCoroutine != null)
        {
            StopCoroutine(cameraLockOnHeightCoroutine);
        }

        cameraLockOnHeightCoroutine = StartCoroutine(SetCameraHeight());
    }

    public void ClearLockTarget()
    {
        nearestLockOnTarget = null;
    }

    public IEnumerator WaitThenFindNewTarget()
    {
        while (Player.instance.playerCurrentState.isPerformingAction)
        {
            yield return null;
        }

        ClearLockTarget();
        HandleLocatingLockOnTarget();

        if(nearestLockOnTarget != null)
        {
            Player.instance.playerCombatManager.SetLockOnTarget(nearestLockOnTarget);
            Player.instance.playerCurrentState.isLockTarget = true;
        }

        yield return null;
    }

    private IEnumerator SetCameraHeight()
    {
        float duration = 3.5f;
        float timer = 0;

        Vector3 velocity = Vector3.zero;
        Vector3 newLockCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, lockCameraHeight, cameraPivotTransform.transform.localPosition.z - lockOnCameradistance);
        Vector3 newUnlockCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, unlockCameraHeight, cameraPivotTransform.transform.localPosition.z + lockOnCameradistance);

        // สร้างตัวแปรมารอไว้เลย จะได้เรียกใช้ง่ายๆ
        Quaternion targetLockRotation = Quaternion.Euler(lockOnAngleX, 0, 0);

        while (timer < duration)
        {
            timer += Time.deltaTime;

            if (Player.instance.playerCombatManager.lockedOnEnemy != null)
            {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockCameraHeight, ref velocity, setCameraHeightSpeed);
                cameraPivotTransform.transform.localRotation = Quaternion.Slerp(cameraPivotTransform.transform.localRotation, targetLockRotation, lockOnTargetFollowSpeed * Time.deltaTime);
                upAndDownLookAngle = lockOnAngleX;
            }
            else
            {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockCameraHeight, ref velocity, setCameraHeightSpeed);
            }

            yield return null;
        }


        if (Player.instance.playerCombatManager.lockedOnEnemy != null)
        {
            cameraPivotTransform.transform.localPosition = newLockCameraHeight;
            cameraPivotTransform.transform.localRotation = targetLockRotation;
            upAndDownLookAngle = lockOnAngleX;

        }
        else
        {
            cameraPivotTransform.transform.localPosition = newUnlockCameraHeight;

        }

        yield return null;
    }
}
