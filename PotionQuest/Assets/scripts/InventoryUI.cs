using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public ChestUI chestUI;
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

            // Clean up the dragged item object
            Destroy(draggedItem);
            draggedItem = null;
            draggedItemIndex = -1;

            // Update the UI to reflect changes
            UpdateInventoryUI();
            chestUI.UpdateChestUI();
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

// public class InventoryUI : MonoBehaviour
// {
//     [SerializeField]
//     private Inventory inventory;
//     public Transform slotPanel;
//     public Image trashCanImage; // Assign your trash can image in the editor
//     //public Transform playerHeadTransform; // Assign the player's head transform in the editor
//     // public GameObject selectedItemDisplayPrefab; // Prefab for displaying selected item above the player's head
//     public GameObject draggedItemPrefab; // Prefab for displaying dragged item

//     private List<GameObject> slots = new List<GameObject>();
//     //private GameObject selectedItemDisplay;
//     private GameObject draggedItem;
//     private int draggedItemIndex = -1;

//     [System.Obsolete]
//     void Start()
//     {
//         Debug.Log("Initializing inventory UI...");

//         if (inventory == null)
//         {
//             Debug.LogError("Inventory is not assigned!");
//             return;
//         }

//         inventory.OnInventoryChanged += UpdateInventoryUI;
//         // inventory.OnSelectedItemChanged += UpdateSelectedItemDisplay;

//         InitializeSlots();
//         UpdateInventoryUI();
//     }

//     [System.Obsolete]
//     void InitializeSlots()
//     {
//         Debug.Log("Initializing slots...");
//         for (int i = 0; i < slotPanel.childCount; i++)
//         {
//             GameObject slot = slotPanel.GetChild(i).gameObject;
//             slots.Add(slot);

//             if (slot.GetComponent<CanvasGroup>() == null)
//             {
//                 slot.AddComponent<CanvasGroup>();
//             }

//             EventTrigger trigger = slot.AddComponent<EventTrigger>();
//             AddEventTrigger(trigger, EventTriggerType.PointerClick, OnSlotClick);
//             AddEventTrigger(trigger, EventTriggerType.BeginDrag, BeginDrag);
//             AddEventTrigger(trigger, EventTriggerType.Drag, Drag);
//             AddEventTrigger(trigger, EventTriggerType.EndDrag, EndDrag);
//         }
//         Debug.Log("Slots initialized. Total slots: " + slots.Count);
//     }

//     void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> action)
//     {
//         EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
//         entry.callback.AddListener(action);
//         trigger.triggers.Add(entry);
//     }

//     void OnSlotClick(BaseEventData data)
//     {
//         PointerEventData pointerData = (PointerEventData)data;
//         int slotIndex = GetSlotIndex(pointerData.pointerPress);
//         inventory.SelectItem(slotIndex);
//     }

//     public void BeginDrag(BaseEventData data)
//     {
//         PointerEventData pointerData = (PointerEventData)data;
//         draggedItemIndex = GetSlotIndex(pointerData.pointerPress);
//         if (draggedItemIndex >= 0 && draggedItemIndex < inventory.items.Count)
//         {
//             draggedItem = CreateDraggedItem(draggedItemIndex);
//             slots[draggedItemIndex].GetComponent<CanvasGroup>().blocksRaycasts = false;
//         }
//     }

//     GameObject CreateDraggedItem(int index)
//     {
//         Debug.Log("Creating dragged item...");
//         GameObject itemObject = Instantiate(draggedItemPrefab);
//         Image itemImage = itemObject.GetComponent<Image>();
//         itemImage.sprite = inventory.items[index].item.itemIcon;
//         itemImage.SetNativeSize();
//         Canvas canvas = FindFirstObjectByType<Canvas>();
//         itemObject.transform.SetParent(canvas.transform, false);
//         itemObject.transform.SetAsLastSibling();
//         return itemObject;
//     }

