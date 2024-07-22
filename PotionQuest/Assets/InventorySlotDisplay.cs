using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlotDisplay : MonoBehaviour
{
    GameObject displayObject;
    public 
    GameObject DebugObject;
    
    private int slotIndex = -1;
    Inventory inv;

    public void Initialize(int index, Inventory inventory)
    {
        inv = inventory;
        slotIndex = index;
    }

    public void DisplayItem(Item item)
    {
        //remove previous object
        RemoveItem();

        //instantiate the item's prefab and disable pickup
        if (item.itemObject != null)
        {
            displayObject = Instantiate(item.itemObject, this.transform.position + item.offsetPosition, this.transform.rotation);
            displayObject.transform.rotation = Quaternion.Euler(item.offsetRotation);
            displayObject.transform.localScale *= item.offsetScale;

            if (displayObject.TryGetComponent<BoxCollider>(out BoxCollider collider))
            {
                collider.enabled = false;
            }
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
