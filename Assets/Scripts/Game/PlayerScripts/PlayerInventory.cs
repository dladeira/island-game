using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class PlayerInventory : NetworkBehaviour
{
    [SerializeField] private PlayerManager player;

    [Header("UI")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private List<InventorySlot> backpack;
    [SerializeField] private List<InventorySlot> hotbar;

    [Header("Pickup")]
    [SerializeField] private TMP_Text pickupText;
    [SerializeField] private LayerMask pickupMask;
    [SerializeField] private float lookDistance;

    private List<InventorySlot> inventorySlots;

    public event Action onInventoryChangeEvent;

    void Awake()
    {
        inventorySlots = new List<InventorySlot>();
        inventorySlots.AddRange(hotbar);
        inventorySlots.AddRange(backpack);

        foreach (InventorySlot slot in inventorySlots)
            slot.Initialize(player, inventorySlots.IndexOf(slot));

        ToggleOpen(false);
    }

    void Update()
    {
        DoItemPickup();
    }

    public void UpdateInventory()
    {
        onInventoryChangeEvent?.Invoke();
    }

    public void ToggleOpen(bool open)
    {
        inventoryPanel.SetActive(open);
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

    // ===== Slot methods

    public void SetSlot(int slotId, InventoryItem item)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.GetId() == slotId)
            {
                slot.SetItem(item);
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

    private int GetFirstItemSlot(InventoryItem item)
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

    private void DoItemPickup()
    {
        if (hasAuthority)
        {
            RaycastHit hit;

            if (Physics.Raycast(player.playerCamera.position, player.playerCamera.forward, out hit, lookDistance, pickupMask))
            {
                ItemObject item = hit.transform.GetComponent<ItemObject>();
                pickupText.transform.gameObject.SetActive(true);
                pickupText.text = "Pickup '" + item.referenceItem.displayName + "'";

                if (Input.GetKeyDown(KeyCode.E))
                {
                    CmdPickupItem(item);
                }
            }
            else
            {
                pickupText.transform.gameObject.SetActive(false);
            }

        }
        else
        {
            pickupText.transform.gameObject.SetActive(false);
        }
    }

    [Command]
    private void CmdPickupItem(ItemObject target)
    {
        RpcPickupItem(target);
    }

    [ClientRpc]
    private void RpcPickupItem(ItemObject target)
    {
        target.OnHandlePickupItem(this);
    }
}
