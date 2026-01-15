using UnityEngine;
using UnityEngine.TextCore.Text;

public class AIStatManager : CharacterStatManager
{
    AICharacterManager aICharacter;
    protected override void Awake()
    {
        base.Awake();
        aICharacter = GetComponent<AICharacterManager>();
    }

    
}
