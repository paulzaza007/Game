using UnityEngine;

public class AIMovementManager : MonoBehaviour
{
    public void RotateTowardAgent(AICharacterManager aICharacter)
    {
        if (aICharacter.IsMoving)
        {
            aICharacter.transform.rotation = aICharacter.navMeshAgent.transform.rotation;
        }
    }
}
