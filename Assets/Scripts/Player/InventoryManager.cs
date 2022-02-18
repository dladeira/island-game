using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private InventorySystem inventory;

    [SerializeField] private List<InventorySlot> inventorySlots;

    public void Start()
    {
        inventory.onInventoryChangeEvent += DrawInventory;
    }

    public void DrawInventory()
    {
        int index = 0;
        foreach (InventoryItem item in inventory.inventory)
        {
            inventorySlots.ToArray()[index].Set(item);
            index++;
        }
    }
}
