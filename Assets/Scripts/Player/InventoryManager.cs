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
        foreach (InventoryItem item in inventory.inventory)
        {
            inventorySlots.ToArray()[index].Set(item);
            index++;
        }
    }
}
