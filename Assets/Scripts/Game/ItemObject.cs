using UnityEngine;
using Mirror;

public class ItemObject : NetworkBehaviour
{
    public InventoryItemData referenceItem;

    public void OnHandlePickupItem(PlayerInventory inventory)
    {
        inventory.Add(referenceItem);
        Destroy(gameObject);
    }
}
