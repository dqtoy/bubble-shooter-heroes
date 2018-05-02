using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class CustomToggle : MonoBehaviour
{

    [System.Serializable]
    public class myToggleEvent : UnityEvent<bool> { }

    [SerializeField]
    public myToggleEvent toggleDelegate;


    public bool toggleState;

    public GameObject OnGroup, OffGroup;

    public void Toggle()
    {
        toggleState = !toggleState;
        if (toggleState == true)
        {
            OnGroup.SetActive(true);
            OffGroup.SetActive(false);
        }
        else
        {
            OnGroup.SetActive(false);
            OffGroup.SetActive(true);
        }
        toggleDelegate.Invoke(toggleState);
    }

    public void SetToggle(bool enable)
    {
        toggleState = enable;
        if (toggleState == true)
        {
            OnGroup.SetActive(true);
            OffGroup.SetActive(false);
        }
        else
        {
            OnGroup.SetActive(false);
            OffGroup.SetActive(true);
        }
        toggleDelegate.Invoke(toggleState);
    }
}
