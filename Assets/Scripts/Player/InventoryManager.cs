using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private InventorySystem inventory;

    [SerializeField] private List<InventorySlot> inventorySlots;

    [SerializeField] private GameObject inventoryPanel;

    public void Start()
    {
        inventory.onInventoryChangeEvent += DrawInventory;
        DrawInventory();
        inventoryPanel.SetActive(false);
    }
    public void ToggleInventory(bool open)
    {
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        inventoryPanel.SetActive(open);
    }

    public void DrawInventory()
    {
        int index = 0;

        foreach (InventorySlot slot in inventorySlots)
        {
            if (inventory.inventory.Count > index)
            {
                InventoryItem item = inventory.inventory.ToArray()[index];
                slot.Set(item);
            }
            else
            {
                slot.Set(null);
            }
            index++;
        }
    }
}
