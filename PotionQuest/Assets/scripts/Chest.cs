using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public List<Inventory.ItemStack> chestItems = new List<Inventory.ItemStack>();

    public event Action OnChestChanged;

    public bool IsFull()
    {
        return chestItems.Count >= 9; // Example limit
    }

    public bool AddItem(Item itemToAdd, int quantity = 1)
    {
        foreach (var itemStack in chestItems)
        {
            if (itemStack.item.itemName == itemToAdd.itemName && itemStack.stackSize < itemStack.item.maxStack)
            {
                int availableSpace = itemStack.item.maxStack - itemStack.stackSize;
                int toAdd = Mathf.Min(quantity, availableSpace);
                itemStack.stackSize += toAdd;
                quantity -= toAdd;
                if (quantity == 0)
                {
                    OnChestChanged?.Invoke();
                    return true;
                }
            }
        }
        if (quantity > 0 && !IsFull())
        {
            chestItems.Add(new Inventory.ItemStack { item = itemToAdd, stackSize = quantity });
            OnChestChanged?.Invoke();
            return true;
        }
        return false;
    }

    public void RemoveItem(Item itemToRemove, int quantity = 1)
    {
        for (int i = chestItems.Count - 1; i >= 0; i--)
        {
            Inventory.ItemStack stack = chestItems[i];
            if (stack.item.itemName == itemToRemove.itemName)
            {
                if (quantity >= stack.stackSize)
                {
                    quantity -= stack.stackSize;
                    chestItems.RemoveAt(i);
                }
                else
                {
                    stack.stackSize -= quantity;
                    quantity = 0;
                }

                if (quantity == 0)
                {
                    OnChestChanged?.Invoke();
                    return;
                }
            }
        }
        OnChestChanged?.Invoke();
    }
}