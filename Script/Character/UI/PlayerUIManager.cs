using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
   public static PlayerUIManager instance;

   [HideInInspector] public PlayerUIHUDManager playerUIHUDManager;
   [HideInInspector] public PlayerUIPopUpManager playerUIPopUpManager;

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
