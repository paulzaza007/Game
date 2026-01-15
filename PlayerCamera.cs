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

        //รับค่าเมาส์ แกนx แกนy
        cameraInput = inputActions.PlayerCamera.CameraControls.ReadValue<Vector2>();
        FollowTarget(); //ให้กล้องตามผู้เล่น
        Rotation(); //ให้กล้องรองรับการหัน
        Collision(); //กันไม่ให้กล้องทะลุกำแพง
    }

    private void FollowTarget()
    {
        //ตำแหน่งที่ต้องตาม แบบsmooth
        Vector3 targetPos = Vector3.SmoothDamp(transform.position, player.position, ref cameraVelocity, cameraSmoothSpeed);
        //แล้วให้กล้องไปที่ตำแหน่งนั้น
        transform.position = targetPos;
    }

    private void Rotation()
    {
        // ถ้าล็อคเป้าหมาย
        if (nearestLockOnTarget != null) 
        {
            // รับตัวแปรจากเป้าหมายใกล้สุด
            Transform targetTransform = nearestLockOnTarget.transform;
            var aiCombatManager = nearestLockOnTarget.GetComponentInChildren<AICharacterCombatManager>();
            
            //เป้าหมายต้องมีสคริปaiCombat และมีจุดที่ล็อคได้
            if(aiCombatManager != null && aiCombatManager.LockOnTargetTransform != null)
            {
                targetTransform = aiCombatManager.LockOnTargetTransform;
            }

            //คำนวนทิศทาง
            Vector3 targetDir = targetTransform.position - transform.position;
            //ทำให้magnitudeเหลือ1
            targetDir.Normalize();
            //ตัดแกน y ทิ้ง
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
            {
                targetDir = transform.forward; // กันบั๊ก
            }
 
            //คำนวนทิศทางที่ต้องหัน
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            //ให้กล้องหันตาม targetRotation แบบsmooth
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, cameraSmoothSpeed);
            
            //อัปเดตค่ามุมมองให้ตรงกัน เพื่อให้พอกดปลดล็อคแล้วกล้องไม่ดีด
            Vector3 rotationEulers = transform.rotation.eulerAngles;
            leftAndRightLookAngle = rotationEulers.y;
            
            return; //จบฟังก์ชันเลย ไม่ต้องคำนวณเมาส์ต่อ
        }

        // ถ้าไม่มีเป้าหมาย ให้หมุนตามเมาส์ปกติ
        // แยกค่าที่รับมาจากเมาส์ออกเป็นสองแกน
        float mouseX = cameraInput.x * sensitivityX;
        float mouseY = cameraInput.y * sensitivityY;

        leftAndRightLookAngle += mouseX; //หันซ้ายขวา
        upAndDownLookAngle -= mouseY; //หันขึ้นลง
        // ล็อคไม่ให้ลงเกินขึ้นเกิน
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

        // สั่งหันซ้ายขวา
        transform.rotation = Quaternion.Euler(0f, leftAndRightLookAngle, 0f);
        // สั่งหันขึ้นลง
        //cameraPivotTransform เป็นchildของการหันซ้ายขวา แลยค้องใช้ localRotation
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
        
        avalibleTargets.Clear(); 
        nearestLockOnTarget = null;

        Collider[] colliders = Physics.OverlapSphere(player.position, lockOnRadius, WorldUtilityManager.instance.GetPlayerLayer());

        for (int i = 0; i < colliders.Length; i++)
        {
            AICharacterManager character = colliders[i].GetComponent<AICharacterManager>();

            if(character != null && !character.aICurrentState.isDead)
            {
                // 
                Vector3 lockOntargetsDirection = character.transform.position - player.position;
                float distanceFromTarget = Vector3.Distance(player.position, character.transform.position);
                
                // 
                float viewableAngle = Vector3.Angle(lockOntargetsDirection, cameraObject.transform.forward);

                // 
                // 

                if(viewableAngle > minimumViewAngle && viewableAngle < maximumViewAngle)
                {
                    RaycastHit hit;
                    
                    // 
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
        while (PlayerManager.instance.playerCurrentState.isPerformingAction)
        {
            yield return null;
        }

        ClearLockTarget();
        HandleLocatingLockOnTarget();

        if(nearestLockOnTarget != null)
        {
            PlayerManager.instance.playerCombatManager.SetLockOnTarget(nearestLockOnTarget);
            PlayerManager.instance.playerCurrentState.isLockTarget = true;
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

            if (PlayerManager.instance.playerCombatManager.lockedOnEnemy != null)
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


        if (PlayerManager.instance.playerCombatManager.lockedOnEnemy != null)
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
