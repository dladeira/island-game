using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InventorySystem : NetworkBehaviour
{
    private Dictionary<InventoryItemData, InventoryItem> m_itemDictionary;
    public List<InventoryItem> inventory { get; private set; }

    public event Action onInventoryChangeEvent;

    private void Awake()
    {
        inventory = new List<InventoryItem>();
        m_itemDictionary = new Dictionary<InventoryItemData, InventoryItem>();
    }

    public void Add(InventoryItemData referenceData)
    {
        Debug.Log("adding a item");
        InventoryItem itemStack = Get(referenceData);
        if (itemStack != null)
        {
            itemStack.AddToStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            inventory.Add(newItem);
            m_itemDictionary.Add(referenceData, newItem);
        }

        onInventoryChangeEvent?.Invoke();
    }

    public void Remove(InventoryItemData referenceData)
    {
        InventoryItem itemStack = Get(referenceData);
        if (itemStack != null)
        {
            itemStack.RemoveFromStack();

            if (itemStack.stackSize == 0)
            {
                inventory.Remove(itemStack);
                m_itemDictionary.Remove(referenceData);
            }
        }

        onInventoryChangeEvent?.Invoke();
    }

    public InventoryItem Get(InventoryItemData referenceData)
    {
        if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            return value;
        }
        return null;
    }
}
