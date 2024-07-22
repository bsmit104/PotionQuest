using System;
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

public class Alch : MonoBehaviour
{
    public InventorySlotDisplay[] displaySlots = new InventorySlotDisplay[4];

    public Inventory inventory;

    [SerializeField]
    public List<Recipe> Recipes; // List of all possible craftable items

    Item lastCraftedItem = null;

    private void Start() {
        int index = 0;
        foreach (InventorySlotDisplay slot in displaySlots)
        {
            slot.Initialize(index, inventory);
            index++;
        } 

        inventory.OnInventoryChanged += UpdateAlchemy;

        inventory.OnInventoryItemRemoved += TakeResult;
        
    }


    private void TakeResult(int index)
    {
        Debug.Log("item was removed");
        //see if the item taken was in the result slot
        if (index != 4) return;

        Debug.Log("it was the result slot!");
        //result was taken, so lets remove the ingredients
        var Recipe = Recipes.Find(recipe => recipe.result == lastCraftedItem);

        if (Recipe.result != null)
        {
            foreach(var ingredient in Recipe.ingredients)
            {
                inventory.RemoveItem(ingredient.item, ingredient.amount);
            }
            
        }
        
        UpdateAlchDisplay();

    }

    private void UpdateAlchemy()
    {
        //Here is where we will put the crafting and recipe stuff!
        //the first 4 slots of the inventory are the ingredients, and the fifth will be the output.

        Item craftedItem = GetCraftedItem();
        if (craftedItem != lastCraftedItem)
        {
            //crafted item has changed, and we need to set the fifth item to the result.
            if (craftedItem != null)
            {
                inventory.items[4].item = craftedItem;
                inventory.items[4].stackSize = 1;
            }
            else
            {
                inventory.items[4].item = craftedItem;
                inventory.items[4].stackSize = 0;
            }

            lastCraftedItem = craftedItem;
        }
        


        UpdateAlchDisplay();
    }

    private void UpdateAlchDisplay()
    {
        for (int i = 0; i < displaySlots.Length; i++)
        {
            displaySlots[i].DisplayItem(inventory.items[i].item);
        }
    }

    private Item GetCraftedItem()
    {
        //loop over ingredient slots and add to a dictionary
        Dictionary<string, int> ingredientCounts = new Dictionary<string, int>();
        for (int i = 0; i < inventory.items.Length - 1; i++)
        {
            ItemStack stack = inventory.items[i];

            if (stack.item == null) continue;
            if (ingredientCounts.ContainsKey(stack.item.itemName))
            {
                ingredientCounts[stack.item.itemName]++;
            }
            else
            {
                ingredientCounts[stack.item.itemName] = 1;
            }   
        }

        foreach (var recipe in Recipes)
        {
            bool missingIngredient = false;
            foreach(Ingredient ingredient in recipe.ingredients)
            {
                if (ingredientCounts.ContainsKey(ingredient.item.itemName) && ingredientCounts[ingredient.item.itemName] == ingredient.amount)
                {
                    continue;
                }
                missingIngredient = true;
                break;
            }

            if (missingIngredient) continue;

            //since we have all the ingredients, lets return the result!
            return recipe.result;
            
        }

        return null;
    }

    
}




















// using System;
// using System.Collections.Generic;
// using UnityEngine;

// public class Alch : MonoBehaviour
// {
//     public List<Inventory.ItemStack> AlchItems = new List<Inventory.ItemStack>();

//     public event Action OnAlchChanged;

//     public bool IsFull()
//     {
//         return AlchItems.Count >= 4; // Example limit
//     }

//     public bool AddItem(Item itemToAdd, int quantity = 1)
//     {
//         foreach (var itemStack in AlchItems)
//         {
//             if (itemStack.item.itemName == itemToAdd.itemName && itemStack.stackSize < itemStack.item.maxStack)
//             {
//                 int availableSpace = itemStack.item.maxStack - itemStack.stackSize;
//                 int toAdd = Mathf.Min(quantity, availableSpace);
//                 itemStack.stackSize += toAdd;
//                 quantity -= toAdd;
//                 if (quantity == 0)
//                 {
//                     OnAlchChanged?.Invoke();
//                     return true;
//                 }
//             }
//         }
//         if (quantity > 0 && !IsFull())
//         {
//             AlchItems.Add(new Inventory.ItemStack { item = itemToAdd, stackSize = quantity });
//             OnAlchChanged?.Invoke();
//             return true;
//         }
//         return false;
//     }

//     public void RemoveItem(Item itemToRemove, int quantity = 1)
//     {
//         if (itemToRemove == null || quantity <= 0) return;

//         for (int i = AlchItems.Count - 1; i >= 0; i--)
//         {
//             Inventory.ItemStack stack = AlchItems[i];
//             if (stack.item.itemName == itemToRemove.itemName)
//             {
//                 if (quantity >= stack.stackSize)
//                 {
//                     quantity -= stack.stackSize;
//                     AlchItems.RemoveAt(i);
//                 }
//                 else
//                 {
//                     stack.stackSize -= quantity;
//                     quantity = 0;
//                 }

