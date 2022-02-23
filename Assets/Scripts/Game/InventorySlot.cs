using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    private Canvas canvas;
    [SerializeField] private RectTransform itemToMove;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private TMP_Text text;
    [SerializeField] private TMP_Text counter;
    [SerializeField] private Image image;

    public NetworkGamePlayerIsland player;
    public InventoryItem item;
    public IGameInventory inventory;
    public int id;

    public void Set(InventoryItem item, NetworkGamePlayerIsland player, IGameInventory sourceInventory)
    {
        Set(item, player, sourceInventory, 0);
    }

    public void Set(InventoryItem item, NetworkGamePlayerIsland player, IGameInventory sourceInventory, int id)
    {
        this.player = player;
        this.inventory = sourceInventory;
        this.canvas = player.canvas;
        this.id = id;

        if (item != null)
        {
            this.item = item;

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
            item = null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item != null && eventData.button == PointerEventData.InputButton.Right)
        {
            player.DropItem(item.data.id, inventory);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            itemToMove.anchoredPosition = new Vector2(0, 0);
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            canvasGroup.alpha = 1;
            itemToMove.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }


    public void OnDrop(PointerEventData eventData)
    {
        GameObject movingItem = eventData.pointerDrag;
        InventorySlot sourceSlot = movingItem.GetComponent<InventorySlot>();

        if (sourceSlot)
        {
            InventoryItem draggedItem = sourceSlot.item;

            inventory.Add(draggedItem.data, draggedItem.stackSize, id);
            sourceSlot.inventory.Remove(draggedItem.data, draggedItem.stackSize);
        }
    }
}
