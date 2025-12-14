using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    Player player;
    
    protected virtual void Awake()
    {
        player = GetComponent<Player>();
    }
    
    public virtual void ProcessInstantEffect(InstantPlayerEffect effect)
    {
        effect.ProcessEffect(player);
    }
}
