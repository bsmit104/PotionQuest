using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class AlchUI : MonoBehaviour
{
    public Alch Alch;
    public Transform slotPanel;
    public GameObject draggedItemPrefab;
    public InventoryUI inventoryUI; // Reference to InventoryUI

    private List<GameObject> slots = new List<GameObject>();
    private GameObject draggedItem;
    private int draggedItemIndex = -1;

    void Start()
    {
        if (Alch == null)
        {
            Debug.LogError("Alch is not assigned!");
            return;
        }

        Alch.OnAlchChanged += UpdateAlchUI;
        InitializeSlots();
        UpdateAlchUI();
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
        if (draggedItemIndex >= 0 && draggedItemIndex < Alch.AlchItems.Count)
        {
            draggedItem = CreateDraggedItem(draggedItemIndex);
            slots[draggedItemIndex].GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    GameObject CreateDraggedItem(int index)
    {
        GameObject itemObject = Instantiate(draggedItemPrefab);
        Image itemImage = itemObject.GetComponent<Image>();
        if (itemImage != null)
        {
            itemImage.sprite = Alch.AlchItems[index].item.itemIcon;
            itemImage.SetNativeSize();
        }

        // Set dragged item to the Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            RectTransform rectTransform = itemObject.GetComponent<RectTransform>();
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

    public void EndDrag(BaseEventData data)
    {
        if (draggedItem != null)
        {
            slots[draggedItemIndex].GetComponent<CanvasGroup>().blocksRaycasts = true;

            // Check if dropped in InventoryUI
            if (IsPointerOverUIObject(inventoryUI.slotPanel.gameObject))
            {
                if (draggedItemIndex >= 0 && draggedItemIndex < Alch.AlchItems.Count)
                {
                    Item itemToMove = Alch.AlchItems[draggedItemIndex].item;
                    if (inventoryUI.inventory.AddItem(itemToMove, 1)) // Assuming you want to add the item to the inventory
                    {
                        Alch.RemoveItem(itemToMove, 1); // Remove item from the Alch
                    }
                }
            }

            Destroy(draggedItem);
            draggedItem = null;
            draggedItemIndex = -1;

            UpdateAlchUI();
            inventoryUI.UpdateInventoryUI(); // Update InventoryUI
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

    public void UpdateAlchUI()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            Transform iconTransform = slots[i].transform.Find("Icon");
            Transform countTransform = slots[i].transform.Find("Count");

            Image iconImage = iconTransform?.GetComponent<Image>();
            TMP_Text countText = countTransform?.GetComponent<TMP_Text>();

            if (i < Alch.AlchItems.Count)
            {
                if (iconImage != null)
                {
                    iconImage.sprite = Alch.AlchItems[i].item.itemIcon;
                    iconImage.color = Color.white;
                }
                if (countText != null)
                {
                    countText.text = Alch.AlchItems[i].stackSize.ToString();
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
        if (Alch != null)
        {
            Alch.OnAlchChanged -= UpdateAlchUI;
        }
    }
}