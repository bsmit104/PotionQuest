using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastTravelDestination : MonoBehaviour
{
    public string DestinationName;
    public Vector3 TeleportToPosition;
    public bool Discovered = false;

    private FastTravelManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("FastTravelManager").GetComponent<FastTravelManager>();
    }

    
    private void OnTriggerEnter(Collider other) {
        //set that we have arrived at this destination
        Discovered = true;
    }
}
