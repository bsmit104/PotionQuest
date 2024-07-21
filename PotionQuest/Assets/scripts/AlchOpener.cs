using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchOpener : MonoBehaviour
{
    public Alch Alch;
    public AlchUI AlchUI;
    // public AlchUI ResultUI;
    public PlayerController playerController;
    private bool isPlayerInRange = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPlayerInRange)
        {
            if (AlchUI.gameObject.activeSelf)
            {
                CloseAlch();
            }
            else
            {
                OpenAlch();
            }
        }
    }

    void OpenAlch()
    {
        AlchUI.gameObject.SetActive(true);
        // ResultUI.gameObject.SetActive(true);
        playerController.UnlockCursor();
    }

    void CloseAlch()
    {
        AlchUI.gameObject.SetActive(false);
        // ResultUI.gameObject.SetActive(false);
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
            if (AlchUI.gameObject.activeSelf)
            {
                CloseAlch();
            }
        }
        // if (other.CompareTag("Player"))
        // {
        //     isPlayerInRange = false;
        //     if (ResultUI.gameObject.activeSelf)
        //     {
        //         CloseAlch();
        //     }
        // }
    }
}