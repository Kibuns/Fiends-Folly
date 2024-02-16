using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Item : MonoBehaviour
{

    private Transform restTransform;

    [SerializeField] public Vector3 offsetPosition;
    [SerializeField] public Vector3 offsetRotation;

    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip putDownClip;

    private Transform targetTransform;
    private AudioSource source;
    public bool isBeingHeld {  get; private set; }
    public float lerpSpeed = 10f;
    void Start()
    {
        Transform startTransform = transform;
        restTransform = startTransform;
        targetTransform = ItemManager.Instance.holdItemPosition;
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isBeingHeld)
        {
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, lerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, lerpSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, restTransform.position, lerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, restTransform.rotation, lerpSpeed * Time.deltaTime);
        }
        
    }

    public void PickUp()
    {
        ItemManager.Instance.SetHeldItem(this);
        Debug.Log("picked up: " + gameObject.name);
        isBeingHeld = true;
        if(!GetComponentInParent<HoveringObject>().playSoundOnHover)
        {
            PlayPickupSound();
        }
    }

    public void Drop()
    {
        GetComponentInParent<HoveringObject>().EnableHoverable();
        isBeingHeld = false;
        PlayPutDownSound();
    }

    public void PlayPickupSound()
    {
        source.PlayOneShot(pickupClip);
    }

    public void PlayPutDownSound()
    {
        source.PlayOneShot(putDownClip);
    }


}
