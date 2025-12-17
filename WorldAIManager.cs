using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

    [Header("Debug")]
    [SerializeField] bool despawnCharacter = false;
    [SerializeField] bool respawnCharacter = false;

    [Header("Characters")] 
    [SerializeField] GameObject[] aiCharacters;
    [SerializeField] List<GameObject> spawnedInCharacters;

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

    private void SpawnAllCharacters()
    {
        foreach (var character in aiCharacters)
        {
            GameObject instantiatedCharacter = Instantiate(character);
            spawnedInCharacters.Add(instantiatedCharacter);
        }
    }

    private void DespawnAllCharacters()
    {
        foreach (var character in spawnedInCharacters)
        {
            Destroy(character);
        }
    }

    private void DisableAllCharacter()
    {
        
    }
}
