using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBossManager : MonoBehaviour
{
    private AICharacterManager aICharacter;

    public int bossID = 0;
    [SerializeField] bool hasBeenDefeated = false;
    [SerializeField] bool hasBeenAwaked = false;
    [SerializeField] List<FogWallInteractable> fogWalls;
 
    [Header("DEBUG")]
    [SerializeField] bool defeatBossDebug = false;
    [SerializeField] bool wakeBossUp = false;

    private void Awake()
    { 
        aICharacter = GetComponent<AICharacterManager>();

        if (!WorldSaveGameManager.instance.currentPlayerData.bossesAwake.ContainsKey(bossID))
        {
            WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Add(bossID, false);
            WorldSaveGameManager.instance.currentPlayerData.bossesDefeat.Add(bossID, false);
        }
        else
        {
            hasBeenDefeated = WorldSaveGameManager.instance.currentPlayerData.bossesDefeat[bossID];
            hasBeenAwaked = WorldSaveGameManager.instance.currentPlayerData.bossesAwake[bossID];

            if (hasBeenDefeated)
            {
                gameObject.SetActive(false);
            }
        }

        StartCoroutine(GetFogWallsFromWorldObjectManager());

        //ถ้าBossยังไม่ถูกกำจัด ให้ Enable fog walls
        if (hasBeenAwaked)
        {
            for (int i = 0; i < fogWalls.Count; i++)
            {
                fogWalls[i].IsActive = true;
            }
        }

        //ถ้าBossถูกกำจัด ให้ Disable fog walls
        if (hasBeenDefeated)
        {
            for (int i = 0; i < fogWalls.Count; i++)
            {
                fogWalls[i].IsActive = false;
            }

            aICharacter.IsActive = false;
        }
    }

    private void Update()
    {

        if (defeatBossDebug)
        {
            defeatBossDebug = false;
            hasBeenDefeated = true;

            if (!WorldSaveGameManager.instance.currentPlayerData.bossesAwake.ContainsKey(bossID))
            {
                WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Add(bossID, true);
                WorldSaveGameManager.instance.currentPlayerData.bossesDefeat.Add(bossID, true);
            }
            else
            {
                WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Remove(bossID);
                WorldSaveGameManager.instance.currentPlayerData.bossesDefeat.Remove(bossID);
                WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Add(bossID, true);
                WorldSaveGameManager.instance.currentPlayerData.bossesDefeat.Add(bossID, true);
            }

            WorldSaveGameManager.instance.SaveGame();
        }

        if (wakeBossUp)
        {
            wakeBossUp = false;
            WakeBoss();
        }
    }

    private IEnumerator GetFogWallsFromWorldObjectManager()
    {
        while(WorldObjectManager.instance.fogWalls.Count == 0)
        {
            yield return new WaitForEndOfFrame();
        }

        fogWalls = new List<FogWallInteractable>();

        foreach (var fogWall in WorldObjectManager.instance.fogWalls)
        {
            if (fogWall.fogWallID == bossID)
            {
                fogWalls.Add(fogWall);
            }
        }
    }
  
    public IEnumerator ProcessDeathEvent()
    {
        aICharacter.characterStatManager.CurrentHealth = 0;
        aICharacter.characterCurrentState.isDead = true;

        hasBeenDefeated = true;

        if (!WorldSaveGameManager.instance.currentPlayerData.bossesAwake.ContainsKey(bossID))
        {
            WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Add(bossID, true);
            WorldSaveGameManager.instance.currentPlayerData.bossesDefeat.Add(bossID, true);
        }
        else
        {
            WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Remove(bossID);
            WorldSaveGameManager.instance.currentPlayerData.bossesDefeat.Remove(bossID);
            WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Add(bossID, true);
            WorldSaveGameManager.instance.currentPlayerData.bossesDefeat.Add(bossID, true);
        }

        WorldSaveGameManager.instance.SaveGame();

        yield return new WaitForSeconds(5);


    }

    public void WakeBoss()
    {
        hasBeenAwaked = true;
        if (!WorldSaveGameManager.instance.currentPlayerData.bossesAwake.ContainsKey(bossID))
        {
            WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Add(bossID, true);
        }
        else
        {
            WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Remove(bossID);
            WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Add(bossID, true);
        }

        for (int i = 0; i < fogWalls.Count; i++)
        {
            fogWalls[i].IsActive = true;
        }
    }
}
