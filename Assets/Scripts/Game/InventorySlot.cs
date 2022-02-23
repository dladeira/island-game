using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
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

    public void Set(InventoryItem item, NetworkGamePlayerIsland player, IGameInventory sourceInventory)
    {
        this.player = player;
        this.inventory = sourceInventory;
        this.canvas = player.canvas;

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
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item != null && eventData.button == PointerEventData.InputButton.Right)
        {
            player.DropItem(item.data.id);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            Debug.Log("OnBeginDrag");
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            Debug.Log("OnEndDrag");
            itemToMove.anchoredPosition = new Vector2(0, 0);
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            Debug.Log("OnDrag");
            canvasGroup.alpha = 1;
            itemToMove.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }


    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        GameObject item = eventData.pointerDrag;
        InventorySlot sourceSlot = item.GetComponent<InventorySlot>();
        InventoryItem draggedItem = sourceSlot.item;

        if (sourceSlot)
        {
            Debug.Log(sourceSlot.player.name);
            sourceSlot.inventory.Remove(draggedItem.data, 1);
            inventory.Add(draggedItem.data, 1);
        }
    }
}
