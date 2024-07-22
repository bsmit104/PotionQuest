using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : Inventory
{
    int selectedSlot = 0;

    void Update()
    {
        HandleNumberKeyInput();
    }

    private void HandleNumberKeyInput()
    {
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectItem(i);
            }
        }
    }

    public void SelectItem(int index)
    {
        if (index >= 0 && index < items.Length)
        {
            selectedSlot = index;
            //selectedItemStack = items[index];
            //OnSelectedItemChanged?.Invoke(selectedItemStack.item);
            //Debug.Log("Selected item: " + selectedItemStack.item.itemName);
        }
    }

    public Item GetSelectedItem()
    {
        return items[selectedSlot].item;
    }
}
