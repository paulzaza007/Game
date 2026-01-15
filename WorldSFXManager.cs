using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WorldSFXManager : MonoBehaviour
{
    public static WorldSFXManager instance;

    [Header("Damage Sounds")]
    public AudioClip[] physicalDamageSFX;
    
    [Header("Action Sounds")]
    public AudioClip rollSFX;

    [Header("Undead Attack Alert")]
    public AudioClip[] UndeadAttackAlertSFX;

    [Header("Undead Idle")]
    public AudioClip UndeadIdleSFX;

    [Header("Undead Attack")]
    public AudioClip[] UndeadAttackSFX;

    [Header("Boss Track")]
    [SerializeField] AudioSource bossIntroPlayer;
    [SerializeField] AudioSource bossLoopPlayer;

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

    public void PlayBossTrack(AudioClip introTrack, AudioClip loopTrack)
    {
        bossIntroPlayer.volume = 0.5f;
        bossIntroPlayer.clip = introTrack;
        bossIntroPlayer.loop = false;
        bossIntroPlayer.Play();

        bossLoopPlayer.volume = 0.5f;
        bossLoopPlayer.clip = loopTrack;
        bossLoopPlayer.loop = true;
        bossLoopPlayer.PlayDelayed(bossIntroPlayer.clip.length);
    }

    private IEnumerator PlayBossIntroThenLoopTrack(AudioClip loopTrack)
    {
        yield return null;
    }

    public void StopBossMusic()
    {
        StartCoroutine(FadeOutBossMusicThenStop());
    }

    private IEnumerator FadeOutBossMusicThenStop()
    {
        while(bossLoopPlayer.volume > 0)
        {
            bossIntroPlayer.volume -= Time.deltaTime;
            bossLoopPlayer.volume -= Time.deltaTime;
            yield return null;
        }

        bossIntroPlayer.Stop();
        bossLoopPlayer.Stop();
    }
}
