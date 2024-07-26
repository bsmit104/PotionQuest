using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TriggerHandler : MonoBehaviour
{
    public GameObject closed;
    public GameObject open;
    public TMP_Text feedbackText;
    public string feedbackMessage = "";
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
            else
            {
                feedbackText.text = feedbackMessage; // Update the feedback text
                feedbackText.gameObject.SetActive(true); // Show the feedback text
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            feedbackText.gameObject.SetActive(false); // Hide the feedback text when player leaves
        }
    }
}

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class TriggerHandler : MonoBehaviour
// {
//     public GameObject closed;
//     public GameObject open;
//     public string itemName = "Strength Potion";
//     public Inventory inventory; // Public Inventory field

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             if (inventory != null && inventory.GetItemByName(itemName) != null)
//             {
//                 closed.SetActive(false);
//                 open.SetActive(true);
//             }
//         }
//     }
// }


/// cld add UI for if i was stronger ///