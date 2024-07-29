using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastTravelDestination : MonoBehaviour
{
    public string Name;
    public Vector3 TeleportToPosition;
    public bool Discovered = false;

    private FastTravelManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("FastTravelManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
