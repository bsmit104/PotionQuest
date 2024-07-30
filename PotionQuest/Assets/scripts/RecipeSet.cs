using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ingredient
{
    public Item item;
    public int amount;
}

[System.Serializable]
public class Recipe
{
    public Item result;

    public List<Ingredient> ingredients;
}

// This attribute allows us to create new items from the Unity editor
[CreateAssetMenu(fileName = "NewRecipeSet", menuName = "Inventory/RecipeSet")]
public class RecipeSet : ScriptableObject
{
    [SerializeField]
    public List<Recipe> Recipes; // List of all possible craftable items
}