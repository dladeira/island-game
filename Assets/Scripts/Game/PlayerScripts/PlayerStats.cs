using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class PlayerStats : NetworkBehaviour
{
    [SerializeField] private PlayerManager player;

    [Header("UI")]
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private GameObject usernameTextPrefab;
    [SerializeField] private Image hpBar;
    [SerializeField] private Image hungerBar;
    [SerializeField] private Image thirstBar;

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

    public int hp { get; private set; }
    public int hunger { get; private set; }
    public int thirst { get; private set; }

    void Start()
    {
        hp = 100;
        hunger = 100;
        thirst = 100;
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.fillAmount = (float)hp / 100;
        hungerBar.fillAmount = (float)hunger / 100;
        thirstBar.fillAmount = (float)thirst / 100;
    }

    public void LateUpdate()
    {
        worldCanvas.gameObject.SetActive(hasAuthority);

        if (hasAuthority)
        {
            foreach (Transform child in worldCanvas.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (PlayerManager player in actualNm.GamePlayers)
            {
                if (player.displayName != this.player.displayName && SeenByCamera(player.gameObject) && Vector3.Distance(this.player.transform.position, player.transform.position) < 40)
                {
                    GameObject text = Instantiate(usernameTextPrefab.transform, worldCanvas.transform).gameObject;
                    Vector3 screenPos = this.player.playerCamera.GetComponent<Camera>().WorldToScreenPoint(player.transform.position + new Vector3(0, 2.4f, 0));
                    text.GetComponent<TMP_Text>().text = player.displayName;
                    text.GetComponent<RectTransform>().anchoredPosition = screenPos;
                }
            }

            foreach (LivingStructure structure in player.hotbar.hitStructures)
            {
                if (structure && SeenByCamera(structure.gameObject) && Vector3.Distance(this.player.transform.position, structure.transform.position) < 15)
                {
                    GameObject text = Instantiate(usernameTextPrefab.transform, worldCanvas.transform).gameObject;
                    Vector3 screenPos = this.player.playerCamera.GetComponent<Camera>().WorldToScreenPoint(structure.transform.position + new Vector3(0, 2.4f, 0));
                    text.GetComponent<TMP_Text>().text = structure.health.ToString();
                    text.GetComponent<RectTransform>().anchoredPosition = screenPos;
                }
            }
        }
    }

    public bool SeenByCamera(GameObject gameObject)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(this.player.playerCamera.GetComponent<Camera>());
        if (GeometryUtility.TestPlanesAABB(planes, gameObject.GetComponent<Collider>().bounds))
            return true;
        else
            return false;
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
