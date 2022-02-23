using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using TMPro;

public class NetworkGamePlayerIsland : NetworkBehaviour
{
    [SyncVar] public string displayName;

    [Scene] [SerializeField] private string lobbyScene;

    [Header("Helper Scripts")]
    [SerializeField] private PlayerMovement movement;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Inventory")]
    [SerializeField] public PlayerHotbar hotbar;
    [SerializeField] public PlayerInventory inventory;
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
                inventory.ToggleInventory(inventoryOpen);
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

    public void DropItem(string itemID, IGameInventory inventory)
    {
        CmdDropItem(itemID, playerCamera.position + (playerCamera.forward * 0.2f) - (playerCamera.up * 0.2f), inventory.GetName());
    }

    [Command]
    private void CmdPickupItem(ItemObject item, PlayerInventory inventory)
    {
        RpcPickupItem(item, inventory);
    }

    [Command]
    private void CmdDropItem(string itemId, Vector3 position, string inventoryName)
    {
        ItemObject itemObject = (NetworkManager.singleton as NetworkManagerIsland).IdToItem(itemId).itemObjectPrefab.GetComponent<ItemObject>();
        ItemObject spawnedItem = Instantiate(itemObject, position, Quaternion.Euler(0, 0, 0));
        NetworkServer.Spawn(spawnedItem.gameObject);

        RpcDropItem(itemId, inventoryName);
    }

    [ClientRpc]
    private void RpcPickupItem(ItemObject item, PlayerInventory inventory)
    {
        item.OnHandlePickupItem(inventory);
    }

    [ClientRpc]
    private void RpcDropItem(string itemId, string inventoryName)
    {
        InventoryItemData item = (NetworkManager.singleton as NetworkManagerIsland).IdToItem(itemId);
        switch (inventoryName)
        {
            case "PlayerInventory":
                inventory.Remove(item, 1);
                break;
            case "PlayerHotbar":
                hotbar.Remove(item, 1);
                break;
        }
    }
}
