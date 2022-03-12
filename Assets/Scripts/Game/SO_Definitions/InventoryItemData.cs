using System;
using UnityEngine;
using UnityEditor;
using Mirror;

// <summary>Class used to store hardcoded item values and the types of items in the game</summary
[CreateAssetMenu(menuName = "Inventory Item Data")]
public class InventoryItemData : ScriptableObject
{
    // <summary>The name code uses to identify the object</summary>
    public string id;

    // <summary>The name that the player is going to see</summary>
    public string displayName;

    // <summary>The icon to display in the player inventory</summary>
    public Sprite icon;

    // <summary>The prefab to use when spawning the item as a object (in game)</summary
    public GameObject itemObjectPrefab;

    // <summary>The prefab to use when equipping the object (in game)</summary
    public GameObject equippedPrefab;

    // <summary>Set the values for the itemData, used for deserialization</summary>
    public void SetValues(string id, string displayName, string iconPath, GameObject itemObjectPrefab, GameObject equipped)
    {
        this.id = id;
        this.displayName = displayName;
        this.icon = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
        this.itemObjectPrefab = itemObjectPrefab;
        this.equippedPrefab = equipped;
    }
}

// <summary>A extension class used for serialization/deserialization over Mirror (network)</summary>
public static class InventoryItemDataReadWriteFunctions
{
    public static void WriteMyType(this NetworkWriter writer, InventoryItemData value)
    {
        writer.WriteString(value != null ? "200" : "NULL");

        if (value)
        {
            writer.WriteString(value.id);
            writer.WriteString(value.displayName);
            writer.WriteString(AssetDatabase.GetAssetPath(value.icon));
            writer.WriteString(value.itemObjectPrefab.name);
            writer.WriteString(value.equippedPrefab.name);
        }
    }

    public static InventoryItemData ReadMyType(this NetworkReader reader)
    {
        String status = reader.ReadString();

        if (status != "NULL")
        {
            InventoryItemData data = ScriptableObject.CreateInstance("InventoryItemData") as InventoryItemData;
            data.SetValues(reader.ReadString(), reader.ReadString(), reader.ReadString(), Resources.Load<GameObject>(reader.ReadString()), Resources.Load<GameObject>(reader.ReadString()));
            return data;
        }
        else
            return null;
    }
}