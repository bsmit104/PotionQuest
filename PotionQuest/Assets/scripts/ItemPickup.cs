using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // Reference to the item scriptable object representing this item

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered!");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player collided!");
            // Find the inventory GameObject in the scene
            GameObject inventoryObject = GameObject.FindGameObjectWithTag("Inventory");
            if (inventoryObject != null)
            {
                // Get the Inventory component from the inventory GameObject
                Inventory playerInventory = inventoryObject.GetComponent<Inventory>();
                if (playerInventory != null)
                {
                    Debug.Log("Player has inventory!");
                    if (playerInventory.AddItem(item) == 1)
                    {
                        Destroy(gameObject); // Destroy the item GameObject after collecting
                        Debug.Log("Item added to inventory");
                        Debug.Log("Inventory Contents:");
                        foreach (var inventoryItem in playerInventory.items)
                        {
                            if (inventoryItem.item != null)
                            {
                                Debug.Log("Item: " + inventoryItem.item.itemName + ", Stack Size: " + inventoryItem.stackSize);
                            }else
                            {
                                Debug.Log("Item: null, Stack Size: 0");
                            }
                        }
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
        }
    }
}