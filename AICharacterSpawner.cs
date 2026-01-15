using UnityEngine;

public class AICharacterSpawner : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] GameObject characterGameObject;
    [SerializeField] GameObject instantiateGameObject;

    private void Awake()
    {
        
    }

    private void Start()
    {
        AttemptToSpawnCharacter();
    }

    public void AttemptToSpawnCharacter()
    {
        if(characterGameObject != null)
        {
            instantiateGameObject = Instantiate(characterGameObject);
            instantiateGameObject.transform.position = transform.position;
            instantiateGameObject.transform.rotation = transform.rotation;
            WorldAIManager.instance.AddCharacterTospawnCharacterList(instantiateGameObject.GetComponent<AICharacterManager>());
        }
    }
}
