using UnityEngine;

public class InstantPlayerEffect : ScriptableObject
{
    [Header("Effects ID")]
    public int instantEffectID;

    public virtual void ProcessEffect(CharacterManager character)
    {
        
    }
}