//     public void Drag(BaseEventData data)
//     {
//         if (draggedItem != null)
//         {
//             PointerEventData pointerData = (PointerEventData)data;
//             Vector3 newPosition = Camera.main.ScreenToWorldPoint(pointerData.position);
//             newPosition.z = 0; // Assuming your dragged items should be on the same z-plane
//             draggedItem.transform.position = newPosition;
//         }
//     }

//     // [System.Obsolete]
//     // Add a reference to the ChestUI
// public ChestUI chestUI;
// [System.Obsolete]
// public void EndDrag(BaseEventData data)
// {
//     if (draggedItem != null)
//     {
//         slots[draggedItemIndex].GetComponent<CanvasGroup>().blocksRaycasts = true;

//         if (IsPointerOverUIObject(trashCanImage.gameObject))
//         {
//             if (draggedItemIndex >= 0 && draggedItemIndex < inventory.items.Count)
//             {
//                 inventory.RemoveItem(inventory.items[draggedItemIndex].item, 1);
//                 inventory.SelectItem(-1);
//             }
//         }
//         else if (IsPointerOverUIObject(chestUI.slotPanel.gameObject))
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
//     // public void EndDrag(BaseEventData data)
//     // {
//     //     if (draggedItem != null)
//     //     {
//     //         slots[draggedItemIndex].GetComponent<CanvasGroup>().blocksRaycasts = true;

//     //         if (IsPointerOverUIObject(trashCanImage.gameObject))
//     //         {
//     //             Debug.Log("Item dropped on trash can.");
//     //             if (draggedItemIndex >= 0 && draggedItemIndex < inventory.items.Count)
//     //             {
//     //                 inventory.RemoveItem(inventory.items[draggedItemIndex].item, 1);  // Always remove one item
//     //                 inventory.SelectItem(-1); // Deselect the item after removing it
//     //             }
//     //         }

//     //         Destroy(draggedItem);
//     //         draggedItem = null;
//     //         draggedItemIndex = -1;

//     //         UpdateInventoryUI();
//     //     }
//     // }

//     int GetSlotIndex(GameObject slot)
//     {
//         return slots.IndexOf(slot);
//     }

//     bool IsPointerOverUIObject(GameObject uiObject)
//     {
//         PointerEventData pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
//         List<RaycastResult> results = new List<RaycastResult>();
//         EventSystem.current.RaycastAll(pointerEventData, results);
//         return results.Exists(result => result.gameObject == uiObject);
//     }

//     public void UpdateInventoryUI()
//     {
//         Debug.Log("Updating inventory UI...");
//         for (int i = 0; i < slots.Count; i++)
//         {
//             Transform iconTransform = slots[i].transform.Find("Icon");
//             Transform countTransform = slots[i].transform.Find("Count");

//             Image iconImage = iconTransform?.GetComponent<Image>();
//             TMP_Text countText = countTransform?.GetComponent<TMP_Text>();

//             if (i < inventory.items.Count)
//             {
//                 if (iconImage != null)
//                 {
//                     iconImage.sprite = inventory.items[i].item.itemIcon;
//                     iconImage.color = Color.white;
//                 }
//                 if (countText != null)
//                 {
//                     countText.text = inventory.items[i].stackSize.ToString();
//                 }
//             }
//             else
//             {
//                 if (iconImage != null) iconImage.sprite = null;
//                 if (countText != null) countText.text = "";
//             }
//         }
//     }

//     // public void UpdateSelectedItemDisplay(Item selectedItem)
//     // {
//     //     // Destroy the previous selectedItemDisplay if it exists
//     //     if (selectedItemDisplay != null)
//     //     {
//     //         Destroy(selectedItemDisplay);
//     //         selectedItemDisplay = null;
//     //     }

//     //     // Check if a new item is selected
//     //     if (selectedItem != null)
//     //     {
//     //         // Instantiate a new selectedItemDisplay
//     //         // selectedItemDisplay = Instantiate(selectedItemDisplayPrefab);
//     //         // Image itemImage = selectedItemDisplay.GetComponent<Image>();
//     //         // if (itemImage != null)
//     //         // {
//     //         //     itemImage.sprite = selectedItem.itemIcon;
//     //         // }

