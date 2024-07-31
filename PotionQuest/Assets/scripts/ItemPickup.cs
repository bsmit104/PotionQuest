using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPickup : Interactable
{
    private PlayerInventory playerInventory;

    public Item item;
    public bool ConsumeItem = true;

    public override string GetHoveredText()
    {
        return "Pick up " + item.itemName;
    }

    private void Start() {
        playerInventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<PlayerInventory>();
        //leftclick, rightclick, and e all work
        OnInteract += Activate;
    }

    private void Activate()
    {
        //Debug.Log("trying to add item to inventory");
        float value = 1;
        if (TryGetComponent<Potion>(out Potion pot))
        {
            value = pot.Amount;
        }

        int added = playerInventory.AddItemToSlot(playerInventory.selectedSlot, item, 1, value);
        if (added == 0)
        {
            //failed add item to specific slot
            added = playerInventory.AddItem(item, 1, value);
            if (added != 0)
            {
                //add item worked
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().PlayPickupSound();
                if (ConsumeItem)
                    Destroy(this.gameObject);
                return;
            }
        }else
        {
            //we added the item to the slot
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().PlayPickupSound();
            if (ConsumeItem)
                Destroy(this.gameObject);
            
        }
    }
}

