using UnityEngine;

public class PlayerEffectManager : PlayerEffects
{
    [Header("Debug Delete Later")]
    [SerializeField] InstantPlayerEffect effectToTest;
    [SerializeField] bool processEffect = false;

    private void Update()
    {
        if (processEffect)
        {
            processEffect = false;
            
            InstantPlayerEffect effect = Instantiate(effectToTest);

            ProcessInstantEffect(effect );
        }
    }
}
