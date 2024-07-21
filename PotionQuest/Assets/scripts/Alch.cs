// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Alch : MonoBehaviour
// {
//     public List<Inventory.ItemStack> AlchItems = new List<Inventory.ItemStack>();
//     public Inventory.ItemStack craftedItem; // The resulting crafted item
//     public Inventory inventory; // Reference to the Inventory to get item definitions

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

//     private void Update()
//     {
//         CheckCrafting();
//     }

//     private void CheckCrafting()
//     {
//         Dictionary<string, int> itemCounts = new Dictionary<string, int>();
//         foreach (var stack in AlchItems)
//         {
//             if (itemCounts.ContainsKey(stack.item.itemName))
//             {
//                 itemCounts[stack.item.itemName] += stack.stackSize;
//             }
//             else
//             {
//                 itemCounts[stack.item.itemName] = stack.stackSize;
//             }
//         }

//         Inventory.ItemStack result = null;

//         if (itemCounts.ContainsKey("Lavender") && itemCounts.ContainsKey("Water"))
//         {
//             result = new Inventory.ItemStack
//             {
//                 item = inventory.GetItemByName("Light Essence"),
//                 stackSize = 1
//             };
//             Debug.Log("light essence made");
//         }
//         else if (itemCounts.ContainsKey("Fly Agaric") && itemCounts.ContainsKey("Water"))
//         {
//             result = new Inventory.ItemStack
//             {
//                 item = inventory.GetItemByName("Goblin Tonic"),
//                 stackSize = 1
//             };
//         }
//         else if (itemCounts.ContainsKey("Mandrake") && itemCounts.ContainsKey("Sulfuric Acid") && itemCounts.ContainsKey("Skull") && itemCounts.ContainsKey("Water"))
//         {
//             result = new Inventory.ItemStack
//             {
//                 item = inventory.GetItemByName("Corrosive Elixir"),
//                 stackSize = 1
//             };
//         }
//         else if (itemCounts.ContainsKey("Fly Agaric") && itemCounts.ContainsKey("Mead"))
//         {
//             result = new Inventory.ItemStack
//             {
//                 item = inventory.GetItemByName("Strength Nectar"),
//                 stackSize = 1
//             };
//         }
//         else if (itemCounts.ContainsKey("Gold Coin") && itemCounts.ContainsKey("Rabbits Foot") && itemCounts.ContainsKey("Water"))
//         {
//             result = new Inventory.ItemStack
//             {
//                 item = inventory.GetItemByName("Fluid Fortune"),
//                 stackSize = 1
//             };
//         }

//         craftedItem = result;
//         OnAlchChanged?.Invoke();
//     }
// }





////works//////

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alch : MonoBehaviour
{
    public List<Inventory.ItemStack> AlchItems = new List<Inventory.ItemStack>();

    public event Action OnAlchChanged;

    public bool IsFull()
    {
        return AlchItems.Count >= 4; // Example limit
    }

    public bool AddItem(Item itemToAdd, int quantity = 1)
    {
        foreach (var itemStack in AlchItems)
        {
            if (itemStack.item.itemName == itemToAdd.itemName && itemStack.stackSize < itemStack.item.maxStack)
            {
                int availableSpace = itemStack.item.maxStack - itemStack.stackSize;
                int toAdd = Mathf.Min(quantity, availableSpace);
                itemStack.stackSize += toAdd;
                quantity -= toAdd;
                if (quantity == 0)
                {
                    OnAlchChanged?.Invoke();
                    return true;
                }
            }
        }
        if (quantity > 0 && !IsFull())
        {
            AlchItems.Add(new Inventory.ItemStack { item = itemToAdd, stackSize = quantity });
            OnAlchChanged?.Invoke();
            return true;
        }
        return false;
    }

    public void RemoveItem(Item itemToRemove, int quantity = 1)
    {
        for (int i = AlchItems.Count - 1; i >= 0; i--)
        {
            Inventory.ItemStack stack = AlchItems[i];
            if (stack.item.itemName == itemToRemove.itemName)
            {
                if (quantity >= stack.stackSize)
                {
                    quantity -= stack.stackSize;
                    AlchItems.RemoveAt(i);
                }
                else
                {
                    stack.stackSize -= quantity;
                    quantity = 0;
                }

                if (quantity == 0)
                {
                    OnAlchChanged?.Invoke();
                    return;
                }
            }
        }
        OnAlchChanged?.Invoke();
    }
}