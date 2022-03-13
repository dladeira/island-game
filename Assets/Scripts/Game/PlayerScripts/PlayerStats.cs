using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerStats : NetworkBehaviour
{
    public int hp { get; private set; }
    public int hunger { get; private set; }
    public int thirst { get; private set; }

    [Header("Canvas")]
    [SerializeField] private Image hpBar;
    [SerializeField] private Image hungerBar;
    [SerializeField] private Image thirstBar;

    void Start()
    {
        hp = 100;
        hunger = 100;
        thirst = 100;
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.fillAmount = (float) hp / 100;
        hungerBar.fillAmount = (float) hunger / 100;
        thirstBar.fillAmount = (float) thirst / 100;
    }

    public bool SetHp(int hp)
    {
        this.hp = Mathf.Clamp(hp, 0, 100);
        // KILL PlAYER
        return hp < 0;
    }

    public void SetHunger(int hunger)
    {
        this.hunger = Mathf.Clamp(hunger, 0, 100);
    }

    public void SetThirst(int thirst)
    {
        this.thirst = Mathf.Clamp(thirst, 0, 100);
    }
}
