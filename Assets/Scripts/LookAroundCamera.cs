using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAroundCamera : MonoBehaviour
{
    public Vector3 mousInput;
    public Vector2 mousePercentagePosition;
    public float LookIntensityX;
    public float LookIntensityY;
    public bool turnedAround;
    public float rotationLerpSpeed;
    private Vector3 frontRotation;
    private Vector3 backRotation;
    public Vector3 backRotationOffset;
    private PlayerInputActions playerInputActions;
    // Start is called before the first frame update
    void Start()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        mousePercentagePosition = Vector2.zero;
        frontRotation = transform.localEulerAngles;
        backRotation = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + 180, transform.localEulerAngles.z) + backRotationOffset;
    }

    // Update is called once per frame
    void Update()
    {
        mousInput = Input.mousePosition;
        mousePercentagePosition = new Vector2(Mathf.Clamp((mousInput.x / Screen.width) - 0.5f, -0.5f, 0.5f), Mathf.Clamp((mousInput.y / Screen.height) - 0.5f, -0.5f, 0.5f));

        Vector3 targetEulerRotation = turnedAround ? backRotation : frontRotation;
        transform.localEulerAngles = AngleLerp(transform.localEulerAngles, targetEulerRotation + MouseRotationOffset, rotationLerpSpeed * Time.deltaTime);


        if (playerInputActions.Player.Jump.triggered)
        {
            TurnAround();
        }
    }

    private void TurnAround()
    {
        turnedAround = !turnedAround;
    }
    private Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
    {
        float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
        float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
        float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
        Vector3 Lerped = new Vector3(xLerp, yLerp, zLerp);
        return Lerped;
    }

    private Vector3 MouseRotationOffset => new Vector3(-mousePercentagePosition.y * LookIntensityY, mousePercentagePosition.x * LookIntensityX, 0f);

}
