using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    [SerializeField] public Transform holdItemPosition;
    [SerializeField] public Item currentlyHeldItem;
    [SerializeField] public LayerMask dragLayerMask;
    [SerializeField] public CanvasGroup toolTipGroup;
    [SerializeField] public float tooltipFadeSpeed;



    [Header("Development options")]
    public bool updateOffsetEveryFrame; //turn off in build;

    private Vector3 holdItemStartPosition;
    private Quaternion holdItemStartRotation;
    private string LMBToolTip;
    private string RMBToolTip;
    private float UIStartAlpha;


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
        toolTipGroup.alpha = 0f;
        holdItemStartPosition = holdItemPosition.localPosition;
        holdItemStartRotation = holdItemPosition.localRotation;
        playerActions.Enable();

        Image[] images = toolTipGroup.gameObject.GetComponentsInChildren<Image>();
        UIStartAlpha = images[0].color.a;

    }


    void Update()
    {
        if(updateOffsetEveryFrame)
        {
            SetOffset();
        }
        if(playerActions.Player.Escape.triggered)
        {
            if (GameManager.Instance.isInGunSequence)
            {
                GameManager.Instance.PlayErrorSound();
                return;
            }
            DropHeldItem();
        }
        ShowHideToolTips();
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
        if (currentlyHeldItem != null) { DropHeldItem(); }
        currentlyHeldItem = item;
        SetToolTipsInCanvas();
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

    private void ShowHideToolTips()
    {
        if(isHoldingItem)
        {
            toolTipGroup.alpha = Mathf.Lerp(toolTipGroup.alpha, 1, tooltipFadeSpeed * Time.deltaTime);
        }
        else
        {
            toolTipGroup.alpha = Mathf.Lerp(toolTipGroup.alpha, 0, tooltipFadeSpeed * 5f * Time.deltaTime);
        }
    }

    public void SetToolTipsInCanvas()
    {
        LMBToolTip = currentlyHeldItem.LMBToolTip;
        RMBToolTip = currentlyHeldItem.RMBToolTip;
        TMP_Text[] textFields = toolTipGroup.gameObject.GetComponentsInChildren<TMP_Text>();
        Image[] images = toolTipGroup.gameObject.GetComponentsInChildren<Image>();

        if (string.IsNullOrEmpty(LMBToolTip))
        {
            textFields[0].alpha = 0f;

            Color tempColor = images[0].color;
            tempColor.a = 0f;
            images[0].color = tempColor;
        }
        else
        {
            textFields[0].alpha = UIStartAlpha;
            textFields[0].text = LMBToolTip;

            Color tempColor = images[0].color;
            tempColor.a = UIStartAlpha;
            images[0].color = tempColor;
        }

        if (string.IsNullOrEmpty(RMBToolTip))
        {
            textFields[1].text = "Drop Item";
        }
        else
        {
            textFields[1].text = RMBToolTip;
        }
    }

    public bool isHoldingItem => currentlyHeldItem != null;
}
