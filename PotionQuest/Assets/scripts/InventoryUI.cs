using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public ChestUI chestUI;
    public AlchUI AlchUI;
    public Transform slotPanel;
    public GameObject draggedItemPrefab;
    public Image trashCanImage;

    private List<GameObject> slots = new List<GameObject>();
    private GameObject draggedItem;
    private int draggedItemIndex = -1;

    void Start()
    {
        inventory.OnInventoryChanged += UpdateInventoryUI;
        InitializeSlots();
        UpdateInventoryUI();
    }

    void InitializeSlots()
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

    void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> action)
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
        if (draggedItemIndex >= 0 && draggedItemIndex < inventory.items.Count)
        {
            draggedItem = CreateDraggedItem(draggedItemIndex);
            slots[draggedItemIndex].GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    GameObject CreateDraggedItem(int index)
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

    public void EndDrag(BaseEventData data)
    {
        if (draggedItem != null)
        {
            // Re-enable raycasts for the original slot
            if (draggedItemIndex >= 0 && draggedItemIndex < slots.Count)
            {
                slots[draggedItemIndex].GetComponent<CanvasGroup>().blocksRaycasts = true;
            }

            // Check if the dragged item is over the chest UI
            if (IsPointerOverUIObject(chestUI.slotPanel.gameObject))
            {
                if (draggedItemIndex >= 0 && draggedItemIndex < inventory.items.Count)
                {
                    Item itemToMove = inventory.items[draggedItemIndex].item;

                    // Try to add the item to the chest
                    if (chestUI.chest.AddItem(itemToMove, 1))
                    {
                        Debug.Log("Item added to chest: " + itemToMove.itemName);
                        // Remove the item from inventory if added successfully
                        inventory.RemoveItem(itemToMove, 1);
                    }
                    else
                    {
                        Debug.Log("Failed to add item to chest: " + itemToMove.itemName);
                    }
                }
            }
            else
            {
                Debug.Log("Dragged item not over chestUI slotPanel");
            }

            if (IsPointerOverUIObject(AlchUI.slotPanel.gameObject))
            {
                if (draggedItemIndex >= 0 && draggedItemIndex < inventory.items.Count)
                {
                    Item itemToMove = inventory.items[draggedItemIndex].item;

                    // Try to add the item to the chest
                    if (AlchUI.Alch.AddItem(itemToMove, 1))
                    {
                        Debug.Log("Item added to Alch: " + itemToMove.itemName);
                        // Remove the item from inventory if added successfully
                        inventory.RemoveItem(itemToMove, 1);
                    }
                    else
                    {
                        Debug.Log("Failed to add item to Alch: " + itemToMove.itemName);
                    }
                }
            }
            else
            {
                Debug.Log("Dragged item not over AlchUI slotPanel");
            }

            // Clean up the dragged item object
            Destroy(draggedItem);
            draggedItem = null;
            draggedItemIndex = -1;

            // Update the UI to reflect changes
            UpdateInventoryUI();
            chestUI.UpdateChestUI();
            AlchUI.UpdateAlchUI();
        }
    }

    int GetSlotIndex(GameObject slot)
    {
        return slots.IndexOf(slot);
    }

    bool IsPointerOverUIObject(GameObject uiObject)
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

            if (i < inventory.items.Count)
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

