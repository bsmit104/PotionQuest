using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{

    public GameObject closed;
    public GameObject open;

    private bool pressed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!pressed)
        if (other.CompareTag("Player"))
        {
            closed.SetActive(false);
            open.SetActive(true);
            GetComponent<AudioSource>().Play();
            pressed = true;
        }
    }
}
