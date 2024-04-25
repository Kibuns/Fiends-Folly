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
