using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class Enemy : NetworkBehaviour
{
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

    private float hitCooldown;

    [SerializeField] private float attackDelay;
    [SerializeField] private int damage;
    [SerializeField] private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (PlayerManager player in Room.GamePlayers)
        {
            float distance = Vector3.Distance(player.transform.position, this.transform.position);
            if (distance < 10)
            {
                agent.SetDestination(player.transform.position);

                if (distance < 1) {
                    if (hitCooldown <= 0)
                    {
                        hitCooldown = attackDelay;
                        player.stats.SetHp(player.stats.hp - damage);
                    }
                    else
                    {
                        hitCooldown -= Time.deltaTime;
                    }
                }
            }
        }
    }
}
