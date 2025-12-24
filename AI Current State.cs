using UnityEngine;

public class AICurrentState : CharacterCurrentStat
{
    public void EnableCanRotate()
    {
        canRotate = true;
    }

    public void DisableCanRotate()
    {
        canRotate = false;
    }
}
