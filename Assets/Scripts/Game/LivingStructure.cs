using System.Collections.Generic;
using UnityEngine;

public class LivingStructure : MonoBehaviour
{
    [SerializeField] public int health { get; private set;} = 100;
    [SerializeField] public List<ItemObject> drops;
    public bool showHealth { get; private set; } = false;

    public void SetHealth(int newHealth)
    {
        Debug.Log("new HP: " + newHealth);

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
        Debug.Log("DEADOWA");
    }

    System.Collections.IEnumerator ShowHealth(float time)
    {
        showHealth = true;
        yield return new WaitForSeconds(time);
        showHealth = false;
    }
}
