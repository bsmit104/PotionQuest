///one below this one is good but idk this one might b better
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class AlchUI : InventoryUI
{
    public Transform resultSlot; // The slot for the crafted item

    public List<Item> craftableItems; // List of all possible craftable items
    
    private Item lastCraftedItem; // Track the last crafted item

    new void Start()
    {
        // Initialize result slot for drag operations
        if (resultSlot != null)
        {
            EventTrigger resultSlotTrigger = resultSlot.gameObject.AddComponent<EventTrigger>();
            AddEventTrigger(resultSlotTrigger, EventTriggerType.BeginDrag, BeginDrag);
            AddEventTrigger(resultSlotTrigger, EventTriggerType.Drag, Drag);
            AddEventTrigger(resultSlotTrigger, EventTriggerType.EndDrag, EndDrag);
        }

        //do the normal UI startup
        base.Start();
    }

    new public void BeginDrag(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;
        Debug.Log("BeginDrag called. Pointer dragging: " + pointerData.pointerDrag.name);

        if (pointerData.pointerDrag == resultSlot.gameObject)
        {
            Debug.Log("Dragging from result slot");
            Item itemToMove = lastCraftedItem;
            if (itemToMove != null)
            {
                draggedItem = CreateDraggedItemFromResultSlot(itemToMove);
                resultSlot.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }
        else
        {
            Debug.Log("Dragging from alchemy slot");
            draggedItemIndex = GetSlotIndex(pointerData.pointerPress);
            Debug.Log("Alchemy slot index: " + draggedItemIndex);

            if (draggedItemIndex >= 0 && draggedItemIndex < inventory.items.Length)
            {
                draggedItem = CreateDraggedItem(draggedItemIndex);
                slots[draggedItemIndex].GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }
    }

    // new public void EndDrag(BaseEventData data)
    // {
    //     if (draggedItem != null)
    //     {
    //         if (resultSlot.GetComponent<CanvasGroup>() != null)
    //         {
    //             resultSlot.GetComponent<CanvasGroup>().blocksRaycasts = true;
    //         }

    //         if (IsPointerOverUIObject(inventoryUI.slotPanel.gameObject))
    //         {
    //             Debug.Log("Dropped over inventory UI");
    //             if (lastCraftedItem != null)
    //             {
    //                 if (inventoryUI.inventory.AddItem(lastCraftedItem, 1))
    //                 {
    //                     RemoveCraftedItemsFromAlch();

    //                     Transform iconTransform = resultSlot.Find("Icon");
    //                     Image iconImage = iconTransform?.GetComponent<Image>();
    //                     if (iconImage != null)
    //                     {
    //                         iconImage.sprite = null;
    //                     }
    //                     lastCraftedItem = null; 
    //                 }
    //             }
    //             else
    //             {
    //                 if (draggedItemIndex >= 0 && draggedItemIndex < Alch.AlchItems.Count)
    //                 {
    //                     Item itemToMove = Alch.AlchItems[draggedItemIndex].item;
    //                     if (inventoryUI.inventory.AddItem(itemToMove, 1))
    //                     {
    //                         Alch.RemoveItem(itemToMove, 1);
    //                     }
    //                 }
    //             }
    //         }

    //         Destroy(draggedItem);
    //         draggedItem = null;
    //         draggedItemIndex = -1;

    //         UpdateAlchUI();
    //         inventoryUI.UpdateInventoryUI();
    //     }
    // }

    public override void EndDrag(BaseEventData data)
    {
        if (draggedItem != null)
        {
            // Re-enable raycasts for the original slot
            if (draggedItemIndex >= 0 && draggedItemIndex < slots.Count)
            {
                slots[draggedItemIndex].GetComponent<CanvasGroup>().blocksRaycasts = true;
            }

            foreach (InventoryUI otherUI in TransferableTo)
            {
                int amountAdded = 0;
                // Check if the dragged item is over the chest UI
                if (IsPointerOverUIObject(otherUI.slotPanel.gameObject))
                {
                    //make sure we are dragging something valid?
                    if (draggedItemIndex >= 0 && draggedItemIndex < inventory.items.Length)
                    {
                        Item itemToMove = inventory.items[draggedItemIndex].item;
                        int amount = inventory.items[draggedItemIndex].stackSize;
                        // Try to add the item to the chest
                        //check if we are over a specific slot, and if we are put it in at that slot
                        PointerEventData pointerData = (PointerEventData)data;
                        int otherInvIndex = otherUI.GetSlotIndex(pointerData.pointerPress);
                        if (otherInvIndex >= 0 && otherInvIndex < otherUI.inventory.items.Length)
                        {
                            //we found a slot to put into, so lets put it in
                            amountAdded = otherUI.inventory.AddItemToSlot(otherInvIndex, itemToMove, amount);
                            if (amountAdded == amount)
                            {
                                Debug.Log("All Items added to chest");
                                // Remove the item from inventory if added successfully
                                inventory.items[draggedItemIndex] = null;
                            }
                            else
                            {
                                Debug.Log("only added " + amountAdded + " items");
                                inventory.items[draggedItemIndex].stackSize -= amountAdded;
                            }
                        }else
                        {
                            //there was no slot, so we just add it to the inventory
                            amountAdded = otherUI.inventory.AddItem(itemToMove, amount);
                            if (amountAdded == amount)
                            {
                                Debug.Log("All Items added to chest");
                                // Remove the item from inventory if added successfully
                                inventory.items[draggedItemIndex] = null;
                            }
                            else
                            {
                                Debug.Log("only added " + amountAdded + " items");
                                inventory.items[draggedItemIndex].stackSize -= amountAdded;
                            }
                        }
                        
                    }
                    otherUI.UpdateInventoryUI();
                    break; // only add to one inventory UI. 
                }
                else
                {
                    Debug.Log("Dragged item not over chestUI slotPanel");
                }
            }

            // Clean up the dragged item object
            Destroy(draggedItem);
            draggedItem = null;
            draggedItemIndex = -1;

            // Update the UI to reflect changes
            UpdateInventoryUI();
        }
    }

    GameObject CreateDraggedItemFromResultSlot(Item item)
    {
        GameObject itemObject = Instantiate(draggedItemPrefab);
        Image itemImage = itemObject.GetComponent<Image>();
        if (itemImage != null)
        {
            itemImage.sprite = item.itemIcon;
            itemImage.SetNativeSize();
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            RectTransform rectTransform = itemObject.GetComponent<RectTransform>();
            rectTransform.SetParent(canvas.transform, false);
            rectTransform.anchoredPosition = Input.mousePosition;
            rectTransform.sizeDelta = new Vector2(itemImage.sprite.rect.width, itemImage.sprite.rect.height);
        }
        else
        {
            Debug.LogError("Canvas not found!");
        }

        return itemObject;
    }

    private void RemoveCraftedItemsFromAlch()
    {
        Dictionary<string, int> ingredientCounts = new Dictionary<string, int>();
        foreach (var itemStack in inventory.items)
        {
            if (itemStack == null) continue;
            if (ingredientCounts.ContainsKey(itemStack.item.itemName))
            {
                ingredientCounts[itemStack.item.itemName]++;
            }
            else
            {
                ingredientCounts[itemStack.item.itemName] = 1;
            }
        }

        if (ingredientCounts.ContainsKey("Lavender") && ingredientCounts.ContainsKey("Water") && ingredientCounts["Lavender"] == 1 && ingredientCounts["Water"] == 1)
        {
            inventory.RemoveItem(GetItemByName("Lavender"), 1);
            inventory.RemoveItem(GetItemByName("Water"), 1);
        }
        else if (ingredientCounts.ContainsKey("Fly Agaric") && ingredientCounts.ContainsKey("Water") && ingredientCounts["Fly Agaric"] == 1 && ingredientCounts["Water"] == 1)
        {
            inventory.RemoveItem(GetItemByName("Fly Agaric"), 1);
            inventory.RemoveItem(GetItemByName("Water"), 1);
        }
        else if (ingredientCounts.ContainsKey("Mandrake") && ingredientCounts.ContainsKey("Sulfuric Acid") && ingredientCounts.ContainsKey("Skull") && ingredientCounts.ContainsKey("Water")
            && ingredientCounts["Mandrake"] == 1 && ingredientCounts["Sulfuric Acid"] == 1 && ingredientCounts["Skull"] == 1 && ingredientCounts["Water"] == 1)
        {
            inventory.RemoveItem(GetItemByName("Mandrake"), 1);
            inventory.RemoveItem(GetItemByName("Sulfuric Acid"), 1);
            inventory.RemoveItem(GetItemByName("Skull"), 1);
            inventory.RemoveItem(GetItemByName("Water"), 1);
        }
        else if (ingredientCounts.ContainsKey("Fly Agaric") && ingredientCounts.ContainsKey("Mead") && ingredientCounts["Fly Agaric"] == 1 && ingredientCounts["Mead"] == 1)
        {
            inventory.RemoveItem(GetItemByName("Fly Agaric"), 1);
            inventory.RemoveItem(GetItemByName("Mead"), 1);
        }
    }

    void Update()
    {
        CheckCraftingResult(); 
    }

    private void CheckCraftingResult()
    {
        Item craftedItem = GetCraftedItem();
        lastCraftedItem = craftedItem;

        Transform iconTransform = resultSlot.Find("Icon");
        Image iconImage = iconTransform?.GetComponent<Image>();

        if (craftedItem != null)
        {
            if (iconImage != null)
            {
                iconImage.sprite = craftedItem.itemIcon;
                iconImage.color = Color.white;
            }
        }
        else
        {
            if (iconImage != null)
            {
                iconImage.sprite = null;
            }
        }
    }

    private Item GetCraftedItem()
    {
        Dictionary<string, int> ingredientCounts = new Dictionary<string, int>();
        foreach (var itemStack in inventory.items)
        {
            if (itemStack == null) continue;
            if (ingredientCounts.ContainsKey(itemStack.item.itemName))
            {
                ingredientCounts[itemStack.item.itemName]++;
            }
            else
            {
                ingredientCounts[itemStack.item.itemName] = 1;
            }
        }

        if (ingredientCounts.ContainsKey("Lavender") && ingredientCounts.ContainsKey("Water") && ingredientCounts["Lavender"] == 1 && ingredientCounts["Water"] == 1)
        {
            return GetItemByName("Light Essence");
        }
        if (ingredientCounts.ContainsKey("Fly Agaric") && ingredientCounts.ContainsKey("Water") && ingredientCounts["Fly Agaric"] == 1 && ingredientCounts["Water"] == 1)
        {
            return GetItemByName("Goblin Tonic");
        }
        if (ingredientCounts.ContainsKey("Mandrake") && ingredientCounts.ContainsKey("Sulfuric Acid") && ingredientCounts.ContainsKey("Skull") && ingredientCounts.ContainsKey("Water")
            && ingredientCounts["Mandrake"] == 1 && ingredientCounts["Sulfuric Acid"] == 1 && ingredientCounts["Skull"] == 1 && ingredientCounts["Water"] == 1)
        {
            return GetItemByName("Corrosive Elixir");
        }
        if (ingredientCounts.ContainsKey("Fly Agaric") && ingredientCounts.ContainsKey("Mead") && ingredientCounts["Fly Agaric"] == 1 && ingredientCounts["Mead"] == 1)
        {
            return GetItemByName("Strength Nectar");
        }

        return null;
    }

    private Item GetItemByName(string itemName)
    {
        return craftableItems.Find(item => item.itemName == itemName);
    }
}






