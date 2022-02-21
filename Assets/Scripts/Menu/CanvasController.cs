using System.Collections.Generic;
using UnityEngine;
using Mirror;

// <summary>State manager for the main menu of the game</summary>
public class CanvasController : MonoBehaviour
{

    public static CanvasController Instance { get; private set; }

    // <summary>Different states of the menu</summary>
    public enum MenuState
    {
        // <summary>The player is forced to input their name</summary>
        NameInput = 0,
        // <summary>The player can host and connect to games</summary>
        MainMenu = 1,
        // <summary>The player is connected to a game and awaiting start</summary>
        Lobby = 2
    }

    [SerializeField] private List<GameObject> menuStateObjects;

    private NetworkManagerIsland networkManager
    {
        get
        {
            return NetworkManager.singleton as NetworkManagerIsland;
        }
    }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void Start()
    {
        SetMenu(MenuState.NameInput);
    }

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

    public void HandleClientConnected()
    {
        SetMenu(MenuState.Lobby);
    }

    public void HandleClientDisconnected()
    {
        SetMenu(MenuState.MainMenu);
    }

    public void SetMenu(MenuState menu)
    {
        int index = (int)menu;

        // Hide all menuStateObjects except for the current one
        for (int i = 0; i < menuStateObjects.Count; i++)
        {
            if (i == index)
            {
                menuStateObjects[i].SetActive(true);
            }
            else
            {
                menuStateObjects[i].SetActive(false);
            }
        }
    }
}
