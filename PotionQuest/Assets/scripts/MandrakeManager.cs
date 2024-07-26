using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MandrakeManager : MonoBehaviour
{
    public float raiseSpeed = 0.5f;
    public float lowerSpeed = 0.2f;
    public float maxHeight = 5f;

    public Item mandrakeItem;

    private List<MandrakeData> mandrakes = new List<MandrakeData>();
    private bool playerInArea = false;
    private PlayerController playerController;

    void Start()
    {
        // Find all mandrakes in the scene
        GameObject[] mandrakeObjects = GameObject.FindGameObjectsWithTag("Interactable");
        
        foreach (GameObject mandrake in mandrakeObjects)
        {
            if (mandrake.TryGetComponent<ItemPickup>(out ItemPickup pickup))
            {
                if (pickup.item == mandrakeItem)
                    mandrakes.Add(new MandrakeData(mandrake.transform, mandrake.transform.position));
            }
        }
    }

    void Update()
    {
        foreach (MandrakeData mandrakeData in mandrakes)
        {
            if (mandrakeData.transform == null)
            {
                continue;
            }

            Vector3 targetPosition;
            if (playerInArea && playerController != null)
            {
                if (IsPlayerJumping())
                {
                    targetPosition = mandrakeData.originalPosition + Vector3.up * maxHeight;
                }
                else
                {
                    targetPosition = mandrakeData.originalPosition;
                }
            }
            else
            {
                targetPosition = mandrakeData.originalPosition;
            }

            // Smoothly move the mandrake to the target position
            mandrakeData.transform.position = Vector3.Lerp(mandrakeData.transform.position, targetPosition, Time.deltaTime * (IsPlayerJumping() ? raiseSpeed : lowerSpeed));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInArea = true;
            playerController = other.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInArea = false;
            playerController = null;
        }
    }

    private bool IsPlayerJumping()
    {
        // Check if the player is jumping based on the PlayerController's grounded state
        return playerController != null && !playerController.GetComponent<CharacterController>().isGrounded;
    }

    private class MandrakeData
    {
        public Transform transform;
        public Vector3 originalPosition;

        public MandrakeData(Transform transform, Vector3 originalPosition)
        {
            this.transform = transform;
            this.originalPosition = originalPosition;
        }
    }
}

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class MandrakeManager : MonoBehaviour
// {
//     public float raiseSpeed = 0.5f;
//     public float lowerSpeed = 0.2f;
//     public float maxHeight = 5f;
//     public float minHeight = 0f;

//     private List<Transform> mandrakes = new List<Transform>();
//     private bool playerInArea = false;
//     private PlayerController playerController;

//     void Start()
//     {
//         // Find all mandrakes in the scene
//         GameObject[] mandrakeObjects = GameObject.FindGameObjectsWithTag("Mandrake");
//         foreach (GameObject mandrake in mandrakeObjects)
//         {
//             mandrakes.Add(mandrake.transform);
//         }
//     }

//     void Update()
//     {
//         // Iterate through a copy of the list to avoid modification issues
//         List<Transform> currentMandrakes = new List<Transform>(mandrakes);
//         foreach (Transform mandrake in currentMandrakes)
//         {
//             if (mandrake == null)
//             {
//                 mandrakes.Remove(mandrake);
//                 continue;
//             }

//             if (playerInArea && playerController != null)
//             {
//                 if (IsPlayerJumping())
//                 {
//                     RaiseMandrake(mandrake);
//                 }
//                 else
//                 {
//                     LowerMandrake(mandrake);
//                 }
//             }
//             else
//             {
//                 LowerMandrake(mandrake);
//             }
//         }
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             playerInArea = true;
//             playerController = other.GetComponent<PlayerController>();
//         }
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             playerInArea = false;
//             playerController = null;
//         }
//     }

//     private bool IsPlayerJumping()
//     {
//         // Check if the player is jumping based on the PlayerController's grounded state
//         return playerController != null && !playerController.GetComponent<CharacterController>().isGrounded;
//     }

//     void RaiseMandrake(Transform mandrake)
//     {
//         if (mandrake != null)
//         {
//             Vector3 targetPosition = mandrake.position;
//             if (targetPosition.y < mandrake.position.y + maxHeight)
//             {
//                 targetPosition += Vector3.up * raiseSpeed * Time.deltaTime;
//                 mandrake.position = targetPosition;
//             }
//         }
//     }

//     void LowerMandrake(Transform mandrake)
//     {
//         if (mandrake != null)
//         {
//             Vector3 targetPosition = mandrake.position;
//             if (targetPosition.y > mandrake.position.y + minHeight)
//             {
//                 targetPosition -= Vector3.up * lowerSpeed * Time.deltaTime;
//                 mandrake.position = targetPosition;
//             }
//         }
//     }
// }


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class MandrakeManager : MonoBehaviour
// {
//     public float raiseSpeed = 0.5f;
//     public float lowerSpeed = 0.2f;
//     public float maxHeight = 5f;
//     public float minHeight = 0f;

//     private List<Transform> mandrakes = new List<Transform>();
//     private bool playerInArea = false;
//     private PlayerController playerController;

//     void Start()
//     {
//         GameObject[] mandrakeObjects = GameObject.FindGameObjectsWithTag("Mandrake");
//         foreach (GameObject mandrake in mandrakeObjects)
//         {
//             mandrakes.Add(mandrake.transform);
//         }
//     }

//     void Update()
//     {
//         if (playerInArea && playerController != null)
//         {
//             foreach (Transform mandrake in mandrakes)
//             {
//                 if (IsPlayerJumping())
//                 {
//                     RaiseMandrake(mandrake);
//                 }
//                 else
//                 {
//                     LowerMandrake(mandrake);
//                 }

//                 mandrake.position = Vector3.Lerp(mandrake.position, mandrake.position, Time.deltaTime);
//             }
//         }
//         else
//         {
//             foreach (Transform mandrake in mandrakes)
//             {
//                 LowerMandrake(mandrake);
//                 mandrake.position = Vector3.Lerp(mandrake.position, mandrake.position, Time.deltaTime);
//             }
//         }
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             playerInArea = true;
//             playerController = other.GetComponent<PlayerController>();
//         }
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             playerInArea = false;
//             playerController = null;
//         }
//     }

//     private bool IsPlayerJumping()
//     {
//         // Check if the player is jumping based on the PlayerController's grounded state
//         return playerController != null && !playerController.GetComponent<CharacterController>().isGrounded;
//     }

//     void RaiseMandrake(Transform mandrake)
//     {
//         Vector3 targetPosition = mandrake.position;
//         if (targetPosition.y < mandrake.position.y + maxHeight)
//         {
//             targetPosition += Vector3.up * raiseSpeed * Time.deltaTime;
//             mandrake.position = targetPosition;
//         }
//     }

//     void LowerMandrake(Transform mandrake)
//     {
//         Vector3 targetPosition = mandrake.position;
//         if (targetPosition.y > mandrake.position.y + minHeight)
//         {
//             targetPosition -= Vector3.up * lowerSpeed * Time.deltaTime;
//             mandrake.position = targetPosition;
//         }
//     }
// }
