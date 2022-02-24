using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Mirror;

[CreateAssetMenu(menuName= "Recipe Item data")]
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

    public void Set(InventoryItemData input, int inputAmount, InventoryItemData output, int outputAmount)
    {
        this.input = new List<InventoryItemData>();
        this.input.Add(input);
        this.inputAmount = new List<int>();
        this.inputAmount.Add(inputAmount);

        this.output = new List<InventoryItemData>();
        this.output.Add(output);
        this.outputAmount = new List<int>();
        this.outputAmount.Add(outputAmount);
    }
}

// <summary>A extension class used for serialization/deserialization over Mirror (network)</summary>
public static class InventoryRecipeDataReadWriteFunctions 
{
    public static void WriteMyType(this NetworkWriter writer, InventoryRecipeData value)
    {
        writer.Write<InventoryItemData>(value.input.ToArray()[0]);
        writer.Write<int>(value.inputAmount.ToArray()[0]);
        writer.Write<InventoryItemData>(value.output.ToArray()[0]);
        writer.Write<int>(value.outputAmount.ToArray()[0]);
    }

    public static InventoryRecipeData ReadMyType(this NetworkReader reader)
    {
        InventoryRecipeData data = ScriptableObject.CreateInstance("InventoryRecipeData") as InventoryRecipeData;
        data.Set(reader.Read<InventoryItemData>(), reader.Read<int>(), reader.Read<InventoryItemData>(), reader.Read<int>());
        return data;
    }
}