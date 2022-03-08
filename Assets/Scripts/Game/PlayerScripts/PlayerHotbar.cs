using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor.Animations;
using UnityEngine.Animations.Rigging;
public class PlayerHotbar : NetworkBehaviour, IGameInventory
{
    [SerializeField] private NetworkGamePlayerIsland player;
    [SerializeField] private List<InventorySlot> inventorySlots;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private int inventorySize;
    [SerializeField] private GameObject holdingParent;
    private GameObject holding;
    private int equippedItem = 1;
    [SerializeField] private Rig holdingRig;

    public Transform leftGrip;
    public Transform rightGrip;

    [SerializeField] Animator anim;

    public Dictionary<int, InventoryItem> inventory = new Dictionary<int, InventoryItem>();

    public event Action onInventoryChangeEvent;

    private void Start()
    {
        onInventoryChangeEvent += DrawInventory;
        onInventoryChangeEvent?.Invoke();

        anim.SetLayerWeight(1, 0);

        inventoryPanel.SetActive(true);
    }

    void Update()
    {
        if (hasAuthority)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetEquippedItem(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetEquippedItem(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetEquippedItem(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SetEquippedItem(4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SetEquippedItem(5);
            }
        }
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
                    slot.Set(item, player, this, index, equippedItem - 1 == index);
                    if (equippedItem - 1 == index)
                    {
                        GameObject newHolding = null;
                        
                        if (holding && holding.name != item.data.name)
                        {
                            DestroyImmediate(holding);
                            newHolding = Instantiate(item.data.holdingItem, holdingParent.transform, false);
                            newHolding.name = item.data.name;
                            holding = newHolding;
                            
                        }
                        else if (!holding)
                        {

                            newHolding = Instantiate(item.data.holdingItem, holdingParent.transform, false);
                            newHolding.name = item.data.name;
                            holding = newHolding;
                        }

                        anim.Play("equip_" + item.data.id);
                    }
                }
                else
                {
                    slot.Set(null, player, this, index, equippedItem - 1 == index);
                    if (equippedItem - 1 == index)
                    {

                        anim.Play("empty");
                        if (holding)
                        {
                            Destroy(holding);
                        }
                    }
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

    public void SetEquippedItem(int index)
    {
        equippedItem = index;
        onInventoryChangeEvent?.Invoke();
    }

    [ContextMenu("Save weapon pose")]
    void SaveWeaponPose()
    {
        GameObjectRecorder recorder = new GameObjectRecorder(anim.gameObject);
        recorder.BindComponentsOfType<Transform>(holding.gameObject, false);
        recorder.BindComponentsOfType<Transform>(leftGrip.gameObject, false);
        recorder.BindComponentsOfType<Transform>(rightGrip.gameObject, false);
        recorder.TakeSnapshot(0.0f);
        recorder.SaveToClip(holding.GetComponent<WeaponScript>().weaponAnimation);
        UnityEditor.AssetDatabase.SaveAssets();
    }
}