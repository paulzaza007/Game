using UnityEngine;
using UnityEngine.Rendering;

public class PlayerSFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySFX(AudioClip sfx, float volume = 1, bool randomizePitch = true, float pitchRandom = 0.1f)
    {
        audioSource.PlayOneShot(sfx, volume);
        audioSource.pitch = 1;

        if (randomizePitch)
        {
            audioSource.pitch += Random.Range(-pitchRandom, pitchRandom);
        }
    }

    public void PlayRollSoundFX()
    {
        
    }
}
