using UnityEngine;

public class WorldSFXManager : MonoBehaviour
{
    public static WorldSFXManager instance;

    [Header("Damage Sounds")]
    public AudioClip[] physicalDamageSFX;
    
    [Header("Action Sounds")]
    public AudioClip rollSFX;

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

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public AudioClip ChooseRandomSFXFromArray(AudioClip[] array)
    {
        int index = Random.Range(0, array.Length);

        return array[index];
    }
}
