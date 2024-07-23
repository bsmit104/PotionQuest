using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    public HashSet<SafeLight> lights = new HashSet<SafeLight>();

    public void AddLight(SafeLight light)
    {
        lights.Add(light);
    }
    
    // private void Update() {
    //     foreach (SafeLight light in lights)
    //     {
    //         if (light.type == LightType.PointLight)
    //         {
    //             //point light
    //         }else
    //         {
    //             //directional or other

    //         }
    //     }
    // }
}
