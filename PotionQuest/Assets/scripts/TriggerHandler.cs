using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    public GameObject closed;
    public GameObject open;
    public string itemName = "Strength Potion";
    public Inventory inventory; // Public Inventory field

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (inventory != null && inventory.GetItemByName(itemName) != null)
            {
                closed.SetActive(false);
                open.SetActive(true);
            }
        }
    }
}
