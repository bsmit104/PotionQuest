using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemRequiringInteractable : Interactable
{
    public PlayerInventory playerInventory;

    public Item itemRequired;
    //public int AmountRequired = 1;
    public bool ConsumeItem = true;

    public List<GameObject> EnableOnUse = new List<GameObject>();
    public List<GameObject> DisableOnUse = new List<GameObject>();

    public string NeedItemMessage = "Looks like I need a ____";
    public string HaveItemMessage = "Use ____?";

    public override string GetHoveredText()
    {
        if (playerInventory.GetItemByName(itemRequired.itemName) != null)
        {
            return HaveItemMessage;
        }else
        {
            return NeedItemMessage;
        }
    }

    private void Start() {
        //leftclick, rightclick, and e all work
        OnInteract += Activate;
    }

    private void Activate()
    {
        //see if we have the item
        if (playerInventory.GetItemByName(itemRequired.itemName) != null)
        {
            //if we do have the item, see if we need to use it up
            if (ConsumeItem) playerInventory.RemoveItem(itemRequired);

            //turn on game objects linked to this
            foreach (GameObject obj in EnableOnUse)
            {
                obj.SetActive(true);
            }
            //turn off game objects linked to this.
            foreach (GameObject obj in DisableOnUse)
            {
                obj.SetActive(false);
            }
        }
        //if this is a one time use kind of thing, just add itself to the disable list
    }
}
