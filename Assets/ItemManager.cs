using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    [SerializeField] public Transform holdItemPosition;
    [SerializeField] public Item currentlyHeldItem;


    [Header("Development options")]
    public bool updateOffsetEveryFrame; //turn off in build;

    private Vector3 holdItemStartPosition;
    private Quaternion holdItemStartRotation;


    private PlayerInputActions playerActions;


    private void Awake()
    {
        playerActions = new PlayerInputActions();
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        holdItemStartPosition = holdItemPosition.localPosition;
        holdItemStartRotation = holdItemPosition.localRotation;
        playerActions.Enable();
    }


    void Update()
    {
        if(updateOffsetEveryFrame)
        {
            SetOffset();
        }
        if(playerActions.Player.Escape.triggered)
        {
            DropHeldItem();
        }
    }

    private void SetOffset()
    {
        if(currentlyHeldItem == null) { return; }
        holdItemPosition.localPosition = holdItemStartPosition;
        holdItemPosition.localRotation = holdItemStartRotation;
        holdItemPosition.localPosition += currentlyHeldItem.offsetPosition;
        holdItemPosition.localRotation *= Quaternion.Euler(currentlyHeldItem.offsetRotation);

    }

    public void SetHeldItem(Item item)
    {
        currentlyHeldItem = item;
        holdItemPosition.localPosition += currentlyHeldItem.offsetPosition;
        holdItemPosition.localRotation *= Quaternion.Euler(currentlyHeldItem.offsetRotation);
    }

    public void DropHeldItem()
    {
        if (currentlyHeldItem != null)
        {
            currentlyHeldItem.Drop();
            currentlyHeldItem = null;
            holdItemPosition.localPosition = holdItemStartPosition;
            holdItemPosition.localRotation = holdItemStartRotation;
        }
        else
        {
            Debug.Log("nothing to drop");
        }

    }
}
