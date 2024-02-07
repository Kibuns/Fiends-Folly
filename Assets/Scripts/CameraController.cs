using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class CameraController : MonoBehaviour
{
    public PlayerInputActions input; // Reference to your PlayerInputActions asset
    public float horizontalInput;
    public float verticalInput;
    public float speed;
    public bool enableCameraSnap;
    public bool togglePerspective;
    public Transform perspectivePoint;
    public Vector3 orthographicStartingPosition;
    public Transform perspectiveStartingPoint;
    public float preemptiveSwitchTime;

    private Vector3 perspectiveStartingPosition;
    public float fieldOfView;

    public AnimationCurve FOVChangeCurve;
    private float FOVChangeDelta;

    public AnimationCurve backwardsFOVChangeCurve;

    public AnimationCurve ProjectionChangeCurve;
    private float ProjectionChangeDelta;
    public float projectionChangeSpeed;
    public float speedMultiplier;

    private Camera cam;

    private float timer;

    private float startingFOV;
    public bool isOrthographic;

    void Awake()
    {
        input = new PlayerInputActions();
    }

    private void Start()
    {
        transform.LookAt(perspectivePoint);
        cam = Camera.main;
        orthographicStartingPosition = transform.position; //assuming you start as orthographic
        perspectiveStartingPosition = perspectiveStartingPoint.position;
        startingFOV = cam.fieldOfView;
        isOrthographic = true;
    }
    void Update()
    {
        if (enableCameraSnap) { SnapCameraToPixelPoints(); }
        MoveInput();

        if(input.Player.TogglePerspective.triggered) { togglePerspective = true; }

        if (togglePerspective)
        {
            togglePerspective = false;
            if (isOrthographic)
            {
                timer = 0f;
                cam.orthographic = false;
                isOrthographic = false;
                perspectiveStartingPosition = perspectiveStartingPoint.position;
                Debug.Log(perspectiveStartingPosition);
                transform.position = perspectiveStartingPosition;
                Debug.Log(transform.position);
            }
            else if (!isOrthographic)
            {
                timer = 1.2f;
                isOrthographic = true;
            }
        }

        if(cam.orthographic) { return; }




        if (isOrthographic)
        {
            timer -= Time.deltaTime * speedMultiplier;
            LerpToOrthographic();
            transform.LookAt(perspectivePoint);
        }

        if (!isOrthographic)
        {
            timer += Time.deltaTime * speedMultiplier;
            LerpToPerspective();
        }

        if(timer < preemptiveSwitchTime && isOrthographic)
        {
            cam.orthographic = true;
            transform.position = orthographicStartingPosition;
        }


        
    }

    private void LerpToPerspective()
    {
        ProjectionChangeDelta = ProjectionChangeCurve.Evaluate(timer);
        transform.position = Vector3.Lerp(transform.position, perspectivePoint.position, projectionChangeSpeed * ProjectionChangeDelta * Time.deltaTime * speedMultiplier);
        FOVChangeDelta = FOVChangeCurve.Evaluate(timer);
        cam.fieldOfView = FOVChangeDelta * fieldOfView;
        if (FOVChangeCurve.Evaluate(timer) * fieldOfView < startingFOV)
        {
            cam.fieldOfView = startingFOV;
        }
    }

    private void LerpToOrthographic()
    {
        ProjectionChangeDelta = ProjectionChangeCurve.Evaluate(timer - preemptiveSwitchTime);
        transform.position = Vector3.Lerp(transform.position, perspectiveStartingPosition, projectionChangeSpeed * ProjectionChangeDelta * Time.deltaTime * speedMultiplier);
        FOVChangeDelta = backwardsFOVChangeCurve.Evaluate(timer - preemptiveSwitchTime); // Use timer here, not -timer
        cam.fieldOfView = FOVChangeDelta * fieldOfView;
        if (FOVChangeCurve.Evaluate(timer - preemptiveSwitchTime) * fieldOfView < startingFOV)
        {
            cam.fieldOfView = startingFOV;
        }
    }

    private void MoveInput()
    {
        if (!isOrthographic) return;
        horizontalInput = input.Player.Move.ReadValue<Vector2>().x;
        verticalInput = input.Player.Move.ReadValue<Vector2>().y;

        transform.position = new Vector3(transform.position.x + horizontalInput * speed * Time.deltaTime, transform.position.y + verticalInput * speed * Time.deltaTime, transform.position.z - horizontalInput * speed * Time.deltaTime);
    }

    private void SnapCameraToPixelPoints()
    {
        // Calculate the position of the canvas camera in world space
        Vector3 cameraPosition = transform.position;

        // Calculate the position of the canvas camera in screen space
        Vector3 screenPosition = cam.WorldToScreenPoint(cameraPosition);


        // Round the screen space position to the nearest pixel
        Vector3 roundedScreenPosition = new Vector3(
            Mathf.RoundToInt(screenPosition.x),
            Mathf.RoundToInt(screenPosition.y),
            Mathf.RoundToInt(screenPosition.z)
        );

        // Convert the rounded screen space position back to world space
        Vector3 roundedCameraPosition = cam.ScreenToWorldPoint(roundedScreenPosition);

        // Snap the canvas camera to the rounded position
        transform.position = roundedCameraPosition;
    }

    public bool IsInFPSState()
    {
        return FOVChangeDelta == 1f;
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
}
