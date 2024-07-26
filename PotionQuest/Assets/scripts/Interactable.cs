using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public event Action OnInteract;
    public event Action OnLeftClick;
    public event Action OnRightClick;
    public event Action OnPress;

    virtual public string GetHoveredText()
    {
        return "";
    }

    public void LeftClick()
    {
        OnInteract?.Invoke();
        OnLeftClick?.Invoke();
    }
    public void RightClick()
    {
        OnInteract?.Invoke();
        OnRightClick?.Invoke();
    }
    public void Press()
    {
        OnInteract?.Invoke();
        OnPress?.Invoke();
    }
}
