using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StarlightPotion : Potion
{
    public Light potionLight;
    private float MaxIntensity = 1.0f;
    private float MinIntensity = 0.1f;

    private void FixedUpdate() {
        //we need to keep the light intensity based on the fluid amount.
        potionLight.intensity = Mathf.Lerp(MinIntensity, MaxIntensity, _amount);
    }

    void Start()
    {
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        materialPropertyBlock.SetFloat("_FillPercent", _amount);
        potionRenderer = potionInside.GetComponent<Renderer>();
        potionRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    private void Update() {
        if (onGround)
        {
            Amount -= (1.0f / 400.0f) * Time.deltaTime;
            if (Amount  <= 0)
            {
                potionLight.range = 0;
                potionLight.intensity = 0;
            }
        }
    }
}
