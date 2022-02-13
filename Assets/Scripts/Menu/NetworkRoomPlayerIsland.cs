using Mirror;
using TMPro;
using UnityEngine;

public class NetworkRoomPlayerIsland : NetworkBehaviour
{
    [SyncVar] public bool IsLeader;
    [SyncVar] public int id;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))] public string DisplayName = "Player";

    [SerializeField] private GameObject canvas;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[3];
    [SerializeField] private GameObject startButton;

    private NetworkManagerIsland room;
    private NetworkManagerIsland Room
    {
        get
        {
            if (room != null)
                return room;
            return room = NetworkManager.singleton as NetworkManagerIsland;
        }
    }

    // If this is the player we own then set our name
    public override void OnStartAuthority()
    {
        CmdSetDisplayName(CanvasController.displayName);
    }

    public void OnLeaveButtonPress()
    {
        if (!isServer)
        {
            Room.StopClient();
        }
        else
        {
            Room.StopHost();
        }
    }

    // This method is called on the client that changes the name
    public void HandleDisplayNameChanged(string oldValue, string newValue)
    {
            // Look for own display
            foreach (var player in Room.RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }
            return;
    }

    public void StartGame()
    {
        if (IsLeader)
        {
            Room.StartGame();
        }
    }

    // Called on every single client that connects
    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);

        UpdateDisplay();
    }

    // Called on every single client that disconnects
    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);

        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        canvas.SetActive(hasAuthority);

        if (!hasAuthority)
        {
            return;
        }

        startButton.SetActive(IsLeader);

        // Clear all names
        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Empty";
        }

        // Set names
        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;

        }
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }
}