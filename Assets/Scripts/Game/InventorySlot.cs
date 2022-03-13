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
    private PlayerManager player;
    private Canvas canvas;

    // Other info
    private InventoryItem item;
    private bool equipped;
    private bool interactable;
    private int id;

    public void Initialize(PlayerManager player, int id)
    {
        this.player = player;
        this.canvas = player.canvas;
        this.id = id;

        SetItem(null);
        SetEquipped(false);
        SetInteractable(true);
    }


    public void SetItem(InventoryItem item)
    {
        if (item != null)
        {
            SetDisplay(item.data.displayName, item.stackSize.ToString(), item.data.icon);
            this.item = item;
        }
        else
        {
            SetDisplay("", "", null);
            this.item = null;
        }
    }

    public void SetInteractable(bool interactable)
    {
        this.interactable = interactable;
    }

    private void SetDisplay(string name, string counter, Sprite image)
    {
        this.displayText.text = name;
        this.displayCounter.text = counter;
        this.displayImage.sprite = image;
        displayImage.gameObject.SetActive(image != null);
    }

    public InventoryItem GetItem()
    {
        return item;
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
        if (interactable && item != null && eventData.button == PointerEventData.InputButton.Right)
        {
            player.DropItem(id, Input.GetKey(KeyCode.LeftShift));
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (interactable && item != null)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (interactable)
        {
            itemToMove.anchoredPosition = new Vector2(0, 0);
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (interactable && item != null)
        {
            canvasGroup.alpha = 1;
            itemToMove.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }


    public void OnDrop(PointerEventData eventData)
    {
        if (interactable)
        {
            GameObject movingItem = eventData.pointerDrag;
            InventorySlot sourceSlot = movingItem.GetComponent<InventorySlot>();

            // If a inventory slot has been dragged on this slot and it has a item
            if (sourceSlot && sourceSlot.item != null)
            {
                InventoryItem draggedItem = sourceSlot.item;

                if (this.item != null)
                    sourceSlot.SetItem(this.item);
                else
                    sourceSlot.SetItem(null);

                SetItem(draggedItem);
                player.inventory.UpdateInventory();
            }
        }
    }
}
