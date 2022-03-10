using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInventory : NetworkBehaviour
{
    [SerializeField] private NetworkGamePlayerIsland player;

    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private List<InventorySlot> backpack;
    [SerializeField] private List<InventorySlot> hotbar;

    private List<InventorySlot> inventorySlots;

    void Awake()
    {
        inventorySlots = new List<InventorySlot>();
        inventorySlots.AddRange(hotbar);
        inventorySlots.AddRange(backpack);

        int setId = 0;
        foreach (InventorySlot slot in inventorySlots)
        {
            slot.SetId(setId++);
            slot.SetPlayer(player);
        }

        ToggleOpen(false);
    }

    public event Action onInventoryChangeEvent;

    public void ToggleOpen(bool open)
    {
        inventoryPanel.SetActive(open);
    }

    public void UpdateInventory()
    {
        onInventoryChangeEvent?.Invoke();
        Debug.Log("updating inventory 2");
    }

    public bool AddItem(InventoryItem item)
    {
        int slotId = GetFirstItemSlot(item);

        if (slotId >= 0)
        {
            ModifySlot(slotId, item.stackSize);
            return true;
        }
        int emptyInventorySlot = GetNextEmptyInventorySlot();

        if (emptyInventorySlot >= 0)
        {
            SetSlot(emptyInventorySlot, item);
            return true;
        }
        return false;
    }

    // ===== Helper Methods

    public void SetSlot(int slotId, InventoryItem item)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.GetId() == slotId)
            {
                if (item != null)
                {
                    slot.SetItem(item);
                }
                else
                {
                    slot.ClearItem();
                }
            }
        }

        UpdateInventory();
    }

    public void ModifySlot(int slotId, int stackChange)
    {
        InventoryItem item = GetSlot(slotId);

        if (item != null)
        {
            item.AddToStack(stackChange);

            if (item.stackSize <= 0)
            {
                SetSlot(slotId, null);
                return;
            }
        }


        SetSlot(slotId, item);
    }

    public InventoryItem GetSlot(int slotId)
    {
        return GetSlotObject(slotId).GetItem();
    }

    public InventorySlot GetSlotObject(int slotId)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.GetId() == slotId)
            {
                return slot;
            }
        }

        return null;
    }

    public int GetFirstItemSlot(InventoryItem item)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.GetItem() != null)
            {
                if (slot.GetItem().data.id == item.data.id)
                {
                    return slot.GetId();
                }
            }
        }

        return -1;
    }

    private int GetNextEmptyInventorySlot()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            // Skip all the hotbar slots
            if (GetSlot(hotbar.Count + i) == null)
                return hotbar.Count + i;
        }

        return -1;
    }

    private void GetItems()
    {
        List<InventoryItem> items = new List<InventoryItem>();

        foreach (InventorySlot slot in inventorySlots)
        {
            InventoryItem slotItem = slot.GetItem();

            if (slotItem != null)
            {
                bool itemFound = false;

                foreach (InventoryItem item in items)
                {
                    if (item.data.id == slotItem.data.id)
                    {
                        item.AddToStack(item.stackSize);
                        itemFound = true;
                        break;
                    }
                }

                if (!itemFound)
                    items.Add(slotItem);
            }

        }
    }
}
