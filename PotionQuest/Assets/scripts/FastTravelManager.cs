using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// FastTravelManager will hold all fast travel locations and 
/// </summary>
public class FastTravelManager : MonoBehaviour
{
    
    private GameObject[] destinations;
    void Start()
    {
        destinations = GameObject.FindGameObjectsWithTag("FastTravelDestination");
    }

    public List<FastTravelDestination> GetDestinations()
    {
        List<FastTravelDestination> result = new List<FastTravelDestination>();
        
        foreach (GameObject obj in destinations)
        {
            if (obj.GetComponent<FastTravelDestination>().Discovered)
                result.Add(obj.GetComponent<FastTravelDestination>());
        }
        return result;
    }

}
