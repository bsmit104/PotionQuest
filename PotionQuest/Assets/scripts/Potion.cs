using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Potion : MonoBehaviour
{
    [SerializeField]
    protected GameObject potionInside = null;
    protected Renderer potionRenderer;

    [SerializeField]
    protected float _amount = 0.5f;

    public bool onGround = false;

    public float Amount {
        get { return _amount;  }
        set { SetAmount(value);}
    }

    void Start()
    {
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        materialPropertyBlock.SetFloat("_FillPercent", _amount);
        if (potionInside != null)
        {
            potionRenderer = potionInside.GetComponent<Renderer>();
            potionRenderer.SetPropertyBlock(materialPropertyBlock);
        }
    }

    private void Awake() {
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        materialPropertyBlock.SetFloat("_FillPercent", _amount);
        if (potionInside != null)
        {
            potionRenderer = potionInside.GetComponent<Renderer>();
            potionRenderer.SetPropertyBlock(materialPropertyBlock);
        }    
    }

    public virtual void SetAmount(float percent)
    {
        _amount = percent;

        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        materialPropertyBlock.SetFloat("_FillPercent", _amount);
        if (potionInside != null)
            potionRenderer.SetPropertyBlock(materialPropertyBlock);
    
        //potionRenderer.material.SetFloat("_FillPercent", percent);
    }

    public void Add(float amount)
    {
        _amount += amount;
        if (_amount > 1) _amount = 1;
        if (_amount < 0) _amount = 0;
        if (potionInside != null)
        {
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetFloat("_FillPercent", _amount);
            potionRenderer.SetPropertyBlock(materialPropertyBlock);
        }
    }

    public void Remove(float amount)
    {
        _amount -= amount;
        if (_amount > 1) _amount = 1;
        if (_amount < 0) _amount = 0;
        if (potionInside != null)
        {
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetFloat("_FillPercent", _amount);
            potionRenderer.SetPropertyBlock(materialPropertyBlock);
        }
    }
}
