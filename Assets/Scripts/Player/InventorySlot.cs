using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private TMP_Text counter;
    [SerializeField] private Image image;
    NetworkGamePlayerIsland player;
    InventoryItem item;

    public void Set(InventoryItem item, NetworkGamePlayerIsland player)
    {
        this.item = item;
        this.player = player;
        if (item != null)
        {
            text.text = item.data.displayName;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item != null)
        {
            player.DropItem(item.data.id);
        }
    }
}
