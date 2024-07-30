using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlotDisplay : MonoBehaviour
{

    GameObject displayObject;
    public 
    GameObject DebugObject;
    
    public int slotIndex = -1;
    public Inventory inventory;

    public bool BlockPlacement = false;

    public void Initialize(int index, Inventory inv)
    {
        inventory = inv;
        slotIndex = index;
    }

    public void DisplayItem(Item item)
    {
        //remove previous object
        RemoveItem();

        if (item == null) return;

        //instantiate the item's prefab and disable pickup
        if (item.itemObject != null)
        {
            displayObject = Instantiate(item.itemObject, this.transform.position + new Vector3(0,0.03f, 0), this.transform.rotation);
            displayObject.transform.rotation = Quaternion.Euler(item.offsetRotation);
            displayObject.transform.localScale *= item.offsetScale;
            if (displayObject.TryGetComponent<Potion>(out Potion potion))
            {
                displayObject.transform.position += new Vector3(0,0.16f, 0);
            }
            if (displayObject.TryGetComponent<Collider>(out Collider collider))
            {
                collider.enabled = false;
            }
            // if (displayObject.TryGetComponent<ItemPickup>(out ItemPickup comp))
            // {
            //     comp.CanBePickedUp = false;
            // }
        }
        else
        {
            displayObject = Instantiate(DebugObject, this.transform.position + new Vector3(0,0.1f,0), this.transform.rotation);
        }
    }

    public void RemoveItem()
    {
        //remove previous object
        if (displayObject != null)
        {
            Destroy(displayObject);
        }
    }
}
