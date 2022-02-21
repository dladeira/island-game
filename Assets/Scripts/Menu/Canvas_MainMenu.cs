using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Canvas_MainMenu : MonoBehaviour
{
    [SerializeField] Button joinButton;

    private bool connecting;

    public void OnEnable()
    {
        SetConnecting(false);
        NetworkManagerIsland.OnClientDisconnected += HandleClientDisconnected;
    }

    public void OnDisable()
    {
        NetworkManagerIsland.OnClientDisconnected -= HandleClientDisconnected;
    }

    private NetworkManagerIsland networkManager
    {
        get
        {
            return NetworkManager.singleton as NetworkManagerIsland;
        }
    }

    public void OnHostButtonPress()
    {
        networkManager.StartHost();
        CanvasController.Instance.SetMenu(CanvasController.MenuState.Lobby);
    }

    public void OnJoinButtonPress()
    {
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
        SetConnecting(true);
    }

    public void HandleClientDisconnected()
    {
        SetConnecting(false);
    }

    public void SetConnecting(bool value)
    {
        joinButton.interactable = !value;
        connecting = value;
    }
}
