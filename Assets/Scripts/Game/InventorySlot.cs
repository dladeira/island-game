using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    // Item to drag
    [SerializeField] private RectTransform itemToMove;
    [SerializeField] private CanvasGroup canvasGroup;

    // Display
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private TMP_Text displayCounter;
    [SerializeField] private Image displayImage;
    [SerializeField] private Image displayEquippedBackground;

    // Player (or loaded from player)
    private NetworkGamePlayerIsland player;
    private Canvas canvas;

    // Other info
    private InventoryItem item;
    private bool equipped;
    private int id;

    void Start()
    {
        // Reset item slot on start
        ClearItem();
        SetEquipped(false);
    }

    public void SetPlayer(NetworkGamePlayerIsland player)
    {
        this.player = player;
        this.canvas = player.canvas;
    }

    public void SetItem(InventoryItem item)
    {
        displayText.text = item.data.displayName;
        displayCounter.text = item.stackSize.ToString();

        displayImage.gameObject.SetActive(true);
        displayImage.sprite = item.data.icon;

        this.item = item;
    }

    public void ClearItem()
    {
        displayText.text = "";
        displayCounter.text = "";

        displayImage.gameObject.SetActive(false);

        this.item = null;
    }

    public InventoryItem GetItem()
    {
        return item;
    }

    public void SetId(int id)
    {
        this.id = id;
    }

    public int GetId()
    {
        return id;
    }

    public void SetEquipped(bool equipped)
    {
        this.equipped = equipped;

        displayEquippedBackground.gameObject.SetActive(equipped);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item != null && eventData.button == PointerEventData.InputButton.Right)
        {
            player.DropItem(id, Input.GetKey(KeyCode.LeftShift));
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
        itemToMove.anchoredPosition = new Vector2(0, 0);
        canvasGroup.blocksRaycasts = true;
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

        // If a inventory slot has been dragged on this slot and it has a item
        if (sourceSlot && sourceSlot.item != null)
        {
            InventoryItem draggedItem = sourceSlot.item;
            int count = draggedItem.stackSize; // Use in seperate variable because stackSize is affected when removing items

            if (this.item != null)
                sourceSlot.SetItem(this.item);
            else
                sourceSlot.ClearItem();
            SetItem(draggedItem);
        }
    }
}
