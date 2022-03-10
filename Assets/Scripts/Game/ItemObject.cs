using UnityEngine;
using Mirror;

public class ItemObject : NetworkBehaviour
{
    public InventoryItemData referenceItem;

    public void OnHandlePickupItem(PlayerInventory inventory)
    {
        inventory.AddItem(new InventoryItem(referenceItem, 1));
        Destroy(gameObject);
    }
}
