using UnityEngine;

public class TitleScreenLoadInputManager : MonoBehaviour
{
    InputSystem_Actions playerControls;
    
    [Header("Title Screen Inputs")]
    [SerializeField] bool deleteCharacterSlots = false;

    private void Update()
    {
        if (deleteCharacterSlots)
        {
            deleteCharacterSlots = false;
            TitleScreenManager.instance.AttemptToDeleteCharacterSlot();
        }
        
    }

    void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new InputSystem_Actions();
            playerControls.UI.Confirm.performed += i => deleteCharacterSlots = true;
        }

        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }
}
