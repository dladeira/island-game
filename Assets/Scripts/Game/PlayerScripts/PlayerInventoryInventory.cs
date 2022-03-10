using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInventoryInventory : NetworkBehaviour
{
    // [SerializeField] private NetworkGamePlayerIsland player;
    // [SerializeField] private List<InventorySlot> inventorySlots;
    // [SerializeField] private GameObject inventoryPanel;
    // [SerializeField] private int inventorySize;

    // public List<InventoryItem> inventory = new List<InventoryItem>();

    // public event Action onInventoryChangeEvent;

    // void Start()
    // {
    //     onInventoryChangeEvent += DrawInventory;
    //     onInventoryChangeEvent?.Invoke();

    //     ToggleInventory(false);
    // }

    // private void DrawInventory()
    // {
    //     if (hasAuthority) // Only draw our own inventory
    //     {
    //         int index = 0;

    //         foreach (InventorySlot slot in inventorySlots)
    //         {
    //             if (inventory.Count > index)
    //             {
    //                 InventoryItem item = inventory[index];
    //                 slot.Set(item, player, this);
    //             }
    //             else
    //             {
    //                 slot.Set(null, player, this);
    //             }
    //             index++;
    //         }
    //     }
    // }

    // public string GetName()
    // {
    //     return "PlayerInventory";
    // }

    // public bool Add(InventoryItemData reference, int count)
    // {
    //     return CmdAdd(reference.id, count);
    // }

    // public bool Add(InventoryItemData reference, int count, int slotId)
    // {
    //     return CmdAdd(reference.id, count);
    // }

    // public bool Remove(InventoryItemData reference, int count)
    // {
    //     CmdRemove(reference.id, count);
    //     return true;
    // }

    // public bool Has(InventoryItemData reference, int count)
    // {

    //     InventoryItem itemStack = GetInventoryItem(reference);
    //     if (itemStack != null)
    //     {
    //         return itemStack.stackSize >= count;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }

    // public bool CmdAdd(string itemId, int count)
    // {
    //     if (inventory.Count >= inventorySize)
    //         return false;

    //     InventoryItemData referenceData = (NetworkManager.singleton as NetworkManagerIsland).IdToItem(itemId);

    //     InventoryItem itemStack = GetInventoryItem(referenceData);
    //     if (itemStack != null)
    //     {
    //         itemStack.AddToStack(count);
    //     }
    //     else
    //     {
    //         InventoryItem newItem = new InventoryItem(referenceData, count);
    //         inventory.Add(newItem);
    //     }

    //     onInventoryChangeEvent?.Invoke();
    //     return true;
    // }

    // public void CmdRemove(string itemId, int count)
    // {
    //     InventoryItemData referenceData = (NetworkManager.singleton as NetworkManagerIsland).IdToItem(itemId);

    //     InventoryItem itemStack = GetInventoryItem(referenceData);
    //     if (itemStack != null)
    //     {
    //         itemStack.RemoveFromStack(count);

    //         if (itemStack.stackSize <= 0)
    //         {
    //             inventory.Remove(itemStack);
    //         }
    //     }

    //     onInventoryChangeEvent?.Invoke();
    // }

    // public InventoryItem GetInventoryItem(InventoryItemData referenceData)
    // {
    //     for (var i = 0; i < inventory.Count; i++)
    //     {
    //         InventoryItem item = inventory[i];

    //         if (item.data.id == referenceData.id)
    //         {
    //             return item;
    //         }
    //     }
    //     return null;
    // }

    // public void ToggleInventory(bool open)
    // {
    //     inventoryPanel.SetActive(open);
    // }
}