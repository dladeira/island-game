using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor.Animations;
using UnityEngine.Animations.Rigging;
public class PlayerHotbar : NetworkBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private GameObject equippedParent;

    [SerializeField] Animator anim;

    private int equippedItem = 1;

    void Start()
    {
        SetEquippedItem(0);
    }

    void OnEnable()
    {
        inventory.onInventoryChangeEvent += ResetEquipped;
    }

    void OnDisable()
    {
        inventory.onInventoryChangeEvent -= ResetEquipped;
    }

    void Update()
    {
        if (hasAuthority)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetEquippedItem(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetEquippedItem(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetEquippedItem(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SetEquippedItem(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SetEquippedItem(4);
            }

            anim.SetBool("IsSwinging", Input.GetMouseButton(0));
        }
    }    

    void ResetEquipped()
    {
        SetEquippedItem(equippedItem);
    }

    public void SetEquippedItem(int index)
    {
        inventory.GetSlotObject(equippedItem).SetEquipped(false);
        inventory.GetSlotObject(index).SetEquipped(true);

        InventoryItem itemToEquip = inventory.GetSlot(index);
        Debug.Log("item to equip: ");
        Debug.Log(itemToEquip);

        foreach (Transform child in equippedParent.transform)
        {
            Destroy(child.gameObject);
        }

        equippedItem = index;

        if (itemToEquip != null)
        {
            GameObject newHolding = Instantiate(itemToEquip.data.holdingItem, equippedParent.transform, false);
            newHolding.name = itemToEquip.data.id;
            anim.Play("equip_" + itemToEquip.data.id);
        }
        else
        {
            anim.Play("empty");
        }
    }
}