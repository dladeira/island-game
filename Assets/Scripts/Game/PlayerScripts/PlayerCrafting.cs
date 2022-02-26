using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerCrafting : NetworkBehaviour
{
    [SerializeField] GameObject craftingPanel;

    void Start()
    {
        ToggleOpen(false);
    }

    public void ToggleOpen(bool open)
    {
        craftingPanel.SetActive(open);
    }
}
