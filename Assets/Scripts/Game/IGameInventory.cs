public interface IGameInventory
{
    public bool Add(InventoryItemData reference, int count);
    public bool Add(InventoryItemData reference, int count, int slotId);
    public bool Remove(InventoryItemData reference, int count);
    public string GetName();
}
