using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LightType
{
    PointLight, DirectionalLight
}

public class SafeLight : MonoBehaviour
{    
    
    new public Light light;
    public LightType type = LightType.PointLight;
    private LightManager manager;

    private void Awake() {
        //get the light manager, and add it to it.
        light = GetComponent<Light>();
        manager = GameObject.FindGameObjectWithTag("LightManager").GetComponent<LightManager>();
        manager.AddLight(this);
    }

    private void OnDestroy() {
        manager.lights.Remove(this);
    }

}
