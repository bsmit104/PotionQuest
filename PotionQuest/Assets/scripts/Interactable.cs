using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Action OnInteract;
    public Action OnLeftClick;
    public Action OnRightClick;
    public Action OnPress;

    virtual public string GetHoveredText()
    {
        return "";
    }

    public void LeftClick()
    {
        OnInteract.Invoke();
        OnLeftClick.Invoke();
    }
    public void RightClick()
    {
        OnInteract.Invoke();
        OnRightClick.Invoke();
    }
    public void Press()
    {
        OnInteract.Invoke();
        OnPress.Invoke();
    }
}
