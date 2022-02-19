using UnityEngine;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private TMP_Text counter;

    public void Set(InventoryItem item)
    {
        if (item != null)
        {
            text.text = item.data.displayName;
            counter.text = item.stackSize.ToString();

        }
        else
        {
            text.text = "";
            counter.text = "";
        }
    }
}
