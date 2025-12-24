using UnityEngine;

public class AIState : ScriptableObject
{
    public virtual AIState Tick(AICharacterManager aICharacter)
    {
        return this;
    }

    protected virtual AIState SwitchState(AICharacterManager aICharacter, AIState newState)
    {
        ResetStateFlags(aICharacter);
        return newState;
    }

    protected virtual void ResetStateFlags(AICharacterManager aICharacter)
    {
        
    }
}
