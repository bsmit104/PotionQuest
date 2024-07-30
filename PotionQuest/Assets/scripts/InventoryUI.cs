using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public Transform slotPanel;
    public GameObject draggedItemPrefab;
    public Image trashCanImage;

    public List<InventoryUI> TransferableTo = new List<InventoryUI>();

    public List<GameObject> slots = new List<GameObject>();
    protected GameObject draggedItem;
    protected int draggedItemIndex = -1;

    public void Start()
    {
        inventory.OnInventoryChanged += UpdateInventoryUI;
        InitializeSlots();
        UpdateInventoryUI();

        TransferableTo.Add (this);
    }

    protected void InitializeSlots()
    {
        for (int i = 0; i < slotPanel.childCount; i++)
        {
            GameObject slot = slotPanel.GetChild(i).gameObject;
            slots.Add(slot);

            if (slot.GetComponent<CanvasGroup>() == null)
            {
                slot.AddComponent<CanvasGroup>();
            }

            EventTrigger trigger = slot.AddComponent<EventTrigger>();
            AddEventTrigger(trigger, EventTriggerType.PointerClick, OnSlotClick);
            AddEventTrigger(trigger, EventTriggerType.BeginDrag, BeginDrag);
            AddEventTrigger(trigger, EventTriggerType.Drag, Drag);
            AddEventTrigger(trigger, EventTriggerType.EndDrag, EndDrag);
        }
    }

    protected void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    void OnSlotClick(BaseEventData data)
    {
        // Handle slot click logic if needed
    }

    public void BeginDrag(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;
        draggedItemIndex = GetSlotIndex(pointerData.pointerPress);
        if (draggedItemIndex >= 0 && draggedItemIndex < inventory.items.Length)
        {
            if ( inventory.items[draggedItemIndex].item != null)
            {
                draggedItem = CreateDraggedItem(draggedItemIndex);
                slots[draggedItemIndex].GetComponent<CanvasGroup>().blocksRaycasts = false;
            }else
            {
                //trying to drag an empty item, so do nothing! revert index as well
                draggedItemIndex = -1;
            }
        }
    }

    protected GameObject CreateDraggedItem(int index)
    {
        GameObject itemObject = Instantiate(draggedItemPrefab);
        RectTransform rectTransform = itemObject.GetComponent<RectTransform>();
        Image itemImage = itemObject.GetComponent<Image>();

        if (itemImage != null)
        {
            itemImage.sprite = inventory.items[index].item.itemIcon;
            itemImage.SetNativeSize();
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            rectTransform.SetParent(canvas.transform, false);
            rectTransform.anchoredPosition = Input.mousePosition; // Position it at the mouse cursor
            rectTransform.sizeDelta = new Vector2(itemImage.sprite.rect.width, itemImage.sprite.rect.height); // Set size to sprite size
        }
        else
        {
            Debug.LogError("Canvas not found!");
        }

        return itemObject;
    }

    public void Drag(BaseEventData data)
    {
        if (draggedItem != null)
        {
            PointerEventData pointerData = (PointerEventData)data;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)draggedItem.transform.parent,
                pointerData.position,
                null,
                out Vector2 localPoint
            );
            draggedItem.GetComponent<RectTransform>().anchoredPosition = localPoint;
        }
    }

    // public void EndDrag(BaseEventData data)
    // {
    //     if (draggedItem != null)
    //     {
    //         slots[draggedItemIndex].GetComponent<CanvasGroup>().blocksRaycasts = true;

    //         if (IsPointerOverUIObject(chestUI.slotPanel.gameObject))
    //         {
    //             if (draggedItemIndex >= 0 && draggedItemIndex < inventory.items.Count)
    //             {
    //                 Item itemToMove = inventory.items[draggedItemIndex].item;
    //                 if (chestUI.chest.AddItem(itemToMove, 1))
    //                 {
    //                     inventory.RemoveItem(itemToMove, 1);
    //                 }
    //             }
    //         }

    //         Destroy(draggedItem);
    //         draggedItem = null;
    //         draggedItemIndex = -1;

    //         UpdateInventoryUI();
    //         chestUI.UpdateChestUI();
    //     }
    // }

    virtual public void EndDrag(BaseEventData data)
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
                // Check if the dragged item is over the chest UI
                if (IsPointerOverUIObject(otherUI.slotPanel.gameObject))
                {
                    //make sure we are dragging something valid?
                    if (draggedItemIndex >= 0 && draggedItemIndex < inventory.items.Length)
                    {
                        Item itemToMove = inventory.items[draggedItemIndex].item;
                        int amount = inventory.items[draggedItemIndex].stackSize;
                        float valueToMove = inventory.items[draggedItemIndex].Value;
                        // Try to add the item to the chest
                        //check if we are over a specific slot, and if we are put it in at that slot
                        int otherInvIndex = -1;
                        for (int i = 0; i < otherUI.slots.Count; i++)
                        {
                            GameObject slot = otherUI.slots[i];
                            if (IsPointerOverUIObject(slot))
                            {
                                otherInvIndex = i;
                                break;
                            }
                        }
                        //Debug.Log("Trying to place item into slot: " + otherInvIndex);
                        if (otherInvIndex >= 0 && otherInvIndex < otherUI.inventory.items.Length)
                        {
                            //we found a slot to put into, so lets put it in
                            int amountAdded = otherUI.inventory.AddItemToSlot(otherInvIndex, itemToMove, amount, valueToMove);
                            if (amountAdded == amount)
                            {
                                //Debug.Log("All Items added to chest");
                                // Remove the item from inventory if added successfully
                                inventory.RemoveItemFromSlot(draggedItemIndex, amountAdded);
                                
                            }
                            else
                            {
                                //Debug.Log("only added " + amountAdded + " items");
                                inventory.RemoveItemFromSlot(draggedItemIndex, amountAdded);
                            }
                        }else
                        {
                            //there was no slot, so we just add it to the inventory
                            int amountAdded = otherUI.inventory.AddItem(itemToMove, amount, valueToMove);
                            if (amountAdded == amount)
                            {
                                //Debug.Log("All Items added to chest");
                                // Remove the item from inventory if added successfully
                                inventory.RemoveItemFromSlot(draggedItemIndex, amountAdded);
                            }
                            else
                            {
                                //Debug.Log("only added " + amountAdded + " items");
                                inventory.RemoveItemFromSlot(draggedItemIndex, amountAdded);
                            }
                        }
                        
                    }
                    otherUI.UpdateInventoryUI();
                    break; // only add to one inventory UI. 
                }
                else
                {
                    //Debug.Log("Dragged item not over chestUI slotPanel");
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

    public int GetSlotIndex(GameObject slot)
    {
        return slots.IndexOf(slot);
    }

    public bool IsPointerOverUIObject(GameObject uiObject)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        return results.Exists(result => result.gameObject == uiObject);
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            Transform iconTransform = slots[i].transform.Find("Icon");
            Transform countTransform = slots[i].transform.Find("Count");

            Image iconImage = iconTransform?.GetComponent<Image>();
            TMP_Text countText = countTransform?.GetComponent<TMP_Text>();

            if (i < inventory.items.Length  &&  inventory.items[i].item != null)
            {
                if (iconImage != null)
                {
                    iconImage.sprite = inventory.items[i].item.itemIcon;
                    iconImage.color = Color.white;
                }
                if (countText != null)
                {
                    countText.text = inventory.items[i].stackSize.ToString();
                }
            }
            else
            {
                if (iconImage != null) iconImage.sprite = null;
                if (iconImage != null) iconImage.color = new Color(1, 0.6588176f, 0.4764151f, 1);
                if (countText != null) countText.text = "";
            }
        }
    }

    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= UpdateInventoryUI;
        }
    }
}

