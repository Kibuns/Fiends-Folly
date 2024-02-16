using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HoveringObject : MonoBehaviour
{
    public GameObject hoveringObject;
    public Vector3 hoverPositionDelta;
    public float maxHoverHeightDelta;
    public float maxHoverRotationDelta;
    public float bobbingSpeedMultiplier;
    public Vector3 hoverRotation;
    public float lerpSpeed;
    public bool pickupOnInteract; //doesnt override other IInteractable behaviours
    public bool playSoundOnHover;
    public bool startDialogueOnInteract; //doesnt override other IInteractable behaviours
    [SerializeField] private Dialogue dialogue;
    public float dialogueStartDelay;

    private int selectedLayer = 6;
    private int highlightLayer = 3;
    private Vector3 localStartPosition;
    private Vector3 localStartRotation;
    private Vector3 globalStartPosition;
    private Vector3 globalStartRotation;
    private bool isHoveredOn;
    private bool hoverable;
    
    private Transform holdItemTransform;
    private PlayerInputActions playerInput;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = new PlayerInputActions();
        playerInput.Enable();
        localStartPosition = hoveringObject.transform.localPosition;
        localStartRotation = hoveringObject.transform.localEulerAngles;
        globalStartPosition = transform.position;
        globalStartRotation = transform.eulerAngles;
        holdItemTransform = ItemManager.Instance.holdItemPosition;
        hoverable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hoverable)
        {
            return;
        }
        if (!isHoveredOn)
        {
            LerpToGlobalStart();
            LerpToLocalStart();
            return;
        }

        Hover();
    }
    private void LerpToHoldPosition()
    {
        transform.position = Vector3.Lerp(transform.position, holdItemTransform.position, Time.deltaTime * lerpSpeed);
        transform.localEulerAngles = AngleLerp(transform.localEulerAngles, holdItemTransform.eulerAngles, Time.deltaTime * lerpSpeed);
    }

    private void InitHover()
    {
        Debug.Log("InitHover");
        CursorManager.instance.EnablePointCursor();
        if (playSoundOnHover)
        {
            hoveringObject.GetComponent<Item>().PlayPickupSound();
        }
    }

    private void OnMouseEnter()
    {
        isHoveredOn = true;
        InitHover();
    }

    private void OnMouseDown()
    {
        
        if (startDialogueOnInteract)
        {
            StartDialogue();
        }
        if (TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact();
        }
        else
        {
            Debug.LogWarning("No IInteractable component found on gameobject");
        }
        if (pickupOnInteract)
        {
            DisableHoverable();
            hoveringObject.GetComponent<Item>().PickUp();
        }
    }

    private void StartDialogue()
    {
        StartCoroutine(DialogueDelay(dialogueStartDelay));
    }

    private IEnumerator DialogueDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ShowDialogue();
    }

    private void ShowDialogue()
    {
        DialogueManager.instance.StartDialogue(dialogue);
    }

    public void DisableHoverable()
    {
        hoverable = false;
        GetComponent<Collider>().enabled = false;
        SetSelected(true);
    }

    public void EnableHoverable()
    {
        hoverable = true;
        GetComponent<Collider>().enabled = true;
        SetSelected(false);
    }

    private void OnMouseExit()
    {
        isHoveredOn = false;
        CursorManager.instance.SwitchToDefaultCursor();
        if (gameObject.layer != highlightLayer && hoverable)
        {
            SetSelected(false);
        }
    }

    private void Hover()
    {
        if(gameObject.layer != selectedLayer)
        {
            SetSelected(true);
        }
        float targetHoverHeight = Mathf.Sin(Time.time * bobbingSpeedMultiplier) * maxHoverHeightDelta;
        Vector3 targetPos = new Vector3(localStartPosition.x +hoverPositionDelta.x, localStartPosition.y + hoverPositionDelta.y + targetHoverHeight, localStartPosition.z + hoverPositionDelta.z);
        hoveringObject.transform.localPosition = Vector3.Lerp(hoveringObject.transform.localPosition, targetPos, Time.deltaTime * lerpSpeed);


        float rotationBobbingMultiplier = 2f;
        float targetHoverRotationY = (hoverRotation.y + Mathf.Sin(Time.time * bobbingSpeedMultiplier * rotationBobbingMultiplier) * maxHoverRotationDelta);
        Vector3 targetRotation = new Vector3(hoverRotation.x, targetHoverRotationY, hoverRotation.z);
        hoveringObject.transform.localEulerAngles = AngleLerp(hoveringObject.transform.localEulerAngles, targetRotation, Time.deltaTime * lerpSpeed);
    }

    private void LerpToLocalStart()
    {
        hoveringObject.transform.localPosition = Vector3.Lerp(hoveringObject.transform.localPosition, localStartPosition, Time.deltaTime * lerpSpeed);
        hoveringObject.transform.localEulerAngles = AngleLerp(hoveringObject.transform.localEulerAngles, localStartRotation, Time.deltaTime * lerpSpeed);
    }

    private void LerpToGlobalStart()
    {
        transform.position = Vector3.Lerp(transform.position, globalStartPosition, Time.deltaTime * lerpSpeed);
        transform.eulerAngles = AngleLerp(transform.eulerAngles, globalStartRotation, Time.deltaTime * lerpSpeed);
    }

    //Replace Vector3.Lerp because that one cant cross an angle of 0, so for instance lerp from 10 to -10 is only possible with this method without weird behaviour
    private Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t) 
    {
        float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
        float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
        float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
        Vector3 Lerped = new Vector3(xLerp, yLerp, zLerp);
        return Lerped;
    }

    private void SetSelected(bool selected)
    {
        int targetLayer = selected ? selectedLayer : highlightLayer;
        SetLayerRecursively(transform, targetLayer);
    }

    private void SetLayerRecursively(Transform root, int layer)
    {
        if (!root.CompareTag("Bloodstain"))
        {
            root.gameObject.layer = layer;
        }
        
        // Iterate through all children recursively
        foreach (Transform child in root)
        {
            SetLayerRecursively(child, layer);
        }
    }
}
