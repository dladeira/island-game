using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerHotbar : NetworkBehaviour, IGameInventory
{
    [SerializeField] private NetworkGamePlayerIsland player;
    [SerializeField] private List<InventorySlot> inventorySlots;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private int inventorySize;

    public Dictionary<int, InventoryItem> inventory = new Dictionary<int, InventoryItem>();

    public event Action onInventoryChangeEvent;

    private void Start()
    {
        onInventoryChangeEvent += DrawInventory;
        onInventoryChangeEvent?.Invoke();

        inventoryPanel.SetActive(true);
    }

    private void DrawInventory()
    {
        if (hasAuthority) // Only draw our own inventory
        {
            int index = 0;

            foreach (InventorySlot slot in inventorySlots)
            {
                if (inventory.ContainsKey(index))
                {
                    InventoryItem item = inventory[index];
                    slot.Set(item, player, this, index);
                }
                else
                {
                    slot.Set(null, player, this, index);
                }
                index++;
            }
        }
    }

    public string GetName()
    {
        return "PlayerHotbar";
    }

    public bool Add(InventoryItemData reference, int count)
    {
        return CmdAdd(reference.id, count, inventory.Count);
    }

    public bool Add(InventoryItemData reference, int count, int slotId)
    {
        return CmdAdd(reference.id, count, slotId);
    }

    public bool Remove(InventoryItemData reference, int count)
    {
        CmdRemove(reference.id, count);
        return true;
    }

    public bool Has(InventoryItemData reference, int count)
    {

        InventoryItem itemStack = GetInventoryItem(reference);
        if (itemStack != null)
        {
            return itemStack.stackSize >= count;
        }
        else
        {
            return false;
        }
    }

    public bool CmdAdd(string itemId, int count, int slotId)
    {
        if (inventory.Count >= inventorySize)
            return false;

        InventoryItemData referenceData = (NetworkManager.singleton as NetworkManagerIsland).IdToItem(itemId);

        InventoryItem itemStack = GetInventoryItem(referenceData);
        if (itemStack != null)
        {
            itemStack.AddToStack(count);
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData, count);
            inventory.Add(slotId, newItem);
        }

        onInventoryChangeEvent?.Invoke();
        return true;
    }

    public void CmdRemove(string itemId, int count)
    {
        InventoryItemData referenceData = (NetworkManager.singleton as NetworkManagerIsland).IdToItem(itemId);

        InventoryItem itemStack = GetInventoryItem(referenceData);
        if (itemStack != null)
        {
            itemStack.RemoveFromStack(count);

            if (itemStack.stackSize <= 0)
            {
                inventory.Remove(GetInventoryItemIndex(referenceData));
            }
        }

        onInventoryChangeEvent?.Invoke();
    }

    public InventoryItem GetInventoryItem(InventoryItemData referenceData)
    {
        foreach (KeyValuePair<int, InventoryItem> kvp in inventory)
        {
            InventoryItem item = kvp.Value;

            if (item.data.id == referenceData.id)
            {
                return item;
            }
        }

        return null;
    }

    public int GetInventoryItemIndex(InventoryItemData referenceData)
    {
        foreach (KeyValuePair<int, InventoryItem> kvp in inventory)
        {
            InventoryItem item = kvp.Value;

            if (item.data.id == referenceData.id)
            {
                return kvp.Key;
            }
        }

        return -1;
    }
}