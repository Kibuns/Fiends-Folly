using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    private Transform restTransform;

    [SerializeField] public Vector3 offsetPosition;
    [SerializeField] public Vector3 offsetRotation;

    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip putDownClip;

    private Transform targetTransform;
    private AudioSource source;
    private Collider col;
    public bool isBeingHeld {  get; private set; }
    public float lerpSpeed = 10f;
    void Start()
    {
        col = GetComponent<Collider>();
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
        col.enabled = value;
    }

    public bool TryPickUp()
    {
        if(isBeingHeld)
        {
            Debug.LogError("object is already being held, why are you trying to pick it up again? pls fix");
            return false;
        }
        if(GameManager.Instance.isInGunSequence && GetComponent<RevolverScript>() == null)
        {
            GameManager.Instance.PlayErrorSound();
            return false;
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
        if (!pickupClip) return;
        if(source.isPlaying) { source.Stop(); }
        source.PlayOneShot(pickupClip);

    }

    public void PlayPutDownSound()
    {
        if (!putDownClip) return;
        source.PlayOneShot(putDownClip);
    }


}
