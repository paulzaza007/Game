using UnityEngine;

[CreateAssetMenu(menuName = "Player Actions/Weapon Actions/Test Action")]
public class WeaponItemAction : ScriptableObject
{
    public int actionID;

    public virtual void AttemptToPerformAction(Player playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        playerPerformingAction.currentWeaponBeingUsed = weaponPerformingAction.itemID;

        //Debug.Log("Action!");
    }
 
    
}
