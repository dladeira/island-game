using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    public void Set(InventoryItem item)
    {
        if (item != null)
        {
            text.text = item.data.displayName;
        }
        else
        {
            text.text = "";
        }
    }
}
