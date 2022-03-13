using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    [SyncVar] public string displayName;

    [Scene][SerializeField] private string lobbyScene;
    [SerializeField] public Transform playerCamera;

    [Header("Helper Scripts")]
    [SerializeField] public PlayerMovement movement;
    [SerializeField] public PlayerInventory inventory;
    [SerializeField] public PlayerCrafting crafting;
    [SerializeField] public PlayerStats stats;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] public Canvas canvas;

    private NetworkManagerIsland nm;
    private NetworkManagerIsland actualNm
    {
        get
        {
            if (nm != null)
                return nm;
            return nm = NetworkManager.singleton as NetworkManagerIsland;
        }
    }

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

    // Called on every single client that connects
    public override void OnStartClient()
    {
        Debug.Log("started client");
        actualNm.GamePlayers.Add(this);
    }

    // Called on every single client that connects
    public override void OnStopClient()
    {
        Debug.Log("stopped client");
        actualNm.GamePlayers.Remove(this);
    }

    void Update()
    {
        DoButtons();

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
                crafting.ToggleOpen(inventoryOpen);
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

    public void DropItem(int slotId, bool dropEntireStack)
    {
        CmdDropItem(slotId, playerCamera.position + (playerCamera.forward * 0.2f) - (playerCamera.up * 0.2f), dropEntireStack);
    }
    [Command]
    private void CmdDropItem(int slotId, Vector3 position, bool dropEntireStack)
    {
        InventoryItem items = inventory.GetSlot(slotId);

        // Spawn dropped items
        for (int i = 0; i < (dropEntireStack ? items.stackSize : 1); i++)
        {
            GameObject spawnedItem = Instantiate(items.data.itemObjectPrefab, position, Quaternion.Euler(0, 0, 0));
            NetworkServer.Spawn(spawnedItem.gameObject);
        }

        RpcDropItem(slotId, dropEntireStack);
    }

    [ClientRpc]
    private void RpcDropItem(int slotId, bool dropEntireStack)
    {
        if (dropEntireStack)
        {
            inventory.SetSlot(slotId, null);
        }
        else
        {
            inventory.ModifySlot(slotId, -1);
        }
    }
}
