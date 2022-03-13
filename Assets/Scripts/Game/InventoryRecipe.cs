using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryRecipe : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject inputObject;
    [SerializeField] private GameObject outputObject;
    [SerializeField] private GameObject ItemSlotPrefab;

    private PlayerManager player;
    private InventoryRecipeData data;
    private bool mouseOver;

    void Start()
    {
        foreach (InventoryItem item in data.GetInputItems())
        {
            InventorySlot slot = Instantiate(ItemSlotPrefab, inputObject.transform).GetComponent<InventorySlot>();
            slot.SetInteractable(false);
            slot.SetItem(item);
        }

        foreach (InventoryItem item in data.GetOutputItems())
        {
            InventorySlot slot = Instantiate(ItemSlotPrefab, outputObject.transform).GetComponent<InventorySlot>();
            slot.SetInteractable(false);
            slot.SetItem(item);
        }
    }

    public void Initialize(PlayerManager player, InventoryRecipeData data)
    {
        this.player = player;
        this.data = data;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && mouseOver)
        {
            player.inventory.CraftRecipe(data);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
    }
}
