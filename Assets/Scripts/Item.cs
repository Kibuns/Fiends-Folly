using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Item : MonoBehaviour
{

    private Transform restTransform;

    [SerializeField] public string LMBToolTip;
    [SerializeField] public string RMBToolTip;
    [SerializeField] public bool alwaysActiveCollider;

    [SerializeField] public Vector3 offsetPosition;
    [SerializeField] public Vector3 offsetRotation;

    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip putDownClip;
    [SerializeField] private float pitchRandomizationDelta = 0f;

    private Transform targetTransform;
    private AudioSource source;
    private Collider col;
    private PlayerInputActions playerInput;
    public bool isBeingHeld {  get; private set; }
    public float lerpSpeed = 10f;

    private void OnEnable()
    {
        playerInput = new PlayerInputActions();
        playerInput.Enable();
    }
    private void OnDisable()
    {
        playerInput.Disable();
    }
    void Start()
    {

        col = GetComponent<Collider>();
        if (alwaysActiveCollider) col.enabled = true;
        Transform startTransform = transform;
        restTransform = startTransform;
        targetTransform = ItemManager.Instance.holdItemPosition;
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isBeingHeld)
        {
            ToggleCollider(true);
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, lerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, lerpSpeed * Time.deltaTime);
        }
        else
        {
            ToggleCollider(false);
            transform.position = Vector3.Lerp(transform.position, restTransform.position, lerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, restTransform.rotation, lerpSpeed * Time.deltaTime);
        }
        
    }

    private void ToggleCollider(bool value)
    {
        if (col == null) return;
        if (alwaysActiveCollider) { 
            return;
        }
        col.enabled = value;
    }

    public bool TryPickUp()
    {
        if(isBeingHeld)
        {
            //Debug.LogError("object is already being held, why are you trying to pick it up again? pls fix");
            return false;
        }
        if(GameManager.Instance.isInGunSequence && GetComponent<RevolverScript>() == null)
        {
            GameManager.Instance.PlayErrorSound();
            return false;
        }
        try
        {
            if (ItemManager.Instance.currentlyHeldItem.transform.parent.TryGetComponent(out RazorScript razorScript) && razorScript.startedCuttingSequence) //if during cutting sequence, cant drop item
            {
                GameManager.Instance.PlayErrorSound();
                return false;
            }
        }
        catch
        {

        }

        ItemManager.Instance.SetHeldItem(this);
        Debug.Log("picked up: " + gameObject.name);
        isBeingHeld = true;
        if(!GetComponentInParent<HoveringObject>().playSoundOnHover)
        {
            PlayPickupSound();
        }
        return true;
    }

    public void Drop()
    {
        GetComponentInParent<HoveringObject>().EnableHoverable();
        isBeingHeld = false;
        PlayPutDownSound();
    }

    public void PlayPickupSound()
    {
        if (!pickupClip || GameManager.Instance.isDead) return;
        SetPitchToRandom(pitchRandomizationDelta);
        if (source.isPlaying) { source.Stop(); }
        source.PlayOneShot(pickupClip);

    }

    public void PlayPutDownSound()
    {
        if (!putDownClip || GameManager.Instance.isDead) return;
        SetPitchToRandom(pitchRandomizationDelta);
        source.PlayOneShot(putDownClip);

        //for phone
        if(TryGetComponent(out PhoneScript phoneScript))
        {
            if(phoneScript.isRinging) { phoneScript.StopRing(); }
            if(phoneScript.ringCount == 3 && !phoneScript.isAnswering)
            {
                GameManager.Instance.StartGunSequence(true, 0.5f, GameManager.DeathReason.WrongPhonePickup);
            }
            
        }
    }

    private void SetPitchToRandom(float maxPitchDelta)
    {
        if (maxPitchDelta == 0) return;
        source.pitch = Random.Range(1 - maxPitchDelta, 1 + maxPitchDelta);
    }

    private void OnMouseOver()
    {
        if (playerInput.Player.LeftClick.IsPressed())
        {
            CursorManager.instance.EnableDragCursor();
        }
        else
        {
            CursorManager.instance.EnablePointCursor();
        }
    }

    private void OnMouseExit()
    {
        CursorManager.instance.EnableDefaultCursor();
    }


}
