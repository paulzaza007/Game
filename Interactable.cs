using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactableText;
    [SerializeField] protected Collider interactableCollider;

    protected virtual void Awake()
    {
        if(interactableCollider == null)
        {
            interactableCollider = GetComponent<Collider>();
        }
    }

    protected virtual void Start()
    {
        
    }

    public virtual void Interact(PlayerManager player) //รันจาก playerInteractionManager
    {
        // Interact เสร็จ ล้างinteract ออก
        interactableCollider.enabled = false; 
        player.playerInteractionManager.RemoveInteractionToList(this);
        PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponentInParent<PlayerManager>();

        if (player != null)
        {
            //Debug.Log("ชน!");
            player.playerInteractionManager.AddInteractionToList(this); //ส่ง interact ไปเพิ่มในlist
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        PlayerManager player = other.GetComponentInParent<PlayerManager>();

        if (player != null)
        {
            player.playerInteractionManager.RemoveInteractionToList(this); //ส่ง interact ไปลบในlist
        }

        PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows(); //แล้วปิดหน้าต่าง interact
    }
}