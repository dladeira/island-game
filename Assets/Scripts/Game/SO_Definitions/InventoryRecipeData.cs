using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Mirror;

[CreateAssetMenu(menuName = "Recipe Item data")]
public class InventoryRecipeData : ScriptableObject
{
    public List<InventoryItemData> input;
    public List<int> inputAmount;
    public List<InventoryItemData> output;
    public List<int> outputAmount;

    public void Set(List<InventoryItemData> input, List<int> inputAmount, List<InventoryItemData> output, List<int> outputAmount)
    {
        this.input = input;
        this.inputAmount = inputAmount;
        this.output = output;
        this.outputAmount = outputAmount;
    }
}

// <summary>A extension class used for serialization/deserialization over Mirror (network)</summary>
public static class InventoryRecipeDataReadWriteFunctions
{
    public static void WriteMyType(this NetworkWriter writer, InventoryRecipeData value)
    {
        WriteArray(writer, value.input);
        WriteArray(writer, value.inputAmount);
        WriteArray(writer, value.output);
        WriteArray(writer, value.outputAmount);
    }

    public static InventoryRecipeData ReadMyType(this NetworkReader reader)
    {
        InventoryRecipeData data = ScriptableObject.CreateInstance("InventoryRecipeData") as InventoryRecipeData;
        data.Set(ReadRecipes(reader), ReadIntegers(reader), ReadRecipes(reader), ReadIntegers(reader));
        return data;
    }

    public static List<InventoryItemData> ReadRecipes(NetworkReader reader)
    {
        List<InventoryItemData> data = new List<InventoryItemData>();

        for (int i = 0; i < 3; i++)
        {
            InventoryItemData read = reader.Read<InventoryItemData>();
            if (read.id != "Empty")
            {
                data.Add(read);
            }
        }

        return data;
    }

    public static List<int> ReadIntegers(NetworkReader reader)
    {
        List<int> data = new List<int>();

        for (int i = 0; i < 3; i++)
        {
            int read = reader.Read<int>();
            if (read > 0)
            {
                data.Add(read);
            }
        }

        return data;
    }

    public static void WriteArray(NetworkWriter writer, List<InventoryItemData> data)
    {
        for (int i = 0; i < 3; i++)
        {
            if (i < data.Count)
            {
                writer.Write<InventoryItemData>(data.ToArray()[i]);
            }
            else
            {
                InventoryItemData empty = ScriptableObject.CreateInstance("InventoryItemData") as InventoryItemData;
                empty.SetValues("Empty", "Empty", null, null, null);
                writer.Write<InventoryItemData>(empty);
            }
        }
    }

    public static void WriteArray(NetworkWriter writer, List<int> data)
    {
        for (int i = 0; i < 3; i++)
        {
            if (i < data.Count)
            {
                writer.Write<int>(data.ToArray()[i]);
            }
            else
            {
                writer.Write<int>(-1);
            }
        }
    }
}