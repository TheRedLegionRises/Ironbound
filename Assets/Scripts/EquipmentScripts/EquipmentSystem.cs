
using UnityEngine;
using System.Collections.Generic;

public class EquipmentSystem : MonoBehaviour
{
    [Header("Inventory")]
    public List<EquipmentItem> inventory = new List<EquipmentItem>();

    [Header("Equipped Items")]
    public List<EquipmentItem> equippedItems = new List<EquipmentItem>();

    // Return all equipped items
    public List<EquipmentItem> GetEquippedItems()
    {
        return equippedItems;
    }
    //Return specific equipped item
    public EquipmentItem GetEquippedItem(EquipmentSlot slot)
    {
        return equippedItems.Find(item => item != null && item.slot == slot);
    }

    public bool AddToInventory(EquipmentItem item)
    {
        if (item == null)
            return false;

        inventory.Add(item);

        return true;
    }

    public bool RemoveFromInventory(EquipmentItem item)
    {
        return inventory.Remove(item);
    }

    public bool EquipItem(EquipmentItem item)
    {
        if (item == null)
            return false;

        if (!inventory.Contains(item))
            return false;

        EquipmentItem oldItem =
            GetEquippedItem(item.slot);

        inventory.Remove(item);

        // Return previously equipped item to inventory
        if (oldItem != null)
        {
            equippedItems.Remove(oldItem);
            inventory.Add(oldItem);
        }

        equippedItems.Add(item);

        Debug.Log("Equipped: " + item.itemName);

        return true;
    }

    public bool UnequipItem(EquipmentSlot slot)
    {
        EquipmentItem item = GetEquippedItem(slot);

        if (item == null)
            return false;

        equippedItems.Remove(item);
        inventory.Add(item);

        Debug.Log("Unequipped: " + item.itemName);

        return true;
    }

    public int GetTotalArmor()
    {
        int armor = 0;

        foreach (EquipmentItem item in equippedItems)
        {
            if (item != null)
                armor += item.armorBonus;
        }

        return armor;
    }

    public int GetTotalDamageBonus()
    {
        int damage = 0;

        foreach (EquipmentItem item in equippedItems)
        {
            if (item != null)
                damage += item.damageBonus;
        }

        return damage;
    }

    public int GetMaxHealthBonus()
    {
        int bonus = 0;

        foreach (EquipmentItem item in equippedItems)
        {
            if (item != null)
                bonus += item.maxHealthBonus;
        }

        return bonus;
    }
}
