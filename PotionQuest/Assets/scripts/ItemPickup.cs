using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemPickup : MonoBehaviour
{
    public Item item; // Reference to the item scriptable object representing this item

    public bool CanBePickedUp = true;
    public bool destroyOnCollect = true; // Checkbox to determine if the item should be destroyed upon collection

    private TMP_Text hintText; // Reference to the TextMeshPro text component

    private bool isPlayerInRange = false; // Track if the player is in range

    private void Start()
    {
        // Find the TextMeshPro text component in the scene by name
        hintText = GameObject.Find("hintText").GetComponent<TMP_Text>();
        
        if (hintText != null)
        {
            hintText.text = "";
        }
        else
        {
            Debug.LogWarning("Hint Text GameObject not found!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (hintText != null)
            {
                // Display the feedback message when the player is in range
                hintText.text = "Press E to collect [" + item.itemName + "]";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (hintText != null)
            {
                // Clear the feedback message when the player leaves the trigger area
                hintText.text = "";
            }
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && CanBePickedUp)
        {
            CollectItem();
        }
    }

    private void CollectItem()
    {
        GameObject inventoryObject = GameObject.FindGameObjectWithTag("Inventory");
        if (inventoryObject != null)
        {
            Inventory playerInventory = inventoryObject.GetComponent<Inventory>();
            if (playerInventory != null)
            {
                if (playerInventory.AddItem(item) == 1)
                {
                    if (destroyOnCollect)
                    {
                        Destroy(gameObject); // Destroy the item GameObject after collecting if destroyOnCollect is true
                    }
                    Debug.Log("Item added to inventory");
                }
                else
                {
                    Debug.Log("Inventory is full, cannot add item");
                }
            }
            else
            {
                Debug.Log("Inventory component not found!");
            }
        }
        else
        {
            Debug.Log("Inventory GameObject not found!");
        }

        // Clear the feedback message after collecting the item
        if (hintText != null)
        {
            hintText.text = "";
        }
    }
}



// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;

// public class ItemPickup : MonoBehaviour
// {
//     public Item item; // Reference to the item scriptable object representing this item

//     public bool CanBePickedUp = true;
//     public bool destroyOnCollect = true; // Checkbox to determine if the item should be destroyed upon collection

//     public TMP_Text feedbackText; // Reference to the TextMeshPro text component
//     public string feedbackMessage = ""; // Custom feedback message

//     private bool isPlayerInRange = false; // Track if the player is in range

//     private void Start()
//     {
//         if (feedbackText != null)
//         {
//             feedbackText.text = "";
//         }
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             isPlayerInRange = true;
//             if (!destroyOnCollect && feedbackText != null)
//             {
//                 // Display the feedback message if the item is not destroyable
//                 feedbackText.text = "Press E to collect [" + item.itemName + "]";
//             }
//         }
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             isPlayerInRange = false;
//             if (feedbackText != null)
//             {
//                 // Clear the feedback message when the player leaves the trigger area
//                 feedbackText.text = "";
//             }
//         }
//     }

//     private void Update()
//     {
//         if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && CanBePickedUp)
//         {
//             CollectItem();
//         }
//     }

//     private void CollectItem()
//     {
//         GameObject inventoryObject = GameObject.FindGameObjectWithTag("Inventory");
//         if (inventoryObject != null)
//         {
//             Inventory playerInventory = inventoryObject.GetComponent<Inventory>();
//             if (playerInventory != null)
//             {
//                 if (playerInventory.AddItem(item) == 1)
//                 {
//                     if (destroyOnCollect)
//                     {
//                         Destroy(gameObject); // Destroy the item GameObject after collecting if destroyOnCollect is true
//                     }
//                     Debug.Log("Item added to inventory");
//                 }
//                 else
//                 {
//                     Debug.Log("Inventory is full, cannot add item");
//                 }
//             }
//             else
//             {
//                 Debug.Log("Inventory component not found!");
//             }
//         }
//         else
//         {
//             Debug.Log("Inventory GameObject not found!");
//         }
//     }
// }

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ItemPickup : MonoBehaviour
// {
//     public Item item; // Reference to the item scriptable object representing this item

//     public bool CanBePickedUp = true;
//     public bool destroyOnCollect = true; // Checkbox to determine if the item should be destroyed upon collection

//     private void OnTriggerEnter(Collider other)
//     {
//         Debug.Log("Trigger entered!");
//         if (!CanBePickedUp) return;
//         if (other.CompareTag("Player"))
//         {
//             Debug.Log("Player collided!");
//             // Find the inventory GameObject in the scene
//             GameObject inventoryObject = GameObject.FindGameObjectWithTag("Inventory");
//             if (inventoryObject != null)
//             {
//                 // Get the Inventory component from the inventory GameObject
//                 Inventory playerInventory = inventoryObject.GetComponent<Inventory>();
//                 if (playerInventory != null)
//                 {
//                     Debug.Log("Player has inventory!");
//                     if (playerInventory.AddItem(item) == 1)
//                     {
//                         if (destroyOnCollect)
//                         {
//                             Destroy(gameObject); // Destroy the item GameObject after collecting if destroyOnCollect is true
//                         }
//                         Debug.Log("Item added to inventory");
//                         Debug.Log("Inventory Contents:");
//                         foreach (var inventoryItem in playerInventory.items)
//                         {
//                             if (inventoryItem.item != null)
//                             {
//                                 Debug.Log("Item: " + inventoryItem.item.itemName + ", Stack Size: " + inventoryItem.stackSize);
//                             }
//                             else
//                             {
//                                 Debug.Log("Item: null, Stack Size: 0");
//                             }
//                         }
//                     }
//                     else
//                     {
//                         Debug.Log("Inventory is full, cannot add item");
//                     }
//                 }
//                 else
//                 {
//                     Debug.Log("Inventory component not found!");
//                 }
//             }
//             else
//             {
//                 Debug.Log("Inventory GameObject not found!");
//             }
//         }
//     }
// }

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ItemPickup : MonoBehaviour
// {
//     public Item item; // Reference to the item scriptable object representing this item

//     public bool CanBePickedUp = true;

//     private void OnTriggerEnter(Collider other)
//     {
//         Debug.Log("Trigger entered!");
//         if (!CanBePickedUp) return;
//         if (other.CompareTag("Player"))
//         {
//             Debug.Log("Player collided!");
//             // Find the inventory GameObject in the scene
//             GameObject inventoryObject = GameObject.FindGameObjectWithTag("Inventory");
//             if (inventoryObject != null)
//             {
//                 // Get the Inventory component from the inventory GameObject
//                 Inventory playerInventory = inventoryObject.GetComponent<Inventory>();
//                 if (playerInventory != null)
//                 {
//                     Debug.Log("Player has inventory!");
//                     if (playerInventory.AddItem(item) == 1)
//                     {
//                         Destroy(gameObject); // Destroy the item GameObject after collecting
//                         Debug.Log("Item added to inventory");
//                         Debug.Log("Inventory Contents:");
//                         foreach (var inventoryItem in playerInventory.items)
//                         {
//                             if (inventoryItem.item != null)
//                             {
//                                 Debug.Log("Item: " + inventoryItem.item.itemName + ", Stack Size: " + inventoryItem.stackSize);
//                             }else
//                             {
//                                 Debug.Log("Item: null, Stack Size: 0");
//                             }
//                         }
//                     }
//                     else
//                     {
//                         Debug.Log("Inventory is full, cannot add item");
//                     }
//                 }
//                 else
//                 {
//                     Debug.Log("Inventory component not found!");
//                 }
//             }
//             else
//             {
//                 Debug.Log("Inventory GameObject not found!");
//             }
//         }
//     }
// }