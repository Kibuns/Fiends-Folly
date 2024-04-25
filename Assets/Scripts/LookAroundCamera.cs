using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAroundCamera : MonoBehaviour
{
    [SerializeField] private Transform[] cameraPositions;

    [Header("Camera Positions")]
    [SerializeField] private Transform overTablePosition;
    [SerializeField] private Transform defaultPosition;
    [SerializeField] private Transform ritualPosition;

    public Vector3 mousInput;
    public Vector2 mousePercentagePosition;
    public float LookIntensityX;
    public float LookIntensityY;

    [SerializeField] private float bobbingSpeed = 1f;
    [SerializeField] private float bobbingAmount = 0.1f;

    private float bobbingOffset = 0f;

    public bool ignoreInput;
    public bool bentOverTable;
    public bool turnedAround;

    public float rotationLerpSpeed;
    public Vector3 backRotationOffset;
    private PlayerInputActions playerInputActions;
    private Transform currentTargetPosition;
    private int currentTargetIndex;
    // Start is called before the first frame update

    private void OnEnable()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }
    void Start()
    {
        mousePercentagePosition = Vector2.zero;
        currentTargetPosition = defaultPosition;
        GameManager.Instance.isTurnedAround = turnedAround;
    }

    // Update is called once per frame
    void Update()
    {
        mousInput = Input.mousePosition;
        mousePercentagePosition = new Vector2(Mathf.Clamp((mousInput.x / Screen.width) - 0.5f, -0.5f, 0.5f), Mathf.Clamp((mousInput.y / Screen.height) - 0.5f, -0.5f, 0.5f));


        Quaternion composedTargetRotation = currentTargetPosition.localRotation * Quaternion.Euler(MouseRotationOffset);

        bobbingOffset += Time.deltaTime * bobbingSpeed;
        float yOffset = Mathf.Sin(bobbingOffset) * bobbingAmount;
        float xOffset = Mathf.Cos(bobbingOffset + 3.1415f) * bobbingAmount * 1.3f;
        Vector3 bobbingOffsetVector = new Vector3(xOffset, yOffset, 0f);
        composedTargetRotation *= Quaternion.Euler(bobbingOffsetVector);

        transform.localRotation = Quaternion.Lerp(transform.localRotation, composedTargetRotation, rotationLerpSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, currentTargetPosition.position, rotationLerpSpeed * Time.deltaTime);
        if (ignoreInput) return;
        if (playerInputActions.Player.Jump.triggered && GameManager.Instance.isInGunSequence)
        {
            GameManager.Instance.PlayErrorSound();
        }

        if (playerInputActions.Player.Jump.triggered && !GameManager.Instance.isInGunSequence)
        {
            turnedAround = !turnedAround;
            GameManager.Instance.isTurnedAround = turnedAround;
            if (turnedAround) { currentTargetPosition = ritualPosition; }
            else { currentTargetPosition = defaultPosition;}
        }

        if(turnedAround) { return; }

        if (playerInputActions.Player.MoveForward.triggered && !GameManager.Instance.isInGunSequence)
        {
            currentTargetPosition = overTablePosition;
        }

        if (playerInputActions.Player.MoveBack.triggered && !GameManager.Instance.isInGunSequence)
        {
            currentTargetPosition = defaultPosition;
        }
    }

    public void MoveToDefaultPosition()
    {
        currentTargetPosition = defaultPosition;
        GameManager.Instance.isTurnedAround = false;
        turnedAround = false;
    }

    private Vector3 MouseRotationOffset => new Vector3(-mousePercentagePosition.y * LookIntensityY, mousePercentagePosition.x * LookIntensityX, 0f);

    private void NextCameraPosition()
    {
        currentTargetIndex++;
        if(currentTargetIndex > cameraPositions.Length - 1)
        {
            currentTargetIndex = 0;
        }
        currentTargetPosition = cameraPositions[currentTargetIndex];
    }

}
