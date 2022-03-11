using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryRecipe : MonoBehaviour
{
    [SerializeField] public InventoryRecipeData data;
    [SerializeField] private GameObject inputObject;
    [SerializeField] private GameObject outputObject;
    [SerializeField] private GameObject ItemSlotPrefab;
    [SerializeField] private PlayerManager player;

    // void Start()
    // {
    //     int index = 0;
    //     foreach (InventoryItemData item in data.input)
    //     {
    //         InventorySlot slot = Instantiate(ItemSlotPrefab).GetComponent<InventorySlot>();
    //         slot.Set(new InventoryItem(item, data.inputAmount[index]), player, 0, false, data);
    //         slot.transform.SetParent(inputObject.transform);
    //         index++;
    //     }

    //     index = 0;
    //     foreach (InventoryItemData item in data.output)
    //     {
    //         InventorySlot slot = Instantiate(ItemSlotPrefab).GetComponent<InventorySlot>();
    //         slot.Set(new InventoryItem(item, data.outputAmount[index]), player, 0, false, data);
    //         slot.transform.SetParent(outputObject.transform);
    //         index++;
    //     }
    // }
}
