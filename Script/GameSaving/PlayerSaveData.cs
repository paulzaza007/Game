using UnityEngine;


[System.Serializable]

public class PlayerSaveData
{
    public string playerName ;

    public float secondsPlayed;

    [Header("World Coordinates")]
    public float xPosition;
    public float yPosition;
    public float zPosition;

    [Header("Resorces")]
    public float currentHealth;
    public float currentStamina;
    
    [Header("Stats")]
    public float vitality;
    public float endurance;
}
