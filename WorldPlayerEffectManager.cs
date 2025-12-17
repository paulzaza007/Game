using System.Collections.Generic;
using UnityEngine;

public class WorldPlayerEffectManager : MonoBehaviour
{
    public static WorldPlayerEffectManager instance;

    [Header("VFX")]
    public GameObject bloodSplatterVFX;
    
    [Header("Damage")]
    public TakeDamageEffect takeDamageEffect;
    

    [SerializeField] List<InstantPlayerEffect> instantEffects;
    
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

        GenerateEffectIDs();
    }

    private void GenerateEffectIDs()
    {
        for(int i = 0; i < instantEffects.Count; i++)
        {
            instantEffects[i].instantEffectID = i;
        }
    }
}
