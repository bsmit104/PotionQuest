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
                playerInventoryUI.slots[selectedSlot].transform.localScale = Vector3.one;
                if (iconImage != null)
                    if (iconImage.sprite == null)
                        iconImage.color = new Color(1, 0.6588176f, 0.4764151f, 1);
                    
            }
            
            selectedSlot = index;

            iconTransform = playerInventoryUI.slots[selectedSlot].transform.Find("Icon");
            if (iconTransform != null)
            {
                playerInventoryUI.slots[selectedSlot].transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
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
