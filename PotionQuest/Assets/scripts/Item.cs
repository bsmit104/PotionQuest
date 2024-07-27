using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This attribute allows us to create new items from the Unity editor
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName; // Name of the item
    //public string displayName = "item";
    public Sprite itemIcon; // Icon representing the item in the UI
    //public GameObject itemPrefab;
    public int maxStack; // Maximum stack size for this item

    // The weight of the object, should be same as rigidbody mass if it has one
    public float itemWeight = 1;
    
    public GameObject itemObject;
    public Vector3 offsetPosition = new Vector3(0,0.1f, 0);
    public Vector3 offsetRotation = new Vector3(-90, 0, 0);
    public float offsetScale = 0.5f;

    // You can add more properties here, like item ID, description, etc.
}