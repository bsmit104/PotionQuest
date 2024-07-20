using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ChestUI : MonoBehaviour
{
    public Chest chest;
    public Transform slotPanel;
    public GameObject draggedItemPrefab;

    private List<GameObject> slots = new List<GameObject>();
    private GameObject draggedItem;
    private int draggedItemIndex = -1;

    void Start()
    {
        if (chest == null)
        {
            Debug.LogError("Chest is not assigned!");
            return;
        }

        chest.OnChestChanged += UpdateChestUI;
        InitializeSlots();
        UpdateChestUI();
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
        if (draggedItemIndex >= 0 && draggedItemIndex < chest.chestItems.Count)
        {
            draggedItem = CreateDraggedItem(draggedItemIndex);
            slots[draggedItemIndex].GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    GameObject CreateDraggedItem(int index)
    {
        GameObject itemObject = Instantiate(draggedItemPrefab);
        Image itemImage = itemObject.GetComponent<Image>();
        itemImage.sprite = chest.chestItems[index].item.itemIcon;
        itemImage.SetNativeSize();
        Canvas canvas = FindObjectOfType<Canvas>();
        itemObject.transform.SetParent(canvas.transform, false);
        itemObject.transform.SetAsLastSibling();
        return itemObject;
    }

    public void Drag(BaseEventData data)
    {
        if (draggedItem != null)
        {
            PointerEventData pointerData = (PointerEventData)data;
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(pointerData.position);
            newPosition.z = 0;
            draggedItem.transform.position = newPosition;
        }
    }

    public void EndDrag(BaseEventData data)
    {
        if (draggedItem != null)
        {
            slots[draggedItemIndex].GetComponent<CanvasGroup>().blocksRaycasts = true;

            // if (IsPointerOverUIObject(trashCanImage.gameObject))
            // {
            //     if (draggedItemIndex >= 0 && draggedItemIndex < chest.chestItems.Count)
            //     {
            //         chest.RemoveItem(chest.chestItems[draggedItemIndex].item, 1);
            //     }
            // }

            Destroy(draggedItem);
            draggedItem = null;
            draggedItemIndex = -1;

            UpdateChestUI();
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

    public void UpdateChestUI()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            Transform iconTransform = slots[i].transform.Find("Icon");
            Transform countTransform = slots[i].transform.Find("Count");

            Image iconImage = iconTransform?.GetComponent<Image>();
            TMP_Text countText = countTransform?.GetComponent<TMP_Text>();

            if (i < chest.chestItems.Count)
            {
                if (iconImage != null)
                {
                    iconImage.sprite = chest.chestItems[i].item.itemIcon;
                    iconImage.color = Color.white;
                }
                if (countText != null)
                {
                    countText.text = chest.chestItems[i].stackSize.ToString();
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
        if (chest != null)
        {
            chest.OnChestChanged -= UpdateChestUI;
        }
    }
}