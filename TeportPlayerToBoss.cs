using UnityEngine;

public class TeportPlayerToBoss : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] bool isTeleport = false;

    private void Awake()
    {
        isTeleport = true;
        Teleporting();
    }

    private void Update()
    {
        Teleporting();
    }

    private void Teleporting()
    {
        if (isTeleport)
        {
            isTeleport = false;
            player.transform.position = this.transform.position;
        }
    }
}
