using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryOpener : MonoBehaviour
{
    public Inventory inv;
    public InventoryUI invUI;
    public PlayerController playerController;
    private bool isPlayerInRange = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPlayerInRange)
        {
            if (invUI.gameObject.activeSelf)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }
    }

    void OpenInventory()
    {
        invUI.gameObject.SetActive(true);
        playerController.UnlockCursor();
    }

    void CloseInventory()
    {
        invUI.gameObject.SetActive(false);
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
            if (invUI.gameObject.activeSelf)
            {
                CloseInventory();
            }
        }
    }
}
