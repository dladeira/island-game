using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public InventoryItemData referenceItem;

    public void OnHandlePickupItem(InventorySystem inventory)
    {
        inventory.Add(referenceItem);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider triggeror)
    {
        NetworkGamePlayerIsland player = triggeror.GetComponent<NetworkGamePlayerIsland>();
        if (player != null)
        {
            OnHandlePickupItem(triggeror.GetComponent<InventorySystem>());
        }
    }
}
