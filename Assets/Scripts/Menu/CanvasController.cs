using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasController : MonoBehaviour
{
    public enum Menu
    {
        NameInput, MainMenu, Lobby
    }

    [SerializeField] private GameObject nameInputObj;
    [SerializeField] private GameObject mainMenuObj;
    [SerializeField] private GameObject lobbyObj;

    [Header("NameLock")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button nameLockBtn;

    [Header("MainMenu")]
    [SerializeField] private NetworkManagerIsland networkManager;

    public static string displayName { get; private set; }

    public void OnEnable()
    {
        NetworkManagerIsland.OnClientConnected += HandleClientConnected;
        NetworkManagerIsland.OnClientDisconnected += HandleClientDisconnected;
    }

    public void OnDisable()
    {
        NetworkManagerIsland.OnClientConnected -= HandleClientConnected;
        NetworkManagerIsland.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void Start()
    {
        SetMenu(Menu.NameInput);
        UpdateNameButton();
    }

    public void HandleClientConnected()
    {
        SetMenu(Menu.Lobby);
    }

    public void HandleClientDisconnected()
    {
        SetMenu(Menu.MainMenu);
    }

    // ==== NameInput

    public void UpdateNameButton()
    {
        nameLockBtn.interactable = !string.IsNullOrWhiteSpace(nameInput.text);
    }

    public void LockPlayerName()
    {
        displayName = nameInput.text;
        SetMenu(Menu.MainMenu);
    }

    // ==== MainMenu

    public void OnHostButtonPress()
    {
        networkManager.StartHost();
        SetMenu(Menu.Lobby);
    }

    public void OnJoinButtonPress()
    {
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
        SetMenu(Menu.Lobby);
    }

    public void SetMenu(Menu menu)
    {
        switch (menu)
        {
            case Menu.NameInput:
                nameInputObj.SetActive(true);
                mainMenuObj.SetActive(false);
                lobbyObj.SetActive(false);
                return;
            case Menu.MainMenu:
                nameInputObj.SetActive(false);
                mainMenuObj.SetActive(true);
                lobbyObj.SetActive(false);
                return;
            case Menu.Lobby:
                nameInputObj.SetActive(false);
                mainMenuObj.SetActive(false);
                lobbyObj.SetActive(true);
                return;
        }
    }
}
