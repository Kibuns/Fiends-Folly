using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
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
    public bool canBeDragged = true;
    public bool changeLayerOnHover = true;
    public bool pickupOnInteract; //doesnt override other IInteractable behaviours
    public bool playSoundOnHover;
    public bool startDialogueOnInteract; //doesnt override other IInteractable behaviours
    public bool isInFrontOfFurnace;
    public bool showColorsOnHover = false;
    [SerializeField] public Dialogue dialogue;
    public float dialogueStartDelay;
    public float overlapYOffset;


    private int bloodLayer = 7;
    private int selectedLayer = 6;
    private int highlightLayer = 3;
    private Vector3 localRestPosition;
    private Vector3 localRestRotation;
    private Vector3 globalStartPosition;
    private Vector3 globalStartRotation;
    private bool isHoveredOn;
    private bool hoverable;
    private float OnMouseEnterCooldown = 0.2f;
    private float MaxClickTime = 0.11f;
    
    private Transform holdItemTransform;
    private PlayerInputActions playerInput;
    private Item attachedItem;
    private LayerMask dragLayerMask;
    private Vector3 dragTargetPosition;
    private List<Transform> overlappingItemPositions;


    public float yOffset;
    private float dragTimer;
    private float timer;
    private bool isDragging;
    private bool mouseCurrentlyOverHitbox;

    // Start is called before the first frame update
    void Start()
    {
        if (showColorsOnHover) { selectedLayer = bloodLayer; }
        attachedItem = hoveringObject.GetComponent<Item>();
        playerInput = new PlayerInputActions();
        playerInput.Enable();
        localRestPosition = hoveringObject.transform.localPosition;
        localRestRotation = hoveringObject.transform.localEulerAngles;
        globalStartPosition = transform.position;
        globalStartRotation = transform.eulerAngles;
        holdItemTransform = ItemManager.Instance.holdItemPosition;
        dragLayerMask = ItemManager.Instance.dragLayerMask;
        hoverable = true;
        dragTargetPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        PlaceDownInput();
        timer += Time.deltaTime;
        if (!hoverable)
        {
            return;
        }
        LerpToDragTarget();
        if (!isHoveredOn)
        {
            LerpToRestPostionAndRotation();
            return;
        }

        Hover();
        
    }

    private void LerpToDragTarget()
    {
        transform.position = Vector3.Lerp(transform.position, dragTargetPosition, lerpSpeed * Time.deltaTime);
    }

    private void PlaceDownInput()
    {
        if (attachedItem == null) return;
        if (attachedItem.isBeingHeld && playerInput.Player.RightClick.triggered)
        {
            if (GameManager.Instance.isInGunSequence)
            {
                GameManager.Instance.PlayErrorSound();
                return;
            }
            if(attachedItem.transform.parent.TryGetComponent(out RazorScript razorScript) && razorScript.startedCuttingSequence) //if during cutting sequence, cant drop item
            {
                GameManager.Instance.PlayErrorSound();
                return;
            }
            if (canBeDragged)
            {
                MoveToMousePosition();
            }
            ItemManager.Instance.DropHeldItem();
        }
    }

    private void MoveToMousePosition()
    {
        Debug.Log("move to mous pos");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, dragLayerMask))
        {
            Debug.Log("hit");
            dragTargetPosition = hit.point + new Vector3(0, yOffset, 0);
            isInFrontOfFurnace = false;
            CursorManager.instance.EnableDragCursor();
        }
        else if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if(hit.collider.transform.TryGetComponent(out FurnaceScript furnaceScript))
            {
                if (!furnaceScript.doorIsOpen) return;
                dragTargetPosition = furnaceScript.burningObjectSelectionPoint.position;
                isInFrontOfFurnace = true;
                CursorManager.instance.EnableDeadCursor();
            }
        }
    }

    private void LerpToHoldPosition()
    {
        transform.position = Vector3.Lerp(transform.position, holdItemTransform.position, Time.deltaTime * lerpSpeed);
        transform.localEulerAngles = AngleLerp(transform.localEulerAngles, holdItemTransform.eulerAngles, Time.deltaTime * lerpSpeed);
    }

    private void InitHover()
    {
        CursorManager.instance.EnablePointCursor();

        if (timer < OnMouseEnterCooldown) return;
        timer = 0f;
        if (playSoundOnHover && !attachedItem.isBeingHeld)
        {
            hoveringObject.GetComponent<Item>().PlayPickupSound();
        }
    }

    private void OnMouseEnter()
    {
        if (isDragging || GameManager.Instance.isDead) return;
        isHoveredOn = true;
        InitHover();
    }

    private void OnMouseUp()
    {
        if(isInFrontOfFurnace)
        {
            FindObjectOfType<FurnaceScript>().EatDraggedObject(gameObject);
        }
        isDragging = false;
        GameManager.Instance.draggedObject = null;
        if (dragTimer > MaxClickTime || !isHoveredOn)
        {
            //isHoveredOn = false;
            dragTimer = 0f;
            CursorManager.instance.EnablePointCursor();
            if(!mouseCurrentlyOverHitbox)
            {
                MouseExit();
            }
            return;
        }
        dragTimer = 0f;

        if (pickupOnInteract && attachedItem.isBeingHeld) { return; }
        CursorManager.instance.EnablePointCursor();
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
            if (!attachedItem.TryPickUp()) return;
            DisableHoverable();
        }

    }

    private void OnMouseDrag()
    {
        if (!canBeDragged || (pickupOnInteract && attachedItem.isBeingHeld)) return;
        dragTimer += Time.deltaTime;
        if (dragTimer < MaxClickTime) return;
        GameManager.Instance.draggedObject = this;
        isHoveredOn = true;
        isDragging = true;
        MoveToMousePosition();
    }

    private void OnMouseOver()
    {
        mouseCurrentlyOverHitbox = true;
    }

    private void StartDialogue()
    {
        if (dialogue == null) return;
        StartCoroutine(DialogueDelay(dialogueStartDelay));
    }

    private IEnumerator DialogueDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (pickupOnInteract && !attachedItem.isBeingHeld) yield break;
        if(GameManager.Instance.isDead) yield break;
        if (attachedItem.TryGetComponent(out RevolverScript revolverScript))
        {
            if (revolverScript.hasShotBlank) yield break;
        }
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
        mouseCurrentlyOverHitbox = false;
        if (isDragging) return;
        MouseExit();
    }

    private void MouseExit()
    {
        isHoveredOn = false;
        CursorManager.instance.EnableDefaultCursor();
        if (gameObject.layer != highlightLayer && hoverable)
        {
            SetSelected(false);
        }
        if (timer < OnMouseEnterCooldown) return;
        timer = 0f;
        if (playSoundOnHover && !attachedItem.isBeingHeld)
        {
            attachedItem.PlayPutDownSound();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other == null)
        {
            yOffset = 0f;
            return;
        }
        if (isDragging && other.CompareTag("Paper"))
        {
            if (other.transform.position.y >= transform.position.y)
            {
                Debug.Log("added " + overlapYOffset + " to yOffset");
                yOffset = other.GetComponent<HoveringObject>().yOffset + overlapYOffset;
            }
        }
    }

    private void Hover()
    {
        if(gameObject.layer != selectedLayer)
        {
            SetSelected(true);
        }
        float targetHoverHeight = Mathf.Sin(Time.time * bobbingSpeedMultiplier) * maxHoverHeightDelta;
        Vector3 targetPos = new Vector3(localRestPosition.x +hoverPositionDelta.x, localRestPosition.y + hoverPositionDelta.y + targetHoverHeight, localRestPosition.z + hoverPositionDelta.z);
        hoveringObject.transform.localPosition = Vector3.Lerp(hoveringObject.transform.localPosition, targetPos, Time.deltaTime * lerpSpeed);


        float rotationBobbingMultiplier = 2f;
        float targetHoverRotationY = (hoverRotation.y + Mathf.Sin(Time.time * bobbingSpeedMultiplier * rotationBobbingMultiplier) * maxHoverRotationDelta);
        Vector3 targetRotation = new Vector3(hoverRotation.x, targetHoverRotationY, hoverRotation.z);
        hoveringObject.transform.localEulerAngles = AngleLerp(hoveringObject.transform.localEulerAngles, targetRotation, Time.deltaTime * lerpSpeed);
    }

    private void LerpToRestPostionAndRotation()
    {
        hoveringObject.transform.localPosition = Vector3.Lerp(hoveringObject.transform.localPosition, localRestPosition, Time.deltaTime * lerpSpeed);
        hoveringObject.transform.localEulerAngles = AngleLerp(hoveringObject.transform.localEulerAngles, localRestRotation, Time.deltaTime * lerpSpeed);
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

    public void SetRestRotation(Vector3 localEulerAngles)
    {
        localRestRotation = localEulerAngles;
    }

    private void SetSelected(bool selected)
    {
        if ((gameObject.layer == bloodLayer && !showColorsOnHover) || !changeLayerOnHover) return;
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
