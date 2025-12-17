using UnityEngine;

public class WorldUtilityManager : MonoBehaviour
{
    public static WorldUtilityManager instance;

    [Header("Layers")]
    [SerializeField] LayerMask playerLayers;
    [SerializeField] LayerMask enviroLayers;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public LayerMask GetPlayerLayer()
    {
        return playerLayers;
    }

    public LayerMask GetEnviroLayers()
    {
        return enviroLayers;
    }

    public bool CanIDamageThisTarget(CharacterGroup attackingCharacter, CharacterGroup targetCharacter)
    {
        if(attackingCharacter == CharacterGroup.Team01)
        {
            switch (targetCharacter)
            {
                case CharacterGroup.Team01: return false;
                    
                case CharacterGroup.Team02: return true;
                    
                default:
                    break;
            }
        }
        else if(attackingCharacter == CharacterGroup.Team02)
        {
            switch (targetCharacter)
            {
                case CharacterGroup.Team01: return true;
                    
                case CharacterGroup.Team02: return false;
                    
                default:
                    break;
            }
        }

        return false;
    }
}
