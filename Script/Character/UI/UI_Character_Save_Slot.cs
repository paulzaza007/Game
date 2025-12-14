using UnityEngine;
using TMPro;
using System;

public class UI_Character_Save_Slot : MonoBehaviour
{
    SaveFileDataWriter saveFileDataWriter;

    [Header("Game Slot")]
    public CharacterSlot characterSlot;

    [Header("Character Info")]
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI timedPlayed;

    void OnEnable()
    {
        LoadSaveSlots();
    }

    private void LoadSaveSlots()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        switch (characterSlot)
        {
            case CharacterSlot.CharacterSlot_01:
                saveFileDataWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileDataWriter.CheckToSeeIfFileExists())
                {
                characterName.text = WorldSaveGameManager.instance.playerDataSlot01.playerName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            case CharacterSlot.CharacterSlot_02:
                saveFileDataWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileDataWriter.CheckToSeeIfFileExists())
                {
                characterName.text = WorldSaveGameManager.instance.playerDataSlot02.playerName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            case CharacterSlot.CharacterSlot_03:
                saveFileDataWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileDataWriter.CheckToSeeIfFileExists())
                {
                characterName.text = WorldSaveGameManager.instance.playerDataSlot03.playerName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            case CharacterSlot.CharacterSlot_04:
                saveFileDataWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileDataWriter.CheckToSeeIfFileExists())
                {
                characterName.text = WorldSaveGameManager.instance.playerDataSlot04.playerName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            case CharacterSlot.CharacterSlot_05:
                saveFileDataWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileDataWriter.CheckToSeeIfFileExists())
                {
                characterName.text = WorldSaveGameManager.instance.playerDataSlot05.playerName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }       
    }

    public void LoadGameFromCharacterSlot()
    {
        WorldSaveGameManager.instance.currentCharacterSlotBeingUsed = characterSlot;
        WorldSaveGameManager.instance.LoadGame();
    }

    public void SelectCurrentSlot()
    {
        TitleScreenManager.instance.SelectCharacterSlot(characterSlot); 
    }
}
