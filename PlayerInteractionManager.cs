using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    PlayerManager player;

    private List<Interactable> currentInteractableActions;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    private void Start()
    {
        currentInteractableActions = new List<Interactable>();

    }

    private void FixedUpdate()
    {
        if (!PlayerUIManager.instance.menuWindowIsOpen && !PlayerUIManager.instance.popUpWindowIsOpen) //ถ้าไม่หน้าต่าง interact ขึ้น ให้หา
        {
            CheckForInteractable(); 
        }
    }

    private void CheckForInteractable() // หา interact
    {
        if (currentInteractableActions.Count == 0)
        {
            return;
        }

        if(currentInteractableActions[0] == null)
        {
            currentInteractableActions.RemoveAt(0);
            return;
        }

        if(currentInteractableActions[0] != null)
        {
            // เอา text จาก interact ที่ชน ส่งไป UI เพื่อให้แสดงข้อความ
            PlayerUIManager.instance.playerUIPopUpManager.SendPlayerMessagePopUp(currentInteractableActions[0].interactableText); 
        }
    }

    public void AddInteractionToList(Interactable interactableObject) //เก็บ interact
    {

        RefreshInteractionList();

        if (!currentInteractableActions.Contains(interactableObject)) //ไม่มี interact ตัวนี้ ให้เก็บเอาไว้
        {
            currentInteractableActions.Add(interactableObject);
        }
    }

    public void RemoveInteractionToList(Interactable interactableObject) //ลบ interact
    {
        RefreshInteractionList();

        if (currentInteractableActions.Contains(interactableObject))//มี interact ตัวนี้ ให้ลบ
        {
            currentInteractableActions.Remove(interactableObject);
        }
    }

    private void RefreshInteractionList()
    {
        for (int i = currentInteractableActions.Count - 1; i > - 1; i--) //ล้าง interact ที่มีค่าเป็น null ออก เผื่อเอาไว้
        {
            if(currentInteractableActions[i] == null)
            {
                currentInteractableActions.RemoveAt(i);
            }
        }
    }

    public void Interact() //สั่งมาจาก input ที่ player กด E
    {
        if(currentInteractableActions.Count == 0)
        {
            return;
        }
        if(currentInteractableActions[0] != null)
        {
            currentInteractableActions[0].Interact(player);
            RefreshInteractionList();
        }
    }
}

