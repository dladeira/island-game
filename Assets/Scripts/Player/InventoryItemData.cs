using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mirror;

[CreateAssetMenu(menuName= "Inventory Item Data")]
public class InventoryItemData : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public GameObject prefab;

    public void SetValues(string id, string displayName, string iconPath, GameObject prefab)
    {
        this.id = id;
        this.displayName = displayName;
        this.icon = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
        this.prefab = prefab;
    }
}

public static class InventoryItemDataReadWriteFunctions 
{
    public static void WriteMyType(this NetworkWriter writer, InventoryItemData value)
    {
        writer.WriteString(value.id);
        writer.WriteString(value.displayName);
        writer.WriteString(AssetDatabase.GetAssetPath(value.icon));
        writer.WriteGameObject(value.prefab);
    }

    public static InventoryItemData ReadMyType(this NetworkReader reader)
    {
        InventoryItemData data = ScriptableObject.CreateInstance("InventoryItemData") as InventoryItemData;
        data.SetValues(reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadGameObject());
        return data;
    }
}