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
    public InventoryUI inventoryUI;
    public Transform resultSlot; // The slot for the crafted item

    public List<Item> craftableItems; // List of all possible craftable items

    private List<GameObject> slots = new List<GameObject>();
    private GameObject draggedItem;
    private int draggedItemIndex = -1;
    private Item lastCraftedItem; // Track the last crafted item

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
        // Initialize alchemy slots
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

        // Initialize result slot for drag operations
        if (resultSlot != null)
        {
            EventTrigger resultSlotTrigger = resultSlot.gameObject.AddComponent<EventTrigger>();
            AddEventTrigger(resultSlotTrigger, EventTriggerType.BeginDrag, BeginDrag);
            AddEventTrigger(resultSlotTrigger, EventTriggerType.Drag, Drag);
            AddEventTrigger(resultSlotTrigger, EventTriggerType.EndDrag, EndDrag);
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

        // Check if dragging from resultSlot
        if (pointerData.pointerDrag == resultSlot.gameObject)
        {
            // Handle dragging from result slot
            Item itemToMove = lastCraftedItem;
            if (itemToMove != null)
            {
                draggedItem = CreateDraggedItemFromResultSlot(itemToMove);
                resultSlot.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }
        else
        {
            // Handle dragging from alchemy slots
            draggedItemIndex = GetSlotIndex(pointerData.pointerPress);
            if (draggedItemIndex >= 0 && draggedItemIndex < Alch.AlchItems.Count)
            {
                draggedItem = CreateDraggedItem(draggedItemIndex);
                slots[draggedItemIndex].GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
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

    GameObject CreateDraggedItemFromResultSlot(Item item)
    {
        GameObject itemObject = Instantiate(draggedItemPrefab);
        Image itemImage = itemObject.GetComponent<Image>();
        if (itemImage != null)
        {
            itemImage.sprite = item.itemIcon;
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
            if (resultSlot.GetComponent<CanvasGroup>() != null)
            {
                resultSlot.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }

            // Check if dropped in InventoryUI
            if (IsPointerOverUIObject(inventoryUI.slotPanel.gameObject))
            {
                if (lastCraftedItem != null)
                {
                    if (inventoryUI.inventory.AddItem(lastCraftedItem, 1)) // Add item to the inventory
                    {
                        // Remove used items from alch table
                        RemoveCraftedItemsFromAlch();

                        // Clear result slot after adding item to inventory
                        Transform iconTransform = resultSlot.Find("Icon");
                        Image iconImage = iconTransform?.GetComponent<Image>();
                        if (iconImage != null)
                        {
                            iconImage.sprite = null;
                        }
                        lastCraftedItem = null; // Clear the last crafted item
                    }
                }
                else
                {
                    if (draggedItemIndex >= 0 && draggedItemIndex < Alch.AlchItems.Count)
                    {
                        Item itemToMove = Alch.AlchItems[draggedItemIndex].item;
                        if (inventoryUI.inventory.AddItem(itemToMove, 1)) // Assuming you want to add the item to the inventory
                        {
                            Alch.RemoveItem(itemToMove, 1); // Remove item from the chest
                        }
                    }
                }
            }
            else
            {
                // Handle other drop cases if needed
            }

            Destroy(draggedItem);
            draggedItem = null;
            draggedItemIndex = -1;

            UpdateAlchUI();
            inventoryUI.UpdateInventoryUI(); // Update InventoryUI
        }
    }

    private void RemoveCraftedItemsFromAlch()
    {
        Dictionary<string, int> ingredientCounts = new Dictionary<string, int>();
        foreach (var itemStack in Alch.AlchItems)
        {
            if (ingredientCounts.ContainsKey(itemStack.item.itemName))
            {
                ingredientCounts[itemStack.item.itemName]++;
            }
            else
            {
                ingredientCounts[itemStack.item.itemName] = 1;
            }
        }

        // Define recipes and remove items
        if (ingredientCounts.ContainsKey("Lavender") && ingredientCounts.ContainsKey("Water") && ingredientCounts["Lavender"] == 1 && ingredientCounts["Water"] == 1)
        {
            Alch.RemoveItem(GetItemByName("Lavender"), 1);
            Alch.RemoveItem(GetItemByName("Water"), 1);
        }
        else if (ingredientCounts.ContainsKey("Fly Agaric") && ingredientCounts.ContainsKey("Water") && ingredientCounts["Fly Agaric"] == 1 && ingredientCounts["Water"] == 1)
        {
            Alch.RemoveItem(GetItemByName("Fly Agaric"), 1);
            Alch.RemoveItem(GetItemByName("Water"), 1);
        }
        else if (ingredientCounts.ContainsKey("Mandrake") && ingredientCounts.ContainsKey("Sulfuric Acid") && ingredientCounts.ContainsKey("Skull") && ingredientCounts.ContainsKey("Water")
            && ingredientCounts["Mandrake"] == 1 && ingredientCounts["Sulfuric Acid"] == 1 && ingredientCounts["Skull"] == 1 && ingredientCounts["Water"] == 1)
        {
            Alch.RemoveItem(GetItemByName("Mandrake"), 1);
            Alch.RemoveItem(GetItemByName("Sulfuric Acid"), 1);
            Alch.RemoveItem(GetItemByName("Skull"), 1);
            Alch.RemoveItem(GetItemByName("Water"), 1);
        }
        else if (ingredientCounts.ContainsKey("Fly Agaric") && ingredientCounts.ContainsKey("Mead") && ingredientCounts["Fly Agaric"] == 1 && ingredientCounts["Mead"] == 1)
        {
            Alch.RemoveItem(GetItemByName("Fly Agaric"), 1);
            Alch.RemoveItem(GetItemByName("Mead"), 1);
        }
        else if (ingredientCounts.ContainsKey("Gold Coin") && ingredientCounts.ContainsKey("Gold Bar") && ingredientCounts["Gold Coin"] == 1 && ingredientCounts["Gold Bar"] == 1)
        {
            Alch.RemoveItem(GetItemByName("Gold Coin"), 1);
            Alch.RemoveItem(GetItemByName("Gold Bar"), 1);
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

    void Update()
    {
        CheckCraftingResult(); // Continuously check the crafting result
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

    private void CheckCraftingResult()
    {
        Item craftedItem = GetCraftedItem();
        lastCraftedItem = craftedItem; // Update last crafted item

        if (craftedItem != null)
        {
            Transform iconTransform = resultSlot.Find("Icon");
            Image iconImage = iconTransform?.GetComponent<Image>();

            if (iconImage != null)
            {
                iconImage.sprite = craftedItem.itemIcon;
                iconImage.color = Color.white;
            }
        }
        else
        {
            Transform iconTransform = resultSlot.Find("Icon");
            Image iconImage = iconTransform?.GetComponent<Image>();

            if (iconImage != null)
            {
                iconImage.sprite = null;
            }
        }
    }

    private Item GetCraftedItem()
    {
        Dictionary<string, int> ingredientCounts = new Dictionary<string, int>();
        foreach (var itemStack in Alch.AlchItems)
        {
            if (ingredientCounts.ContainsKey(itemStack.item.itemName))
            {
                ingredientCounts[itemStack.item.itemName]++;
            }
            else
            {
                ingredientCounts[itemStack.item.itemName] = 1;
            }
        }

        // Check each recipe
        if (ingredientCounts.ContainsKey("Lavender") && ingredientCounts.ContainsKey("Water") && ingredientCounts["Lavender"] == 1 && ingredientCounts["Water"] == 1)
        {
            Debug.Log("light essence");
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
        if (ingredientCounts.ContainsKey("Gold Coin") && ingredientCounts.ContainsKey("Gold Bar") && ingredientCounts["Gold Coin"] == 1 && ingredientCounts["Gold Bar"] == 1)
        {
            return GetItemByName("Gold Essence");
        }

        return null;
    }

    private Item GetItemByName(string itemName)
    {
        foreach (var item in craftableItems)
        {
            if (item.itemName == itemName)
            {
                return item;
            }
        }
        Debug.LogError("Item not found: " + itemName);
        return null;
    }
}














// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.EventSystems;
// using TMPro;

// public class AlchUI : MonoBehaviour
// {
//     public Alch Alch;
//     public Transform slotPanel;
//     public GameObject draggedItemPrefab;
//     public InventoryUI inventoryUI;
//     public Transform resultSlot; // The slot for the crafted item

//     public List<Item> craftableItems; // List of all possible craftable items

//     private List<GameObject> slots = new List<GameObject>();
//     private GameObject draggedItem;
//     private int draggedItemIndex = -1;
//     private Item lastCraftedItem; // Track the last crafted item

//     void Start()
//     {
//         if (Alch == null)
//         {
//             Debug.LogError("Alch is not assigned!");
//             return;
//         }

//         Alch.OnAlchChanged += UpdateAlchUI;
//         InitializeSlots();
//         UpdateAlchUI();
//     }

//     void InitializeSlots()
//     {
//         // Initialize alchemy slots
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

//         // Initialize result slot for drag operations
//         if (resultSlot != null)
//         {
//             EventTrigger resultSlotTrigger = resultSlot.gameObject.AddComponent<EventTrigger>();
//             AddEventTrigger(resultSlotTrigger, EventTriggerType.BeginDrag, BeginDrag);
//             AddEventTrigger(resultSlotTrigger, EventTriggerType.Drag, Drag);
//             AddEventTrigger(resultSlotTrigger, EventTriggerType.EndDrag, EndDrag);
//         }
//     }

//     void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> action)
//     {
//         EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
//         entry.callback.AddListener(action);
//         trigger.triggers.Add(entry);
//     }

//     void OnSlotClick(BaseEventData data)
//     {
//         // Handle slot click logic if needed
//     }

//     public void BeginDrag(BaseEventData data)
//     {
//         PointerEventData pointerData = (PointerEventData)data;

//         // Check if dragging from resultSlot
//         if (pointerData.pointerDrag == resultSlot.gameObject)
//         {
//             // Handle dragging from result slot
//             Item itemToMove = lastCraftedItem;
//             if (itemToMove != null)
//             {
//                 draggedItem = CreateDraggedItemFromResultSlot(itemToMove);
//                 resultSlot.GetComponent<CanvasGroup>().blocksRaycasts = false;
//             }
//         }
//         else
//         {
//             // Handle dragging from alchemy slots
//             draggedItemIndex = GetSlotIndex(pointerData.pointerPress);
//             if (draggedItemIndex >= 0 && draggedItemIndex < Alch.AlchItems.Count)
//             {
//                 draggedItem = CreateDraggedItem(draggedItemIndex);
//                 slots[draggedItemIndex].GetComponent<CanvasGroup>().blocksRaycasts = false;
//             }
//         }
//     }

//     GameObject CreateDraggedItem(int index)
//     {
//         GameObject itemObject = Instantiate(draggedItemPrefab);
//         Image itemImage = itemObject.GetComponent<Image>();
//         if (itemImage != null)
//         {
//             itemImage.sprite = Alch.AlchItems[index].item.itemIcon;
//             itemImage.SetNativeSize();
//         }

//         // Set dragged item to the Canvas
//         Canvas canvas = FindObjectOfType<Canvas>();
//         if (canvas != null)
//         {
//             RectTransform rectTransform = itemObject.GetComponent<RectTransform>();
//             rectTransform.SetParent(canvas.transform, false);
//             rectTransform.anchoredPosition = Input.mousePosition; // Position it at the mouse cursor
//             rectTransform.sizeDelta = new Vector2(itemImage.sprite.rect.width, itemImage.sprite.rect.height); // Set size to sprite size
//         }
//         else
//         {
//             Debug.LogError("Canvas not found!");
//         }

//         return itemObject;
//     }

//     GameObject CreateDraggedItemFromResultSlot(Item item)
//     {
//         GameObject itemObject = Instantiate(draggedItemPrefab);
//         Image itemImage = itemObject.GetComponent<Image>();
//         if (itemImage != null)
//         {
//             itemImage.sprite = item.itemIcon;
//             itemImage.SetNativeSize();
//         }

//         // Set dragged item to the Canvas
//         Canvas canvas = FindObjectOfType<Canvas>();
//         if (canvas != null)
//         {
//             RectTransform rectTransform = itemObject.GetComponent<RectTransform>();
//             rectTransform.SetParent(canvas.transform, false);
//             rectTransform.anchoredPosition = Input.mousePosition; // Position it at the mouse cursor
//             rectTransform.sizeDelta = new Vector2(itemImage.sprite.rect.width, itemImage.sprite.rect.height); // Set size to sprite size
//         }
//         else
//         {
//             Debug.LogError("Canvas not found!");
//         }

//         return itemObject;
//     }

//     public void Drag(BaseEventData data)
//     {
//         if (draggedItem != null)
//         {
//             PointerEventData pointerData = (PointerEventData)data;
//             RectTransformUtility.ScreenPointToLocalPointInRectangle(
//                 (RectTransform)draggedItem.transform.parent,
//                 pointerData.position,
//                 null,
//                 out Vector2 localPoint
//             );
//             draggedItem.GetComponent<RectTransform>().anchoredPosition = localPoint;
//         }
//     }

//     public void EndDrag(BaseEventData data)
//     {
//         if (draggedItem != null)
//         {
//             if (resultSlot.GetComponent<CanvasGroup>() != null)
//             {
//                 resultSlot.GetComponent<CanvasGroup>().blocksRaycasts = true;
//             }

//             // Check if dropped in InventoryUI
//             if (IsPointerOverUIObject(inventoryUI.slotPanel.gameObject))
//             {
//                 if (lastCraftedItem != null)
//                 {
//                     if (inventoryUI.inventory.AddItem(lastCraftedItem, 1)) // Add item to the inventory
//                     {
//                         // Remove items used for crafting
//                         RemoveCraftedItemsFromAlch();

//                         // Clear the result slot after adding item to inventory
//                         Transform iconTransform = resultSlot.Find("Icon");
//                         Image iconImage = iconTransform?.GetComponent<Image>();
//                         if (iconImage != null)
//                         {
//                             iconImage.sprite = null;
//                         }
//                         lastCraftedItem = null; // Clear the last crafted item

//                         // Remove the result item from the alchemy table
//                         RemoveResultItemFromAlch();
//                     }
//                 }
//                 // else
//                 // {
//                 //     if (draggedItemIndex >= 0 && draggedItemIndex < Alch.AlchItems.Count)
//                 //     {
//                 //         Item itemToMove = Alch.AlchItems[draggedItemIndex].item;
//                 //         if (inventoryUI.inventory.AddItem(itemToMove, 1)) // Assuming you want to add the item to the inventory
//                 //         {
//                 //             Alch.RemoveItem(itemToMove, 1); // Remove item from the chest
//                 //         }
//                 //     }
//                 // }
//             }
//             else
//             {
//                 // Handle other drop cases if needed
//             }

//             Destroy(draggedItem);
//             draggedItem = null;
//             draggedItemIndex = -1;

//             UpdateAlchUI();
//             inventoryUI.UpdateInventoryUI(); // Update InventoryUI
//         }
//     }

//     // public void EndDrag(BaseEventData data)
//     // {
//     //     if (draggedItem != null)
//     //     {
//     //         slots[draggedItemIndex].GetComponent<CanvasGroup>().blocksRaycasts = true;

//     //         // Check if dropped in InventoryUI
//     // if (IsPointerOverUIObject(inventoryUI.slotPanel.gameObject))
//     // {
//     // if (draggedItemIndex >= 0 && draggedItemIndex < Alch.AlchItems.Count)
//     // {
//     //     Item itemToMove = Alch.AlchItems[draggedItemIndex].item;
//     //     if (inventoryUI.inventory.AddItem(itemToMove, 1)) // Assuming you want to add the item to the inventory
//     //     {
//     //         Alch.RemoveItem(itemToMove, 1); // Remove item from the chest
//     //     }
//     // }
//     // }

//     //         Destroy(draggedItem);
//     //         draggedItem = null;
//     //         draggedItemIndex = -1;

//     //         UpdateAlchUI();
//     //         inventoryUI.UpdateInventoryUI(); // Update InventoryUI
//     //     }
//     // }

//     private void RemoveResultItemFromAlch()
//     {
//         if (lastCraftedItem != null)
//         {
//             // Ensure the item is present before removing
//             if (Alch.HasItem(lastCraftedItem))
//             {
//                 Alch.RemoveItem(lastCraftedItem, 1);
//             }
//         }
//     }

//     private void RemoveCraftedItemsFromAlch()
//     {
//         Dictionary<string, int> ingredientCounts = new Dictionary<string, int>();
//         foreach (var itemStack in Alch.AlchItems)
//         {
//             if (ingredientCounts.ContainsKey(itemStack.item.itemName))
//             {
//                 ingredientCounts[itemStack.item.itemName]++;
//             }
//             else
//             {
//                 ingredientCounts[itemStack.item.itemName] = 1;
//             }
//         }

//         // Check and remove items based on recipes
//         TryRemoveItem("Lavender", 1, "Water", 1);
//         TryRemoveItem("Fly Agaric", 1, "Water", 1);
//         TryRemoveItem("Mandrake", 1, "Sulfuric Acid", 1, "Skull", 1, "Water", 1);
//         TryRemoveItem("Fly Agaric", 1, "Mead", 1);
//     }

//     private void TryRemoveItem(string item1, int quantity1, string item2 = null, int quantity2 = 0, string item3 = null, int quantity3 = 0, string item4 = null, int quantity4 = 0)
//     {
//         bool canRemove = true;

//         if (Alch.HasItem(GetItemByName(item1)) && Alch.GetItemQuantity(GetItemByName(item1)) >= quantity1)
//         {
//             Alch.RemoveItem(GetItemByName(item1), quantity1);
//         }
//         else
//         {
//             canRemove = false;
//         }

//         if (item2 != null && Alch.HasItem(GetItemByName(item2)) && Alch.GetItemQuantity(GetItemByName(item2)) >= quantity2)
//         {
//             Alch.RemoveItem(GetItemByName(item2), quantity2);
//         }
//         else
//         {
//             canRemove = false;
//         }

//         if (item3 != null && Alch.HasItem(GetItemByName(item3)) && Alch.GetItemQuantity(GetItemByName(item3)) >= quantity3)
//         {
//             Alch.RemoveItem(GetItemByName(item3), quantity3);
//         }
//         else
//         {
//             canRemove = false;
//         }

//         if (item4 != null && Alch.HasItem(GetItemByName(item4)) && Alch.GetItemQuantity(GetItemByName(item4)) >= quantity4)
//         {
//             Alch.RemoveItem(GetItemByName(item4), quantity4);
//         }
//         else
//         {
//             canRemove = false;
//         }
//     }

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

//     void Update()
//     {
//         CheckCraftingResult(); // Continuously check the crafting result
//     }

//     public void UpdateAlchUI()
//     {
//         for (int i = 0; i < slots.Count; i++)
//         {
//             Transform iconTransform = slots[i].transform.Find("Icon");
//             Transform countTransform = slots[i].transform.Find("Count");

//             Image iconImage = iconTransform?.GetComponent<Image>();
//             TMP_Text countText = countTransform?.GetComponent<TMP_Text>();

//             if (i < Alch.AlchItems.Count)
//             {
//                 if (iconImage != null)
//                 {
//                     iconImage.sprite = Alch.AlchItems[i].item.itemIcon;
//                     iconImage.color = Color.white;
//                 }
//                 if (countText != null)
//                 {
//                     countText.text = Alch.AlchItems[i].stackSize.ToString();
//                 }
//             }
//             else
//             {
//                 if (iconImage != null) iconImage.sprite = null;
//                 if (countText != null) countText.text = "";
//             }
//         }
//     }

//     private void CheckCraftingResult()
//     {
//         Item craftedItem = GetCraftedItem();
//         lastCraftedItem = craftedItem; // Update last crafted item

//         if (craftedItem != null)
//         {
//             Transform iconTransform = resultSlot.Find("Icon");
//             Image iconImage = iconTransform?.GetComponent<Image>();

//             if (iconImage != null)
//             {
//                 iconImage.sprite = craftedItem.itemIcon;
//                 iconImage.color = Color.white;
//             }
//         }
//         else
//         {
//             Transform iconTransform = resultSlot.Find("Icon");
//             Image iconImage = iconTransform?.GetComponent<Image>();

//             if (iconImage != null)
//             {
//                 iconImage.sprite = null;
//             }
//         }
//     }

//     private Item GetCraftedItem()
//     {
//         Dictionary<string, int> ingredientCounts = new Dictionary<string, int>();
//         foreach (var itemStack in Alch.AlchItems)
//         {
//             if (ingredientCounts.ContainsKey(itemStack.item.itemName))
//             {
//                 ingredientCounts[itemStack.item.itemName]++;
//             }
//             else
//             {
//                 ingredientCounts[itemStack.item.itemName] = 1;
//             }
//         }

//         // Check each recipe
//         if (ingredientCounts.ContainsKey("Lavender") && ingredientCounts.ContainsKey("Water") && ingredientCounts["Lavender"] == 1 && ingredientCounts["Water"] == 1)
//         {
//             Debug.Log("light essence");
//             return GetItemByName("Light Essence");
//         }
//         if (ingredientCounts.ContainsKey("Fly Agaric") && ingredientCounts.ContainsKey("Water") && ingredientCounts["Fly Agaric"] == 1 && ingredientCounts["Water"] == 1)
//         {
//             return GetItemByName("Goblin Tonic");
//         }
//         if (ingredientCounts.ContainsKey("Mandrake") && ingredientCounts.ContainsKey("Sulfuric Acid") && ingredientCounts.ContainsKey("Skull") && ingredientCounts.ContainsKey("Water")
//             && ingredientCounts["Mandrake"] == 1 && ingredientCounts["Sulfuric Acid"] == 1 && ingredientCounts["Skull"] == 1 && ingredientCounts["Water"] == 1)
//         {
//             return GetItemByName("Corrosive Elixir");
//         }
//         if (ingredientCounts.ContainsKey("Fly Agaric") && ingredientCounts.ContainsKey("Mead") && ingredientCounts["Fly Agaric"] == 1 && ingredientCounts["Mead"] == 1)
//         {
//             return GetItemByName("Strength Nectar");
//         }

//         return null;
//     }

//     private Item GetItemByName(string itemName)
//     {
//         foreach (var item in craftableItems)
//         {
//             if (item.itemName == itemName)
//             {
//                 return item;
//             }
//         }
//         Debug.LogError("Item not found: " + itemName);
//         return null;
//     }
// }