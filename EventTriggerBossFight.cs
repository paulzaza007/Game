using Unity.VisualScripting;
using UnityEngine;

public class EventTriggerBossFight : MonoBehaviour
{
    [SerializeField] int bossID;

    public void OnTriggerEnter(Collider other)
    {
        AIBossManager boss = WorldAIManager.instance.GetBossCharacterByID(bossID);

        if(boss != null)
        {
            boss.WakeBoss();
            Debug.Log("ตื่น!");
        }
        Debug.Log("ชน!");
    }
    
}
