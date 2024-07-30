using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemStack
{
    public ItemStack()
    {

    }
    public ItemStack(Item _item, int _count)
    {
        item = _item;
        stackSize = _count;
        Value = 1;
    }
    public Item item;
    public int stackSize;
    //fck it, value for potion amount
    public float Value;
}

public class Inventory : MonoBehaviour
{
    [SerializeField]
    public ItemStack[] items = new ItemStack[4];

    

    public event Action OnInventoryChanged;
    public event Action<int> OnInventoryItemRemoved;
    public event Action<Item> OnSelectedItemChanged;



    void Start()
    {
    }

    public bool IsFull()
    {
        foreach (var itemstack in items)
        {
            if (itemstack.item == null)
            {
                return false;
            }
        }
        return true;
    }

    public Item GetItemByName(string itemName)
    {
        foreach (var itemStack in items)
        {
            if (itemStack.item != null && itemStack.item.itemName == itemName)
            {
                return itemStack.item;
            }
        }
        return null;
    }

    //returns how many were actually added to the inventory.
    public int AddItem(Item itemToAdd, int quantity = 1, float Value = 1)
    {
        if (quantity <= 0) return 0;
        int remaining = quantity;

        //first try adding to existing stacks
        for (int i = 0; i < items.Length; i++)
        {
            ItemStack itemStack = items[i];
            //first adding to existing stacks
            
            if (itemStack.item != null && itemStack.item.itemName == itemToAdd.itemName && itemStack.stackSize < itemStack.item.maxStack)
            {
                int availableSpace = itemStack.item.maxStack - itemStack.stackSize;
                int toAdd = Mathf.Min(quantity, availableSpace);
                itemStack.stackSize += toAdd;
                itemStack.Value = Value;
                remaining -= toAdd;
                if (remaining == 0)
                {
                    OnInventoryChanged?.Invoke();
                    return quantity;
                }
            }
        }

        for (int i = 0; i < items.Length; i++)
        {
            ItemStack itemStack = items[i];
            if (itemStack.item == null)
            {
                int toAdd = Mathf.Min(itemToAdd.maxStack, remaining);
                remaining -= toAdd;
                items[i].item = itemToAdd;
                items[i].stackSize = toAdd;
                items[i].Value = Value;
                if (remaining == 0)
                {
                    OnInventoryChanged?.Invoke();
                    return quantity;
                }
            }
        }

        int amountRemoved = quantity - remaining;
        if (amountRemoved != 0) OnInventoryChanged?.Invoke();
        return amountRemoved;
    }

    public int AddItemToSlot(int slotIndex, Item itemToAdd, int quantity = 1, float Value = 1)
    {
        if (slotIndex < 0 || slotIndex > items.Length) return 0;
        if (quantity <= 0) return 0;
        int remaining = quantity;

        //try adding to existing stack in slot
        ItemStack itemStack = items[slotIndex];
        if (itemStack.item != null)
        {
            //there is already a stack, so lets try to add to it
            if (itemStack.item.itemName == itemToAdd.itemName && itemStack.stackSize < itemStack.item.maxStack)
            {
                int availableSpace = itemStack.item.maxStack - itemStack.stackSize;
                int toAdd = Mathf.Min(quantity, availableSpace);
                itemStack.stackSize += toAdd;
                itemStack.Value = Value;
                remaining -= toAdd;
            }else
            {
                //there was a stack there, but not of the same type!
                return 0;
            }
        }else
        {
            //there isn't a stack, so lets just add the stack here!
            int toAdd = Mathf.Min(itemToAdd.maxStack, remaining);
            remaining -= toAdd;
            items[slotIndex].item = itemToAdd;
            items[slotIndex].stackSize = toAdd;
            items[slotIndex].Value = Value;
            if (remaining == 0)
            {
                OnInventoryChanged?.Invoke();
                return quantity;
            }
            
        }

        

        int amountRemoved = quantity - remaining;
        if (amountRemoved != 0) OnInventoryChanged?.Invoke();
        return amountRemoved;
    }

    public ItemStack GetItemStackAtSlot(int slotIndex)
    {
        return items[slotIndex];
    }

    /// <summary>
    /// returns the amount removed from the inventory
    /// </summary>
    public int RemoveItem(Item itemToRemove, int quantity = 1)
    {
        Debug.Log($"Trying to remove {quantity} of {itemToRemove.itemName}");

        int remainingToRemove = quantity;
        for (int i = items.Length - 1; i >= 0; i--)
        {
            ItemStack stack = items[i];

            //ignore empty stacks
            if (stack.item == null) continue;

            if (stack.item.itemName == itemToRemove.itemName)
            {
                if (remainingToRemove >= stack.stackSize)//need to delete whole stack
                {
                    remainingToRemove -= stack.stackSize;
                    items[i].item = null;
                    items[i].stackSize = 0;
                }
                else
                {
                    stack.stackSize -= remainingToRemove;
                    Debug.Log($"Reducing stack by {quantity}, new count {stack.stackSize}");
                    remainingToRemove = 0;
                }

                if (remainingToRemove == 0)
                {
                    OnInventoryChanged?.Invoke();
                    return quantity;
                }
            }
        }

        int amountRemoved = quantity - remainingToRemove;
        if (amountRemoved != 0) OnInventoryChanged?.Invoke();
        return amountRemoved;
    }

    public void RemoveItemFromSlot(int slot, int amount)
    {
        items[slot].stackSize -= amount;
        if (items[slot].stackSize < 1)
        {
            items[slot].item = null;
            items[slot].stackSize = 0;
        }
        OnInventoryChanged?.Invoke();
        //Debug.Log("invoked!");
        OnInventoryItemRemoved?.Invoke(slot);
    }
}