//                 if (quantity == 0)
//                 {
//                     OnAlchChanged?.Invoke();
//                     return;
//                 }
//             }
//         }
//         OnAlchChanged?.Invoke();
//     }

//     // Method to check if the alchemy table has a specific item
//     public bool HasItem(Item item)
//     {
//         if (item == null) return false;
//         foreach (var itemStack in AlchItems)
//         {
//             if (itemStack.item == item)
//             {
//                 return true;
//             }
//         }
//         return false;
//     }

//     // Method to get the quantity of a specific item in the alchemy table
//     public int GetItemQuantity(Item item)
//     {
//         if (item == null) return 0;
//         foreach (var itemStack in AlchItems)
//         {
//             if (itemStack.item == item)
//             {
//                 return itemStack.stackSize;
//             }
//         }
//         return 0;
//     }
// }






// using System;
// using System.Collections.Generic;
// using UnityEngine;

// public class Alch : MonoBehaviour
// {
//     public List<Inventory.ItemStack> AlchItems = new List<Inventory.ItemStack>();

//     public event Action OnAlchChanged;

//     public bool IsFull()
//     {
//         return AlchItems.Count >= 4; // Example limit
//     }

//     public bool AddItem(Item itemToAdd, int quantity = 1)
//     {
//         foreach (var itemStack in AlchItems)
//         {
//             if (itemStack.item.itemName == itemToAdd.itemName && itemStack.stackSize < itemStack.item.maxStack)
//             {
//                 int availableSpace = itemStack.item.maxStack - itemStack.stackSize;
//                 int toAdd = Mathf.Min(quantity, availableSpace);
//                 itemStack.stackSize += toAdd;
//                 quantity -= toAdd;
//                 if (quantity == 0)
//                 {
//                     OnAlchChanged?.Invoke();
//                     return true;
//                 }
//             }
//         }
//         if (quantity > 0 && !IsFull())
//         {
//             AlchItems.Add(new Inventory.ItemStack { item = itemToAdd, stackSize = quantity });
//             OnAlchChanged?.Invoke();
//             return true;
//         }
//         return false;
//     }

//     public void RemoveItem(Item itemToRemove, int quantity = 1)
//     {
//         for (int i = AlchItems.Count - 1; i >= 0; i--)
//         {
//             Inventory.ItemStack stack = AlchItems[i];
//             if (stack.item.itemName == itemToRemove.itemName)
//             {
//                 if (quantity >= stack.stackSize)
//                 {
//                     quantity -= stack.stackSize;
//                     AlchItems.RemoveAt(i);
//                 }
//                 else
//                 {
//                     stack.stackSize -= quantity;
//                     quantity = 0;
//                 }

//                 if (quantity == 0)
//                 {
//                     OnAlchChanged?.Invoke();
//                     return;
//                 }
//             }
//         }
//         OnAlchChanged?.Invoke();
//     }
// }




////works//////

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Alch : MonoBehaviour
// {
//     public List<Inventory.ItemStack> AlchItems = new List<Inventory.ItemStack>();

//     public event Action OnAlchChanged;

//     public bool IsFull()
//     {
//         return AlchItems.Count >= 4; // Example limit
//     }

//     public bool AddItem(Item itemToAdd, int quantity = 1)
//     {
//         foreach (var itemStack in AlchItems)
//         {
//             if (itemStack.item.itemName == itemToAdd.itemName && itemStack.stackSize < itemStack.item.maxStack)
//             {
//                 int availableSpace = itemStack.item.maxStack - itemStack.stackSize;
//                 int toAdd = Mathf.Min(quantity, availableSpace);
//                 itemStack.stackSize += toAdd;
//                 quantity -= toAdd;
//                 if (quantity == 0)
//                 {
//                     OnAlchChanged?.Invoke();
//                     return true;
//                 }
//             }
//         }
//         if (quantity > 0 && !IsFull())
//         {
//             AlchItems.Add(new Inventory.ItemStack { item = itemToAdd, stackSize = quantity });
//             OnAlchChanged?.Invoke();
//             return true;
//         }
//         return false;
//     }

//     public void RemoveItem(Item itemToRemove, int quantity = 1)
//     {
//         for (int i = AlchItems.Count - 1; i >= 0; i--)
//         {
//             Inventory.ItemStack stack = AlchItems[i];
//             if (stack.item.itemName == itemToRemove.itemName)
//             {
//                 if (quantity >= stack.stackSize)
//                 {
//                     quantity -= stack.stackSize;
//                     AlchItems.RemoveAt(i);
//                 }
//                 else
//                 {
//                     stack.stackSize -= quantity;
//                     quantity = 0;
//                 }

//                 if (quantity == 0)
//                 {
//                     OnAlchChanged?.Invoke();
//                     return;
//                 }
//             }
//         }
//         OnAlchChanged?.Invoke();
//     }
// }