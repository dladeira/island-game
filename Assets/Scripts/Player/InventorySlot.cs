using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private TMP_Text counter;
    [SerializeField] private Image image;

    public void Set(InventoryItem item)
    {
        if (item != null)
        {
            text.text = item.data.displayName;
            counter.text = item.stackSize.ToString();
            counter.text = item.stackSize.ToString();
            image.enabled = true;
            image.sprite = item.data.icon;

        }
        else
        {
            text.text = "";
            counter.text = "";
            image.enabled = false;
        }
    }
}
