using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public InventoryItemData referenceItem;

    public void OnHandlePickupItem(InventorySystem inventory)
    {
        inventory.Add(referenceItem);
        Destroy(gameObject);
    }
}
