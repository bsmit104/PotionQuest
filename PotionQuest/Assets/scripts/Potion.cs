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
        potionRenderer = potionInside.GetComponent<Renderer>();
    }

    public void SetAmount(float percent)
    {
        _amount = percent;
        potionRenderer.material.SetFloat("_FillPercent", percent);
    }

    public void Add(float amount)
    {
        _amount += amount;
        if (_amount > 1) _amount = 1;
        if (_amount < 0) _amount = 0;
        potionRenderer.material.SetFloat("_FillPercent", _amount);
    }

    public void Remove(float amount)
    {
        _amount -= amount;
        if (_amount > 1) _amount = 1;
        if (_amount < 0) _amount = 0;
        potionRenderer.material.SetFloat("_FillPercent", _amount);
    }



    // Update is called once per frame
    void Update()
    {
        //Amount = 0.1f;
    }
}