//     //         // // Set the parent to the player's head transform
//     //         // selectedItemDisplay.transform.SetParent(playerHeadTransform);

//     //         // // Set position relative to the player's head
//     //         // selectedItemDisplay.transform.localPosition = new Vector3(0, 2, 0); // Adjust position above the player's head

//     //         // // Ensure the item is positioned in world space
//     //         // selectedItemDisplay.transform.localPosition = Vector3.zero; // Reset local position
//     //         // selectedItemDisplay.transform.localRotation = Quaternion.identity; // Reset local rotation
//     //         // selectedItemDisplay.transform.localScale = Vector3.one; // Reset local scale
//     //     }
//     // }

//     private void OnDestroy()
//     {
//         if (inventory != null)
//         {
//             inventory.OnInventoryChanged -= UpdateInventoryUI;
//             // inventory.OnSelectedItemChanged -= UpdateSelectedItemDisplay;
//         }
//     }
// }


///////////////////works//////////////
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;

// public class InventoryUI : MonoBehaviour
// {
//     [SerializeField]
//     private Inventory inventory;
//     public Transform slotPanel;

//     private List<GameObject> slots = new List<GameObject>();

//     void Start()
//     {
//         Debug.Log("Initializing inventory UI...");

//         if (inventory == null)
//         {
//             Debug.LogError("Inventory is not assigned!");
//             return;
//         }

//         inventory.OnInventoryChanged += UpdateInventoryUI;

//         InitializeSlots();
//         UpdateInventoryUI();
//     }

//     void InitializeSlots()
//     {
//         Debug.Log("Initializing slots...");
//         for (int i = 0; i < slotPanel.childCount; i++)
//         {
//             GameObject slot = slotPanel.GetChild(i).gameObject;
//             slots.Add(slot);
//         }
//         Debug.Log("Slots initialized. Total slots: " + slots.Count);
//     }

//     public void UpdateInventoryUI()
//     {
//         Debug.Log("Updating inventory UI...");
//         Debug.Log("Current slots count: " + slots.Count);

//         for (int i = 0; i < slots.Count; i++)
//         {
//             Transform iconTransform = slots[i].transform.Find("Icon");
//             Transform countTransform = slots[i].transform.Find("Count");

//             if (iconTransform == null)
//             {
//                 Debug.LogError("Icon child object not found in slot prefab!");
//             }
//             if (countTransform == null)
//             {
//                 Debug.LogError("Count child object not found in slot prefab!");
//             }

//             Image iconImage = iconTransform?.GetComponent<Image>();
//             TMP_Text countText = countTransform?.GetComponent<TMP_Text>();

//             if (iconImage == null)
//             {
//                 Debug.LogError("Icon Image component not found in slot prefab!");
//             }
//             if (countText == null)
//             {
//                 Debug.LogError("Count TMP_Text component not found in slot prefab!");
//             }

//             if (i < inventory.items.Count)
//             {
//                 if (iconImage != null)
//                 {
//                     iconImage.color = Color.white; // Ensure the color is set to white
//                     iconImage.sprite = inventory.items[i].item.itemIcon;
//                 }
//                 if (countText != null)
//                 {
//                     countText.text = inventory.items[i].stackSize.ToString();
//                 }
//             }
//             else
//             {
//                 if (iconImage != null)
//                 {
//                     iconImage.sprite = null;
//                 }
//                 if (countText != null)
//                 {
//                     countText.text = "";
//                 }
//             }
//         }

//         Debug.Log("Inventory UI updated. Total slots: " + slots.Count);
//     }

//     private void OnDestroy()
//     {
//         if (inventory != null)
//         {
//             inventory.OnInventoryChanged -= UpdateInventoryUI;
//         }
//     }
// }