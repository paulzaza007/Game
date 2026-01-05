using UnityEngine;

public class AICyclopManager : AICharacterManager
{
    public AICyclopSFXManager cyclopSFXManager;

    protected override void Awake()
    {
        base.Awake();

        cyclopSFXManager = GetComponent<AICyclopSFXManager>();
    }
}
