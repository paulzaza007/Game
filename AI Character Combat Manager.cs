using Mono.Cecil;
using UnityEngine;
using UnityEngine.UIElements;

public class AICharacterCombatManager : PlayerCombatManager
{
    [Header("Action Recovery")]
    public float actionRecoveryTimer = 0;

    [Header("Target Information")]
    public float distanceFromTarget;
    public float viewableAngle;
    public Vector3 targetDirection;                                     
    
    
    [Header("Detection")]
    [SerializeField] float detectionRadius = 15;
    public float minimumFOV = -35;
    public float maximumFOV = 35;

    [Header("Attack Rotation Speed")]
    public float attackRotationSpeed = 25;

    protected override void Awake()
    {
        base.Awake();

        LockOnTargetTransform = GetComponentInChildren<LockOnTransform>().transform;
    }

    public void FindTargetViaLineOfSight(AICharacterManager aiCharacter)
    {
        if(currentTarget != null)
        {
            return;
            
        }

        Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, WorldUtilityManager.instance.GetPlayerLayer());
        
        for (int i = 0; i < colliders.Length; i++)
        {
            Player targetCharacter = colliders[i].transform.GetComponent<Player>();

            if (targetCharacter == null)
            {
                continue;
            }

            if(targetCharacter == aiCharacter)
            {
                continue;     
            }

            if (targetCharacter.playerCurrentState.isDead)
            {
                continue;
            }

            if(WorldUtilityManager.instance.CanIDamageThisTarget(aiCharacter.characterGroup, targetCharacter.characterGroup))
            {
                Vector3 targetsDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                float angleOfTarget = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);

                if(angleOfTarget > minimumFOV && angleOfTarget < maximumFOV)
                {
                    if(Physics.Linecast(aiCharacter.playerCombatManager.LockOnTargetTransform.position, 
                    targetCharacter.playerCombatManager.LockOnTargetTransform.position, 
                    WorldUtilityManager.instance.GetEnviroLayers()))
                    {
                        Debug.DrawLine(aiCharacter.playerCombatManager.LockOnTargetTransform.position, targetCharacter.playerCombatManager.LockOnTargetTransform.position);
                    }
                    else
                    {
                        targetDirection = targetCharacter.transform.position - transform.position;
                        viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, targetDirection);
                        aiCharacter.playerCombatManager.SetAttackTarget(targetCharacter);
                        PivotTowardTarget(aiCharacter);
                    }
                }
            }
        }
    }

    public void PivotTowardTarget(AICharacterManager aICharacter)
    {
        if (aICharacter.aICurrentState.isPerformingAction)
        {
            return;
        }

        if(viewableAngle >= 20 && viewableAngle <= 60)
        {
            aICharacter.aIAnimationManager.PlayerTargetActionAnimation("45Turn_Right", true);
        }
        else if (viewableAngle <= -20 && viewableAngle >= -60)
        {
            aICharacter.aIAnimationManager.PlayerTargetActionAnimation("45Turn_Left", true);
        }
        else if (viewableAngle >= 61 && viewableAngle <= 110)
        {
            aICharacter.aIAnimationManager.PlayerTargetActionAnimation("90Turn_Right", true);
        }
        else if (viewableAngle <= -61 && viewableAngle >= 110)
        {
            aICharacter.aIAnimationManager.PlayerTargetActionAnimation("90Turn_Left", true);
        }
    }

    public void RotateTowardsAgent(AICharacterManager aICharacter)
    {
        if(aICharacter.IsMoving)
        {
            aICharacter.transform.rotation = aICharacter.navMeshAgent.transform.rotation;
        }
    }

    public void RotateTowardsTargetWhileAttacking(AICharacterManager aICharacter)
    {
        if(currentTarget == null)
        {
            return;
        }

        if (!aICharacter.aICurrentState.isPerformingAction)
        {
            return;
        }

        Vector3 targetDirection = currentTarget.transform.position - aICharacter.transform.position;
        targetDirection.y = 0; 
        targetDirection.Normalize();

        if(targetDirection == Vector3.zero)
        {
            targetDirection = aICharacter.transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        aICharacter.transform.rotation = Quaternion.Slerp(aICharacter.transform.rotation, targetRotation, attackRotationSpeed * Time.deltaTime);
    }

    public void HandleActionRecovery(AICharacterManager aICharacter)
    {
        if(actionRecoveryTimer > 0)
        {
            if (!aICharacter.aICurrentState.isPerformingAction)
            {
                actionRecoveryTimer -= Time.deltaTime;
            }
        }
    }
}
