using System.Collections;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.AI;

public class AICharacterManager : CharacterManager
{
   
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
    public AIDeathState deathState;
    

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

    public bool isActive = false;
    public bool IsActive
    {
        get => isActive;
        set
        {
            if (isActive == value) return;

            bool old = isActive;
            isActive = value;

            OnActive?.Invoke(old, value);
        }
    }

    public event System.Action<bool, bool> OnActive;

    protected override void Awake()
    {
        base.Awake();
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

    private void Start()
    {
        characterSFXManager.audioSource.loop = true;
        characterSFXManager.audioSource.clip = WorldSFXManager.instance.UndeadIdleSFX;
        characterSFXManager.audioSource.volume = 1f;
        characterSFXManager.audioSource.Play();
    }

    protected virtual void Update()
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
            if (newBool)
            {
                animator.CrossFade("Walking", 0.3f);
            }
            else if(!aICurrentState.IsTargeting)
            {
                animator.CrossFade("Idle", 0.3f);
            }
            else
            {
                animator.CrossFade("Idle_Targeting", 0.3f);
            }

        }
    }

    private void OnDisable()
    {
        OnMoving -= OnMovingBoolChange;
    }

    public IEnumerator ProcessingDeath()
    {
        deathState.SwitchToDeathState(this);
        aIAnimationManager.enabled = false;
        animator.Rebind(); // ล้างค่า Parameter 
        animator.Update(0); // refresh animation
        aICharacterManager.aICurrentState.isDead = true;
        int randomDeathAnimation = Random.Range(0,2);

        characterSFXManager.StopAllAISounds();
        


        if(navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }

        if(GetComponentInChildren<Collider>() != null)
        {
            GetComponentInChildren<Collider>().enabled = false;
        }

        aICurrentState.isPerformingAction = true;
        

        yield return null;

        if(randomDeathAnimation == 0)
        {
            aIAnimationManager.PlayerTargetActionAnimation("Undead_Dying_01", true, false);
        }
        else
        {
            aIAnimationManager.PlayerTargetActionAnimation("Undead_Dying_02", true, false);
        }

        yield return new WaitForSeconds(5);

        Destroy(gameObject);
    }
}