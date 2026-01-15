using UnityEngine;

[CreateAssetMenu(menuName = "Player Actions/Weapon Actions/Test Action")]
public class WeaponItemAction : ScriptableObject
{
    public int actionID;

    //สั่งจาก characterCombat ที่รับ input มาอีกที
    public virtual void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        playerPerformingAction.CurrentRightHandWeaponID = weaponPerformingAction.itemID; // เปลี่ยน ID อาวุธให้ player 

        //Debug.Log("Action!");
    }
 
    
}
