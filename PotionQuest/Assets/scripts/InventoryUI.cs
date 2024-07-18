using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private Inventory inventory;
    public Transform slotPanel;

    private List<GameObject> slots = new List<GameObject>();

    void Start()
    {
        Debug.Log("Initializing inventory UI...");

        if (inventory == null)
        {
            Debug.LogError("Inventory is not assigned!");
            return;
        }

        inventory.OnInventoryChanged += UpdateInventoryUI;

        InitializeSlots();
        UpdateInventoryUI();
    }

    void InitializeSlots()
    {
        Debug.Log("Initializing slots...");
        for (int i = 0; i < slotPanel.childCount; i++)
        {
            GameObject slot = slotPanel.GetChild(i).gameObject;
            slots.Add(slot);
        }
        Debug.Log("Slots initialized. Total slots: " + slots.Count);
    }

    public void UpdateInventoryUI()
    {
        Debug.Log("Updating inventory UI...");
        Debug.Log("Current slots count: " + slots.Count);

        for (int i = 0; i < slots.Count; i++)
        {
            Transform iconTransform = slots[i].transform.Find("Icon");
            Transform countTransform = slots[i].transform.Find("Count");

            if (iconTransform == null)
            {
                Debug.LogError("Icon child object not found in slot prefab!");
            }
            if (countTransform == null)
            {
                Debug.LogError("Count child object not found in slot prefab!");
            }

            Image iconImage = iconTransform?.GetComponent<Image>();
            TMP_Text countText = countTransform?.GetComponent<TMP_Text>();

            if (iconImage == null)
            {
                Debug.LogError("Icon Image component not found in slot prefab!");
            }
            if (countText == null)
            {
                Debug.LogError("Count TMP_Text component not found in slot prefab!");
            }

            if (i < inventory.items.Count)
            {
                if (iconImage != null)
                {
                    iconImage.color = Color.white; // Ensure the color is set to white
                    iconImage.sprite = inventory.items[i].item.itemIcon;
                }
                if (countText != null)
                {
                    countText.text = inventory.items[i].stackSize.ToString();
                }
            }
            else
            {
                if (iconImage != null)
                {
                    iconImage.sprite = null;
                }
                if (countText != null)
                {
                    countText.text = "";
                }
            }
        }

        Debug.Log("Inventory UI updated. Total slots: " + slots.Count);
    }

    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= UpdateInventoryUI;
        }
    }
}