using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InventorySystem : NetworkBehaviour
{
    [SerializeField] private NetworkGamePlayerIsland player;
    [SerializeField] private List<InventorySlot> inventorySlots;
    [SerializeField] private GameObject inventoryPanel;

    public List<InventoryItem> inventory = new List<InventoryItem>();

    public List<InventoryItem> editorInventory = new List<InventoryItem>();

    public event Action onInventoryChangeEvent;

    private void Awake()
    {

        onInventoryChangeEvent += DrawInventory;
        onInventoryChangeEvent?.Invoke();

        ToggleInventory(false);
    }

    private void DrawInventory()
    {
        Debug.Log("inventory changed of player " + player.displayName);
        int index = 0;

        foreach (InventorySlot slot in inventorySlots)
        {
            if (inventory.Count > index)
            {
                InventoryItem item = inventory[index];
                slot.Set(item, player);
            }
            else
            {
                slot.Set(null, null);
            }
            index++;
        }

        editorInventory.Clear();

        foreach (InventoryItem item in inventory)
        {
            editorInventory.Add(item);
        }
    }

    public void Add(InventoryItemData reference)
    {
        CmdAdd(reference.id);
    }

    public void Remove(InventoryItemData reference)
    {
        CmdRemove(reference.id);
    }

    public void CmdAdd(string itemId)
    {
        InventoryItemData referenceData = (NetworkManager.singleton as NetworkManagerIsland).IdToItem(itemId);

        InventoryItem itemStack = Get(referenceData);
        if (itemStack != null)
        {
            itemStack.AddToStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            inventory.Add(newItem);
        }

        onInventoryChangeEvent?.Invoke();
    }

    public void CmdRemove(string itemId)
    {
        InventoryItemData referenceData = (NetworkManager.singleton as NetworkManagerIsland).IdToItem(itemId);

        InventoryItem itemStack = Get(referenceData);
        if (itemStack != null)
        {
            itemStack.RemoveFromStack();

            if (itemStack.stackSize == 0)
            {
                inventory.Remove(itemStack);
            }
        }

        onInventoryChangeEvent?.Invoke();
    }

    public InventoryItem Get(InventoryItemData referenceData)
    {
        for (var i = 0; i < inventory.Count; i++)
        {
            InventoryItem item = inventory[i];

            if (item.data.id == referenceData.id)
            {
                return item;
            }
        }

        return null;
    }

    public void ToggleInventory(bool open)
    {
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        inventoryPanel.SetActive(open);
    }
}