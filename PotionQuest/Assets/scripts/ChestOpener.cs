using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestOpener : MonoBehaviour
{
    public Chest chest;
    public ChestUI chestUI;
    public PlayerController playerController;
    private bool isPlayerInRange = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPlayerInRange)
        {
            if (chestUI.gameObject.activeSelf)
            {
                CloseChest();
            }
            else
            {
                OpenChest();
            }
        }
    }

    void OpenChest()
    {
        chestUI.gameObject.SetActive(true);
        playerController.UnlockCursor();
    }

    void CloseChest()
    {
        chestUI.gameObject.SetActive(false);
        playerController.LockCursor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (chestUI.gameObject.activeSelf)
            {
                CloseChest();
            }
        }
    }
}

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ChestOpener : MonoBehaviour
// {
//     public Chest chest;
//     public ChestUI chestUI;
//     public PlayerController playerController;
//     private bool isPlayerInRange = false;

//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.E) && isPlayerInRange)
//         {
//             if (chestUI.gameObject.activeSelf)
//             {
//                 CloseChest();
//             }
//             else
//             {
//                 OpenChest();
//             }
//         }
//     }

//     void OpenChest()
//     {
//         chestUI.gameObject.SetActive(true);
//         playerController.UnlockCursor();
//     }

//     void CloseChest()
//     {
//         chestUI.gameObject.SetActive(false);
//         playerController.LockCursor();
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             isPlayerInRange = true;
//         }
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             isPlayerInRange = false;
//         }
//     }
// }

// public class ChestOpener : MonoBehaviour
// {
//     public Chest chest;
//     public ChestUI chestUI;
//     public PlayerController playerController;

//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.E))
//         {
//             if (chestUI.gameObject.activeSelf)
//             {
//                 CloseChest();
//             }
//             else if (Vector3.Distance(transform.position, chest.transform.position) < .1f)
//             {
//                 OpenChest();
//             }
//         }
//     }

//     void OpenChest()
//     {
//         chestUI.gameObject.SetActive(true);
//         playerController.UnlockCursor();
//     }

//     void CloseChest()
//     {
//         chestUI.gameObject.SetActive(false);
//         playerController.LockCursor();
//     }
// }