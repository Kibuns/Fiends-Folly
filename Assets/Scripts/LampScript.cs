using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampScript : MonoBehaviour
{
    [SerializeField] private Light lampLight;
    [SerializeField] private GameObject lightRay;
    private bool isOn;

    private void Start()
    {
        if (lampLight.enabled)
        {
            isOn = true;
        }
    }
    private void OnMouseEnter()
    {
        CursorManager.instance.EnablePointCursor();
    }

    private void OnMouseExit()
    {
        CursorManager.instance.EnableDefaultCursor();
    }

    private void OnMouseDown()
    {
        ToggleLight();
    }

    private void ToggleLight()
    {
        isOn = !isOn;
        lampLight.enabled = isOn;
        lightRay.SetActive(isOn);
    }
}
