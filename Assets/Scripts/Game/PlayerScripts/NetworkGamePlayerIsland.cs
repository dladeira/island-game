using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using TMPro;

public class NetworkGamePlayerIsland : NetworkBehaviour
{
    [SyncVar] public string displayName;

    [Scene][SerializeField] private string lobbyScene;

    [Header("Helper Scripts")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] public PlayerInventory inventory;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Inventory")]
    // [SerializeField] public PlayerCrafting crafting;
    [SerializeField] private TMP_Text pickupText;
    [SerializeField] private float lookDistance;
    [SerializeField] private LayerMask pickupMask;

    [SerializeField] private Transform playerCamera;
    [SerializeField] public Canvas canvas;

    bool inventoryOpen = false;

    void Start()
    {
        if (hasAuthority)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        DontDestroyOnLoad(this);
    }

    void OnEnable()
    {
        NetworkManagerIsland.OnClientDisconnected += ReturnToMainMenu;
    }

    void OnDisable()
    {
        NetworkManagerIsland.OnClientDisconnected -= ReturnToMainMenu;
    }

    void Update()
    {
        DoButtons();
        DoItemPickup();

        if (hasAuthority)
        {
            CmdUpdateAnimations(movement.GetVelocity().magnitude > 0.5, movement.isJumping, movement.isGrounded, movement.isCrouching);
        }
    }

    [Command]
    private void CmdUpdateAnimations(bool isRunning, bool isJumping, bool isGrounded, bool isCrouching)
    {
        RpcUpdateAnimations(isRunning, isJumping, isGrounded, isCrouching);
    }

    private void DoButtons()
    {
        if (hasAuthority)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                inventoryOpen = !inventoryOpen;

                Cursor.lockState = inventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;

                inventory.ToggleOpen(inventoryOpen);
                // crafting.ToggleOpen(inventoryOpen);
                movement.ToggleMovement(!inventoryOpen);
                movement.ToggleLook(!inventoryOpen);
            }
        }
    }

    void ReturnToMainMenu()
    {
        SceneManager.LoadScene(lobbyScene);
    }

    // =====
    // Animations
    // =====

    [ClientRpc]
    private void RpcUpdateAnimations(bool isRunning, bool isJumping, bool isGrounded, bool isCrouching)
    {
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsCrouching", isCrouching);
    }

    // =====
    // Inventory
    // =====

    public void DoItemPickup()
    {
        if (hasAuthority)
        {
            RaycastHit hit;

            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, lookDistance, pickupMask))
            {
                ItemObject item = hit.transform.GetComponent<ItemObject>();
                pickupText.transform.gameObject.SetActive(true);
                pickupText.text = "Pickup '" + item.referenceItem.displayName + "'";

                if (Input.GetKeyDown(KeyCode.E))
                {
                    CmdPickupItem(item, inventory);
                }
            }
            else
            {
                pickupText.transform.gameObject.SetActive(false);
            }

        }
        else
        {
            pickupText.transform.gameObject.SetActive(false);
        }
    }

    public void DropItem(int slotId, bool dropEntireStack)
    {
        CmdDropItem(slotId, playerCamera.position + (playerCamera.forward * 0.2f) - (playerCamera.up * 0.2f), dropEntireStack);
    }

    // public void CraftItem(InventoryRecipeData data)
    // {
    //     CmdCraftItem(data);
    // }

    [Command]
    private void CmdPickupItem(ItemObject item, PlayerInventory inventory)
    {
        RpcPickupItem(item, inventory);
    }

    // [Command]
    // private void CmdCraftItem(InventoryRecipeData data)
    // {
    //     RpcCraftItem(data);
    // }

    [Command]
    private void CmdDropItem(int slotId, Vector3 position, bool dropEntireStack)
    {
        InventoryItem items = inventory.GetSlot(slotId);

        // Spawn dropped items
        for (int i = 0; i < (dropEntireStack ? items.stackSize : 1); i++)
        {
            ItemObject itemObject = items.data.itemObjectPrefab.GetComponent<ItemObject>();
            ItemObject spawnedItem = Instantiate(itemObject, position, Quaternion.Euler(0, 0, 0));
            NetworkServer.Spawn(spawnedItem.gameObject);
        }

        RpcDropItem(slotId, dropEntireStack);
    }

    [ClientRpc]
    private void RpcPickupItem(ItemObject item, PlayerInventory inventory)
    {
        item.OnHandlePickupItem(inventory);
    }

    [ClientRpc]
    private void RpcDropItem(int slotId, bool dropEntireStack)
    {
        if (dropEntireStack)
        {
            inventory.SetSlot(slotId, null);
        }
        else {
            inventory.ModifySlot(slotId, -1);
        }
    }

    // [ClientRpc]
    // private void RpcCraftItem(InventoryRecipeData data)
    // {
    //     List<InventoryItemData> itemsToRemove = new List<InventoryItemData>(data.input);
    //     List<InventoryItemData> itemsToHave = new List<InventoryItemData>(data.input);

    //     for (int haveIndex = data.input.Count - 1; haveIndex >= 0; haveIndex--)
    //     {
    //         if (fakeInventory.Has(data.input[haveIndex], data.inputAmount[haveIndex]))
    //         {
    //             itemsToHave.RemoveAt(haveIndex);
    //         }
    //         else if (hotbar.Has(data.input[haveIndex], data.inputAmount[haveIndex]))
    //         {
    //             itemsToHave.RemoveAt(haveIndex);
    //         }
    //     }


    //     if (itemsToHave.Count <= 0)
    //     {
    //         for (int removeIndex = data.input.Count - 1; removeIndex >= 0; removeIndex--)
    //         {
    //             if (fakeInventory.Remove(data.input[removeIndex], data.inputAmount[removeIndex]))
    //             {
    //                 itemsToRemove.RemoveAt(removeIndex);
    //             }
    //             else if (hotbar.Remove(data.input[removeIndex], data.inputAmount[removeIndex]))
    //             {
    //                 itemsToRemove.RemoveAt(removeIndex);
    //             }
    //         }
    //         for (int addIndex = 0; addIndex < data.output.Count; addIndex++)
    //         {
    //             Debug.Log("adding " + data.output[addIndex]);
    //             fakeInventory.Add(data.output[addIndex], data.outputAmount[addIndex]);
    //         }
    //     }
    // }
}
