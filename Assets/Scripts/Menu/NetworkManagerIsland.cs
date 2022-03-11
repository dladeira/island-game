using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerIsland : NetworkManager
{
    // <summary>The state of the game</summary>
    public enum GameState
    {
        // <summary>The lobby phase</summary>
        LOBBY,
        // <summary>The game has started and people are playing</summary>
        PLAYING
    }

    [Header("Config")]
    [Scene] [SerializeField] private string lobbyScene;
    [Scene] [SerializeField] private string gameScene;
    [SerializeField] public static int maxPlayers = 4; // TODO Create username fields in the lobby dynamically based upon max player count

    [Header("Prefabs")]
    [SerializeField] private NetworkRoomPlayerIsland roomPlayerPrefab;
    [SerializeField] private PlayerManager gamePlayerPrefab;

    [Header("Items")]
    [SerializeField] private List<InventoryItemData> itemDatas;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public List<NetworkRoomPlayerIsland> RoomPlayers { get; } = new List<NetworkRoomPlayerIsland>();
    public List<PlayerManager> GamePlayers { get; } = new List<PlayerManager>();

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

    // ===== Network

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (numPlayers >= maxPlayers)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().path == lobbyScene)
        {
            NetworkRoomPlayerIsland roomPlayer = Instantiate(roomPlayerPrefab);
            roomPlayer.IsLeader = RoomPlayers.Count == 0;
            roomPlayer.id = RoomPlayers.Count;

            NetworkServer.AddPlayerForConnection(conn, roomPlayer.gameObject);
            SendPlayerList();
        }
        else if (SceneManager.GetActiveScene().path == gameScene)
        {
            PlayerManager playerInstance = Instantiate(gamePlayerPrefab, new Vector3(10, 10, 10), Quaternion.Euler(0, 0, 0));
            NetworkServer.Spawn(playerInstance.gameObject);
            playerInstance.displayName = ("FAILED");
            Debug.Log(conn.connectionId);
            NetworkServer.AddPlayerForConnection(conn, playerInstance.gameObject);
            GamePlayers.Add(playerInstance);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().path == lobbyScene)
        {
            NetworkRoomPlayerIsland roomPlayer = conn.identity.GetComponent<NetworkRoomPlayerIsland>();
            RoomPlayers.Remove(roomPlayer);
            SendPlayerList();

            base.OnServerDisconnect(conn);
        }
        else
        {
            Destroy(conn.identity);
            base.OnServerDisconnect(conn);
        }
    }
    public override void OnStopServer()
    {
        OnClientDisconnected?.Invoke();
        base.OnStopServer();
    }

    public override void OnClientConnect()
    {
        OnClientConnected?.Invoke();
        base.OnClientConnect();
    }

    public override void OnClientDisconnect()
    {
        OnClientDisconnected?.Invoke();
        RoomPlayers.Clear();
        GamePlayers.Clear();
        base.OnClientDisconnect();
    }

    public override void OnClientError(Exception exception)
    {
        Debug.Log("error");
    }

    public void SendPlayerList()
    {
        foreach (var player in RoomPlayers)
        {
            player.UpdateDisplay();
        }
    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().path == lobbyScene)
        {
            ServerChangeScene(gameScene);
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        // From Lobby to Game
        if (SceneManager.GetActiveScene().path == lobbyScene && newSceneName == gameScene)
        {
            for (int i = 0; RoomPlayers.Count > i; i++)
            {
                var conn = RoomPlayers[i].connectionToClient;
                PlayerManager playerInstance = Instantiate(gamePlayerPrefab, new Vector3(10, 10, 10), Quaternion.Euler(0, 0, 0));
                NetworkServer.ReplacePlayerForConnection(conn, playerInstance.gameObject);
                playerInstance.displayName = (RoomPlayers[i].DisplayName);
                GamePlayers.Add(playerInstance);
            }
            base.ServerChangeScene(newSceneName);
            return;
        }

        base.ServerChangeScene(newSceneName);
    }

    public InventoryItemData IdToItem(string itemId)
    {
        for (int i = 0; i < itemDatas.Count; i++)
        {
            if (itemDatas[i].id == itemId)
            {
                return itemDatas[i];

            }
        }

        return null;
    }
}