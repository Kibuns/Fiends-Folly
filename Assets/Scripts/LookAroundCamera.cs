using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAroundCamera : MonoBehaviour
{
    public Vector3 mousInput;
    public Vector2 mousePercentagePosition;
    public float LookIntensityX;
    public float LookIntensityY;

    private Vector3 startRotation;
    // Start is called before the first frame update
    void Start()
    {
        mousePercentagePosition = Vector2.zero;
        startRotation = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        mousInput = Input.mousePosition;
        mousePercentagePosition = new Vector2(Mathf.Clamp((mousInput.x / Screen.width) - 0.5f, -0.5f, 0.5f), Mathf.Clamp((mousInput.y / Screen.height) - 0.5f, -0.5f, 0.5f));

        transform.localEulerAngles = startRotation + new Vector3(-mousePercentagePosition.y * LookIntensityY, mousePercentagePosition.x * LookIntensityX, 0f);
    }
}
