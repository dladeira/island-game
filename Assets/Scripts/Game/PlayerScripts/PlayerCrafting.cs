using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerCrafting : NetworkBehaviour
{
    [SerializeField] PlayerManager player;
    [SerializeField] List<InventoryRecipeData> recipes;

    [Header("UI")]
    [SerializeField] GameObject craftingPanel;
    [SerializeField] GameObject recipesObject;
    [SerializeField] GameObject recipePrefab;

    void Start()
    {
        player.inventory.onInventoryChangeEvent += UpdateRecipes;
        ToggleOpen(false);
    }

    public void ToggleOpen(bool open)
    {
        craftingPanel.SetActive(open);
    }

    void UpdateRecipes()
    {
        foreach (Transform child in recipesObject.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (InventoryRecipeData recipe in recipes)
        {
            if (recipe.HasRequirements(player.inventory))
                Instantiate(recipePrefab, recipesObject.transform).GetComponent<InventoryRecipe>().Initialize(player, recipe);
        }
    }
}
