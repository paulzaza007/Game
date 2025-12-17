using UnityEngine;

public class PlayerEffectsManager : MonoBehaviour
{
    Player player;

    [Header("VFX")]
    [SerializeField] GameObject bloodSplatterVFX;
    
    protected virtual void Awake()
    {
        player = GetComponent<Player>();
    }
    
    public virtual void ProcessInstantEffect(InstantPlayerEffect effect)
    {
        effect.ProcessEffect(player);
    }

    public void PlayBloodSplatterVFX(Vector3 contactPoint)
    {
        if(bloodSplatterVFX != null)
        {
            GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
        }
        else
        {
            GameObject bloodSplatter = Instantiate(WorldPlayerEffectManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
        }
    }
}
