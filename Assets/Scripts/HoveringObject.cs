using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoveringObject : MonoBehaviour
{
    public GameObject hoveringObject;
    public float hoverHeight;
    public float maxHoverHeightDelta;
    public float maxHoverRotationDelta;
    public float bobbingSpeedMultiplier;
    public Vector3 hoverRotation;
    public float lerpSpeed;

    private int selectedLayer = 6;
    private int highlightLayer = 3;
    private Vector3 startPosition;
    private Vector3 startRotation;
    private bool isHoveredOn;
    private bool isHovering;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = hoveringObject.transform.position;
        startRotation = hoveringObject.transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (!isHoveredOn)
        {
            LerpToStart();
            return;
        }

        Hover();
    }

    private void InitHover()
    {
        CursorManager.instance.EnablePointCursor();
    }

    private void OnMouseEnter()
    {
        isHoveredOn = true;
        InitHover();
    }

    private void OnMouseDown()
    {
        if (TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact();
        }
        else
        {
            Debug.LogWarning("No IInteractable component found on gameobject");
        }
    }

    private void OnMouseExit()
    {
        isHoveredOn = false;
        CursorManager.instance.DisablePointCursor();
        if (gameObject.layer != highlightLayer)
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
        float targetHoverHeight = hoverHeight + Mathf.Sin(Time.time * bobbingSpeedMultiplier) * maxHoverHeightDelta;
        Vector3 targetPos = new Vector3(startPosition.x, startPosition.y + targetHoverHeight, startPosition.z);
        hoveringObject.transform.position = Vector3.Lerp(hoveringObject.transform.position, targetPos, Time.deltaTime * lerpSpeed);


        float rotationBobbingMultiplier = 2f;
        float targetHoverRotationY = (hoverRotation.y + Mathf.Sin(Time.time * bobbingSpeedMultiplier * rotationBobbingMultiplier) * maxHoverRotationDelta);
        Vector3 targetRotation = new Vector3(hoverRotation.x, targetHoverRotationY, hoverRotation.z);
        hoveringObject.transform.localEulerAngles = AngleLerp(hoveringObject.transform.localEulerAngles, targetRotation, Time.deltaTime * lerpSpeed);
    }

    private void LerpToStart()
    {
        hoveringObject.transform.position = Vector3.Lerp(hoveringObject.transform.position, startPosition, Time.deltaTime * lerpSpeed);
        hoveringObject.transform.localEulerAngles = AngleLerp(hoveringObject.transform.localEulerAngles, startRotation, Time.deltaTime * lerpSpeed);
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
