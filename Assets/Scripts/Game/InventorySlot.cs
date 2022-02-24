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
    public InventoryRecipeData recipe;
    public int id;
    public bool interactable;
    public bool crafting;

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
        this.interactable = true;
        this.crafting = false;

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
            this.item = null;

            text.text = "";
            counter.text = "";
            image.enabled = false;
        }
    }

    public void Set(InventoryItem item, NetworkGamePlayerIsland player, int test, bool interactable, InventoryRecipeData recipe)
    {
        this.canvas = player.canvas;
        this.interactable = interactable;
        this.crafting = true;
        this.recipe = recipe;
        this.player = player;


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
            this.item = null;

            text.text = "";
            counter.text = "";
            image.enabled = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item != null && eventData.button == PointerEventData.InputButton.Right && interactable)
        {
            player.DropItem(item.data.id, inventory);
        }
        else if (crafting && recipe != null)
        {
            player.CraftItem(recipe);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null && interactable)
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
        if (item != null && interactable)
        {
            canvasGroup.alpha = 1;
            itemToMove.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }


    public void OnDrop(PointerEventData eventData)
    {
        GameObject movingItem = eventData.pointerDrag;
        InventorySlot sourceSlot = movingItem.GetComponent<InventorySlot>();

        if (sourceSlot && interactable)
        {
            InventoryItem draggedItem = sourceSlot.item;
            int count = draggedItem.stackSize; // Use in seperate variable because stackSize is affected when removing items

            sourceSlot.inventory.Remove(draggedItem.data, count);
            inventory.Add(draggedItem.data, count, id);
        }
    }
}
