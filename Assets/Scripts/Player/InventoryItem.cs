using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using Mirror;

[Serializable]
public class InventoryItem
{
    public InventoryItemData data { get; private set; }
    public int stackSize { get; private set; }

    public InventoryItem(InventoryItemData source) {
        data = source;
        AddToStack();
    }

    public InventoryItem(InventoryItemData source, int count) {
        data = source;
        AddToStack(count);
    }
    
    public void AddToStack()
    {
        stackSize++;
    }

    public void AddToStack(int count)
    {
        stackSize += count;
    }

    public void RemoveFromStack()
    {
        stackSize--;
    }

    
}

public static class InventoryItemReadWriteFunctions 
{
    public static void WriteMyType(this NetworkWriter writer, InventoryItem item)
    {
        writer.Write<InventoryItemData>(item.data);
        writer.WriteInt(item.stackSize);
    }

    public static InventoryItem ReadMyType(this NetworkReader reader)
    {
        return new InventoryItem(reader.Read<InventoryItemData>(), reader.ReadInt());
    }
}