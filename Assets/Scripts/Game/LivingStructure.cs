using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LivingStructure : NetworkBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] public int health { get; private set; }
    [SerializeField] public List<GameObject> drops;
    public bool showHealth { get; private set; } = false;

    void Start()
    {
        this.health = maxHealth;
    }

    public void SetHealth(int newHealth)
    {
        if (newHealth <= 0)
        {
            OnObjectDeath();
            Destroy(this.gameObject);
            return;
        }

        StopAllCoroutines();
        StartCoroutine(ShowHealth(5));
        this.health = newHealth;
    }

    private void OnObjectDeath()
    {
        foreach (GameObject itemObject in drops)
        {
            GameObject spawned = Instantiate(itemObject, this.transform.position + new Vector3(0, 1, 0), itemObject.transform.rotation);
            NetworkServer.Spawn(spawned);
        }
    }

    System.Collections.IEnumerator ShowHealth(float time)
    {
        showHealth = true;
        yield return new WaitForSeconds(time);
        showHealth = false;
    }
}
