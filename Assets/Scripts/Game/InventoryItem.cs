using System;
using Mirror;

// <summary>Class used to store a stack of items in the player's inventory (called itemStack in Minecraft)</summary>
[Serializable]
public class InventoryItem
{
    // <summary>The item type that is being stored</summary>
    public InventoryItemData data { get; private set; }

    // <summary>The quantity of that item</summary>
    public int stackSize { get; private set; }

    // <summary>Create a new inventoryItem with a stack size of 1</summary>
    public InventoryItem(InventoryItemData source)
    {
        data = source;
        AddToStack();
    }

    // <summary>Create a new inventoryItem with predefined stack size</summary>
    public InventoryItem(InventoryItemData source, int count)
    {
        data = source;
        AddToStack(count);
    }

    // <summary>Add a item to the stack</summary>
    public void AddToStack()
    {
        stackSize++;
    }

    public void AddToStack(int count)
    {
        stackSize += count;
    }

    // <summary>Remove a item from the stack</summary>
    public void RemoveFromStack()
    {
        stackSize--;
    }

    public void RemoveFromStack(int count)
    {
        stackSize -= count;
    }
}

// <summary>A extension class used for serialization/deserialization over Mirror (network)</summary>
public static class InventoryItemReadWriteFunctions
{
    public static void WriteMyType(this NetworkWriter writer, InventoryItem item)
    {
        writer.WriteString(item != null ? "200" : "NULL");
        if (item != null)
        {
            writer.Write<InventoryItemData>(item.data);
            writer.WriteInt(item.stackSize);
        }
    }

    public static InventoryItem ReadMyType(this NetworkReader reader)
    {
        string status = reader.ReadString();
        if (status != "NULL")
            return new InventoryItem(reader.Read<InventoryItemData>(), reader.ReadInt());
        else
            return null;
    }
}