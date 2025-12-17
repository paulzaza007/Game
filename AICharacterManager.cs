using UnityEngine;

public class AICharacterManager : CharacterManager
{
    public AICharacterCombatManager aICharacterCombatManager;

    [Header("Current State")]
    [SerializeField] AIState currentState;

    protected override void Awake()
    {
        base.Awake();

        aICharacterCombatManager = GetComponent<AICharacterCombatManager>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        ProcessStateMachine();
    }
    
    private void ProcessStateMachine()
    {
        AIState nextState = null;

        if(currentState != null)
        {
            nextState = currentState.Tick(this);
        }

        if(nextState != null)
        {
            currentState = nextState;
        }
    }
}
