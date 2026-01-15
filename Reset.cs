using UnityEngine;

public class Reset : MonoBehaviour
{
    private PlayerManager player;
    [SerializeField] bool reset = false;
    [SerializeField] bool switchRightWeapon = false;
    [SerializeField] bool   switchLeftWeapon = false;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        DebugMenu(player);
    }

    private void DebugMenu(PlayerManager player)
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

            player.playerStatManager.CurrentHealth = player.playerStatManager.baseHealth * player.playerStatManager.endurance;
        }

        if (switchRightWeapon)
        {
            switchRightWeapon = false;
            player.playerEquipmentManager.SwitchRightWeapon();
        }
    }
    
}
