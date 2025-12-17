using UnityEngine;

public class AICharacterCombatManager : PlayerCombatManager
{

    [Header("Detection")]
    [SerializeField] float detectionRadius = 15;
    [SerializeField] float minimumDirectionAngle = -35;
    [SerializeField] float maximumDirectionAngle = 35;

    
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

            if(targetCharacter == null)
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
                float viewableAngle = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);

                if(viewableAngle > minimumDirectionAngle && viewableAngle < maximumDirectionAngle)
                {
                    if(Physics.Linecast(aiCharacter.playerCombatManager.LockOnTargetTransform.position, 
                    targetCharacter.playerCombatManager.LockOnTargetTransform.position, 
                    WorldUtilityManager.instance.GetEnviroLayers()))
                    {
                        Debug.DrawLine(aiCharacter.playerCombatManager.LockOnTargetTransform.position, targetCharacter.playerCombatManager.LockOnTargetTransform.position);
                        Debug.Log("BLOCK");
                    }
                    else
                    {
                        aiCharacter.playerCombatManager.SetAttackTarget(targetCharacter);
                    }
                }
            }
        }
    }
}
