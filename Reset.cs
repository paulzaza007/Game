using UnityEngine;

public class Reset : MonoBehaviour
{
    private Player player;
    [SerializeField] bool reset = false;
    [SerializeField] bool switchRightWeapon = false;
    [SerializeField] bool   switchLeftWeapon = false;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        DebugMenu(player);
    }

    private void DebugMenu(Player player)
    {
        if (reset)
        {
            reset = false;
            player.animator.CrossFade("Empty", 0.1f);

            player.playerCurrentState.isPerformingAction = false;
            player.playerCurrentState.canRotate = true;
            player.playerCurrentState.canMove = true;
            player.playerCurrentState.applyRootMotion = false;
            player.playerCurrentState.isDead = false;
            player.playerCurrentState.isJumping = false;

            player.playerStatManager.currentHealth = player.playerStatManager.baseHealth * player.playerStatManager.endurance;
        }

        if (switchRightWeapon)
        {
            switchRightWeapon = false;
            player.playerEquipmentManager.SwitchRightWeapon();
        }
    }
    
}
