using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
   public static PlayerUIManager instance;

   [HideInInspector] public PlayerUIHUDManager playerUIHUDManager;
   [HideInInspector] public PlayerUIPopUpManager playerUIPopUpManager;

   [Header("UI Flags")]
    public bool menuWindowIsOpen = false; //ช่องเก็บของ, หน้าเมนูต่างๆ
    public bool popUpWindowIsOpen = false; //พวกชื่อไอเทมบนพื้น บนพูด



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        playerUIHUDManager = GetComponentInChildren<PlayerUIHUDManager>();
        playerUIPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>();
    }

    
}
