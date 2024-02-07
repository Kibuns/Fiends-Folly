using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PerspectiveCameraMovement : MonoBehaviour
{

    [Range(0f, 6f)]
    public float sens;
    public Transform orientation;
    public PlayerInputActions input; // Reference to your PlayerInputActions asset
    public float lerpSpeed = 10f;


    private CameraController camController;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    private bool outOfState;
    private float timer = 0f;
    // Start is called before the first frame update
    void Awake()
    {
        input = new PlayerInputActions();
    }

    private void Start()
    {
        camController = GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!camController.IsInFPSState()) {
            outOfState = true;
            Cursor.lockState = CursorLockMode.None;
            return; 
        
        }
        Vector2 mouseInput;
        if (outOfState) //when receiving control, initate following:
        {
            outOfState = false;
            xRotation = camController.transform.localEulerAngles.x;
            timer = 0f;
            //StartCoroutine(LerpRotation());
            yRotation = camController.transform.localEulerAngles.y;
        }
        Cursor.lockState = CursorLockMode.Locked;
        // Get the mouse input from the Input Actions
        mouseInput = input.Player.Look.ReadValue<Vector2>();

        // Calculate the camera rotation
        xRotation -= mouseInput.y * sens;
        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f); // Clamp to prevent over-rotation

        yRotation += mouseInput.x * sens;

        // Apply rotation to the orientation and the camera
        orientation.transform.localRotation = Quaternion.Euler(0.0f, yRotation, 0f);
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    private IEnumerator LerpRotation()
    {
        float targetXRotation = 0f;


        while (Mathf.Abs(xRotation - targetXRotation) > 0.03f)
        {
            timer += Time.deltaTime;
            if(timer > 0.2f)
            {
                Debug.Log("HEYA");
            }
            else
            {
                xRotation = Mathf.Lerp(xRotation, targetXRotation, lerpSpeed * Time.deltaTime);
            }
            yield return null;
        }
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
