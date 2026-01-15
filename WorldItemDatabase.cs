using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldItemDatabase : MonoBehaviour //เก็บข้อมูลไอเท็มทั้งหมด
{
    public static WorldItemDatabase instance;
    
    public WeaponItem unarmedWeapon;

    [Header("Weapons")]
    [SerializeField] List<WeaponItem> weapons = new List<WeaponItem>(); 

    [Header("Items")]
    [SerializeField] List<Item> items = new List<Item>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        foreach(var weapon in weapons)
        {
            items.Add(weapon); //แอดอาวุธใส่ใน List item
        }

        for(int i = 0; i < items.Count; i++)
        {
            items[i].itemID = i; //สร้าง ID ให้ item แต่ละชิ้น
        }
    }

    public WeaponItem GetWeaponByID(int ID)
    {
        return weapons.FirstOrDefault(weapon => weapon.itemID == ID); //ใช้ LINQ หา ID จากListอาวุธ
    }
}
