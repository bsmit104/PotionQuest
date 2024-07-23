using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    [SerializeField]
    protected GameObject potionInside;
    protected Renderer potionRenderer;

    protected float _amount = 1;
    public float Amount {
        get { return _amount;  }
        set { SetAmount(value);}
    }

    void Start()
    {
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        materialPropertyBlock.SetFloat("_FillPercent", _amount);
        potionRenderer = potionInside.GetComponent<Renderer>();
        potionRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    public void SetAmount(float percent)
    {
        _amount = percent;

        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        materialPropertyBlock.SetFloat("_FillPercent", _amount);
        potionRenderer.SetPropertyBlock(materialPropertyBlock);
    
        //potionRenderer.material.SetFloat("_FillPercent", percent);
    }

    public void Add(float amount)
    {
        _amount += amount;
        if (_amount > 1) _amount = 1;
        if (_amount < 0) _amount = 0;
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        materialPropertyBlock.SetFloat("_FillPercent", _amount);
        potionRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    public void Remove(float amount)
    {
        _amount -= amount;
        if (_amount > 1) _amount = 1;
        if (_amount < 0) _amount = 0;
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        materialPropertyBlock.SetFloat("_FillPercent", _amount);
        potionRenderer.SetPropertyBlock(materialPropertyBlock);
    }



    // Update is called once per frame
    void Update()
    {
        Amount = 0.1f;
    }
}
