using UnityEngine;

public class CharacterEffectManager : MonoBehaviour
{
    CharacterManager character;

    [Header("VFX")]
    [SerializeField] GameObject bloodSplatterVFX;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual void ProcessInstantEffect(InstantPlayerEffect effect)
    {
        effect.ProcessEffect(character);
    }

    public void PlayBloodSplatterVFX(Vector3 contactPoint)
    {
        if (bloodSplatterVFX != null)
        {
            GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
        }
        else
        {
            GameObject bloodSplatter = Instantiate(WorldPlayerEffectManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);

        }
    }
}
