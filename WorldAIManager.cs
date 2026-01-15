using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

    [Header("Debug")]
    [SerializeField] bool despawnCharacter = false;
    [SerializeField] bool respawnCharacter = false;

    [Header("Characters")] 
    [SerializeField] GameObject[] aiCharacters;
    [SerializeField] List<AICharacterManager> aICharacterSpawner;
    [SerializeField] List<AICharacterManager> spawnedInCharacters;

    [Header("Bosses")]
    [SerializeField] List<AIBossManager> spawnedInBoss;

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
        StartCoroutine(WaitForSceneToLoadThenSpawnCharacter());
    }

    private void Update()
    {
        if (despawnCharacter)
        {
            despawnCharacter = false;
            DespawnAllCharacters();
        }

        if (respawnCharacter)
        {
            respawnCharacter = false;
            SpawnAllCharacters();
        }
    }

    private IEnumerator WaitForSceneToLoadThenSpawnCharacter()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }

        SpawnAllCharacters();
    }

    public void AddCharacterTospawnCharacterList(AICharacterManager character)
    {
        if (spawnedInCharacters.Contains(character))
        {
            return;
        }

        spawnedInCharacters.Add(character);

        AIBossManager bossCharacter = character.GetComponent<AIBossManager>();

        if (bossCharacter != null)
        {
            if (!spawnedInBoss.Contains(bossCharacter))
            {
                spawnedInBoss.Add(bossCharacter);
            }
        }
    }

    public AIBossManager GetBossCharacterByID(int ID)
    {
        return spawnedInBoss.FirstOrDefault(boss => boss.bossID == ID);
    }

    private void SpawnAllCharacters()
    {
        if (aiCharacters == null || aiCharacters.Length == 0)
        {
            return;
        }

        foreach (var characterPrefab in aiCharacters)
        {
            if (characterPrefab == null) continue;

            GameObject instantiatedCharacter = Instantiate(characterPrefab);
            Debug.Log($"กำลังเสก: {characterPrefab.name}");
            AICharacterManager aiScript = instantiatedCharacter.GetComponent<AICharacterManager>();

            if (aiScript != null)
            {
                AddCharacterTospawnCharacterList(aiScript);
            }
        }
    }

    private void DespawnAllCharacters()
    {
        foreach (var character in spawnedInCharacters)
        {
            Destroy(character.gameObject);
        }
    }

    private void DisableAllCharacter()
    {
        
    }
}
