using Mirror;
using TMPro;
using System.Collections.Generic;
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

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(CanvasController.displayName);
    }

    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);

        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);

        UpdateDisplay();
    }

    public void OnLeaveButtonPress()
    {
        if (!isServer)
        {
            Debug.Log("stopping client");
            Room.StopClient();
        }
        else
        {
            Debug.Log("stopping server");
            Room.StopHost();
        }
    }

    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    public void StartGame()
    {
        if (IsLeader)
        {
            Room.StartGame();
        }
    }

    public void UpdateDisplay()
    {
        // Look for own GUI
        if (!hasAuthority)
        {
            if (canvas)
                canvas.SetActive(false);
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
        canvas.SetActive(true);

        startButton.SetActive(IsLeader);

        Debug.Log("updating display for " + Room.RoomPlayers.Count + " players");

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