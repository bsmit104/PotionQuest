using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class PlayerInventory : Inventory
{
    public int selectedSlot = 0;
    public InventoryUI playerInventoryUI;

    private void Start() {
        //SelectItem(selectedSlot);
    }

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
            //set previous selection to normal color
            Transform iconTransform = playerInventoryUI.slots[selectedSlot].transform.Find("Icon");
            if (iconTransform != null)
            {
                UnityEngine.UI.Image iconImage = iconTransform?.GetComponent<UnityEngine.UI.Image>();
                if (iconImage != null)
                    iconImage.color = new Color(0.4431373f, 0.172549f, 0.172549f, 1);
            }
            
            selectedSlot = index;

            iconTransform = playerInventoryUI.slots[selectedSlot].transform.Find("Icon");
            if (iconTransform != null)
            {
                UnityEngine.UI.Image iconImage = iconTransform?.GetComponent<UnityEngine.UI.Image>();
                if (iconImage != null)
                    iconImage.color = Color.white;
            }

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
