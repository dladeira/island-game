using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerIsland : NetworkManager
{
    public enum GameState
    {
        LOBBY, PLAYING
    }

    [Header("Config")]
    [Scene] [SerializeField] private string lobbyScene;
    [Scene] [SerializeField] private string gameScene;
    [SerializeField] public static int maxPlayers = 4;

    [Header("Prefabs")]
    [SerializeField] private NetworkRoomPlayerIsland roomPlayerPrefab;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public List<NetworkRoomPlayerIsland> RoomPlayers { get; } = new List<NetworkRoomPlayerIsland>();

    public override void Start()
    {
        DontDestroyOnLoad(this);
        base.Start();
    }

    // ===== Network

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (numPlayers >= maxPlayers)
        {
            conn.Disconnect();
            return;
        }

        if (SceneManager.GetActiveScene().path != lobbyScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Debug.Log("adding player");
        if (SceneManager.GetActiveScene().path == lobbyScene)
        {
            NetworkRoomPlayerIsland roomPlayer = Instantiate(roomPlayerPrefab);
            roomPlayer.IsLeader = RoomPlayers.Count == 0;
            roomPlayer.id = RoomPlayers.Count;

            NetworkServer.AddPlayerForConnection(conn, roomPlayer.gameObject);
            SendPlayerList();
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().path == lobbyScene)
        {
            if (conn.identity != null)
            {
                NetworkRoomPlayerIsland roomPlayer = conn.identity.GetComponent<NetworkRoomPlayerIsland>();
                RoomPlayers.Remove(roomPlayer);
                SendPlayerList();
            }
            base.OnServerDisconnect(conn);
        }
        else
        {
            Destroy(conn.identity);
            base.OnServerDisconnect(conn);
        }
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        OnClientDisconnected?.Invoke();
    }


    public void SendPlayerList()
    {
        Debug.Log("sending the player list");
        foreach (var player in RoomPlayers)
        {
            player.UpdateDisplay();
        }
    }
}