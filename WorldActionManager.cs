using UnityEngine;
using System.Linq;

public class WorldActionManager : MonoBehaviour
{
    public static WorldActionManager instance;

    [Header("Weapon Action Actions")]
    public WeaponItemAction[] weaponActionItem;

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

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        for(int i = 0; i < weaponActionItem.Length; i++)
        {
            weaponActionItem[i].actionID = i;
        }
    }

    public WeaponItemAction GetWeaponActionItemByID(int ID)
    {
        return weaponActionItem.FirstOrDefault(action => action.actionID == ID);
    }
}
