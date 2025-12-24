using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.AI;

public class AICharacterManager : CharacterManager
{
    public static AICharacterManager instance;
    
    [HideInInspector] public AICharacterCombatManager aICharacterCombatManager;
    [HideInInspector] public AIMovementManager aIMovementManager;
    [HideInInspector] public AIAnimationManager aIAnimationManager;
    [HideInInspector] public AICurrentState aICurrentState;

    [Header("Navmesh Agent")]
    public NavMeshAgent navMeshAgent;

    [Header("Current State")]
    [SerializeField] AIState currentState;

    [Header("States")]
    public IdleState idle;
    public PurSueTargetState purSueTarget;
    public AICombatStanceState combatStance;
    public AIAttackState attack;
    

    [HideInInspector] public Animator animator;
    [HideInInspector] public CharacterController characterController;

    [SerializeField] public float gravity = 9.81f;

    // ระบบตรวจสอบการเคลื่อนที่พร้อม Event
    private bool isMoving = false;
    public bool IsMoving
    {
        get => isMoving;
        set
        {
            if (isMoving == value) return;
            bool old = isMoving;
            isMoving = value;
            OnMoving?.Invoke(old, value);
        }
    }

    public event System.Action<bool, bool> OnMoving;

    protected override void Awake()
    {
        base.Awake();
        
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        aICharacterCombatManager = GetComponent<AICharacterCombatManager>();
        aIMovementManager = GetComponent<AIMovementManager>();
        aICurrentState = GetComponent<AICurrentState>();
        aIAnimationManager = GetComponent<AIAnimationManager>();

        animator = GetComponent<Animator>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        characterController = GetComponent<CharacterController>();


        // ตั้งค่า Agent สำหรับ Root Motion
        // เราปิด updatePosition เพราะจะให้ Animation เป็นคนขยับ Transform เอง
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = true;

        // Clone Scriptable Objects เพื่อไม่ให้ข้อมูลทับกันระหว่าง AI แต่ละตัว
        if (idle != null) idle = Instantiate(idle);
        if (purSueTarget != null) purSueTarget = Instantiate(purSueTarget);

        currentState = idle;

        // สมัคร Event สำหรับเปลี่ยนค่า Animator
        OnMoving += OnMovingBoolChange;
    }

    private void Update()
    {
        aICharacterCombatManager.HandleActionRecovery(this);
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        ProcessStateMachine();
    }

    private void ProcessStateMachine()
    {
        AIState nextState = currentState?.Tick(this);

        if (nextState != null)
        {
            currentState = nextState;
        }

        // ล็อกให้ตัวนำทาง (Agent) อยู่ที่จุดศูนย์กลางของโมเดลเสมอ
        navMeshAgent.transform.localPosition = Vector3.zero;    
        navMeshAgent.transform.localRotation = Quaternion.identity;

        if(aICharacterCombatManager.currentTarget != null)
        {
            aICharacterCombatManager.targetDirection = aICharacterCombatManager.currentTarget.transform.position - transform.position;
            aICharacterCombatManager.viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, aICharacterCombatManager.targetDirection);
            aICharacterCombatManager.distanceFromTarget = Vector3.Distance(transform.position, aICharacterCombatManager.currentTarget.transform.position);
        }

        if (navMeshAgent.enabled)
        {
            // เช็คว่าถึงเป้าหมายหรือยัง
            float remainingDistance = Vector3.Distance(navMeshAgent.destination, transform.position);

            if (remainingDistance > navMeshAgent.stoppingDistance)
            {
                IsMoving = true;
            }
            else
            {
                IsMoving = false;
            }
        }
        else
        {
            IsMoving = false;
        }
    }

    private void OnMovingBoolChange(bool oldBool, bool newBool)
    {
        if (animator != null)
        {
            animator.SetBool("isMoving", newBool);
            animator.CrossFade("Undead_Walk", 0.2f);
        }
    }

    private void OnDisable()
    {
        OnMoving -= OnMovingBoolChange;
    }
}