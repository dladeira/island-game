using UnityEngine;
using Mirror;
public class PlayerHotbar : NetworkBehaviour
{
    private PlayerManager player;
    [SerializeField] private GameObject equippedParent;

    [SerializeField] Animator anim;

    public bool attackEnabled { get; private set; } = true;

    private int equippedItem = 1;

    void Start()
    {
        this.player = GetComponent<PlayerManager>();
        SetEquippedItem(0);

        player.inventory.onInventoryChangeEvent += ResetEquipped;
    }

    void Update()
    {
        if (hasAuthority)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetEquippedItem(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetEquippedItem(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetEquippedItem(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SetEquippedItem(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SetEquippedItem(4);
            }

            anim.SetBool("IsSwinging", Input.GetMouseButton(0));
            CmdUpdateAnimations(Input.GetMouseButton(0));
        }
    }

    public void SwingWeapon()
    {
        if (hasAuthority && attackEnabled)
        {
            Debug.Log("Swinging weapon");
        }
    }

    void ToggleAttackEnabled(bool attackEnabled)
    {
        this.attackEnabled = attackEnabled;
    }

    void ResetEquipped()
    {
        SetEquippedItem(equippedItem);
    }

    public void SetEquippedItem(int index)
    {
        if (hasAuthority)
        {
            player.inventory.GetSlotObject(equippedItem).SetEquipped(false);
            player.inventory.GetSlotObject(index).SetEquipped(true);

            InventoryItem itemToEquip = player.inventory.GetSlot(index);

            foreach (Transform child in equippedParent.transform)
            {
                Destroy(child.gameObject);
            }

            if (itemToEquip != null)
            {
                GameObject objectToSpawn = Instantiate(itemToEquip.data.equippedPrefab, equippedParent.transform, false);
                objectToSpawn.name = itemToEquip.data.id;
                anim.Play("equip_" + itemToEquip.data.id);
            }
            else
            {
                anim.Play("empty");
            }

            equippedItem = index;

            CmdSetEquippedItem(itemToEquip);
        }
    }

    [Command]
    public void CmdSetEquippedItem(InventoryItem itemToEquip)
    {
        RpcSetEquippedItem(itemToEquip);
    }

    [Command]
    void CmdUpdateAnimations(bool IsSwinging)
    {
        RpcUpdateAnimations(IsSwinging);
    }

    [ClientRpc]
    public void RpcSetEquippedItem(InventoryItem itemToEquip)
    {
        if (!hasAuthority)
        {
            foreach (Transform child in equippedParent.transform)
            {
                Destroy(child.gameObject);
            }


            if (itemToEquip != null)
            {
                GameObject objectToSpawn = Instantiate(itemToEquip.data.equippedPrefab, equippedParent.transform, false);
                objectToSpawn.name = itemToEquip.data.id;
                anim.Play("equip_" + itemToEquip.data.id);
            }
            else
            {
                anim.Play("empty");
            }
        }

    }

    [ClientRpc]
    void RpcUpdateAnimations(bool IsSwinging)
    {
        if (!hasAuthority)
        {
            anim.SetBool("IsSwinging", IsSwinging);
        }
    }
}