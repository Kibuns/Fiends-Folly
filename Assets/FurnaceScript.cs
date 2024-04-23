using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class FurnaceScript : MonoBehaviour, IInteractable
{
    [SerializeField] public Transform burningObjectSelectionPoint;
    [SerializeField] private AudioClip burnClip;
    public float lerpSpeed;
    private Quaternion startRotation;
    private Quaternion openRotation;
    private Quaternion currentTargetRotation;
    private Transform door;
    private AudioSource source;

    public bool doorIsOpen;


    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        door = GetComponent<HoveringObject>().hoveringObject.transform;
        startRotation = door.localRotation;
        openRotation = Quaternion.Euler(0, 90, 0);
        currentTargetRotation = startRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(doorIsOpen)
        {
            currentTargetRotation = openRotation;
            door.localRotation = Quaternion.Lerp(door.localRotation, currentTargetRotation, lerpSpeed * Time.deltaTime);
        }
        else
        {
            currentTargetRotation = startRotation;
            door.localRotation = Quaternion.Lerp(door.localRotation, currentTargetRotation, lerpSpeed * Time.deltaTime);
        }
    }
    public void EatDraggedObject(GameObject eatenObject)
    {
        string eatenObjectName = eatenObject.name;
        Debug.Log("eating: " + eatenObjectName);
        Destroy(eatenObject);
        GetComponentInChildren<CandleScript>().Pulse();
        StartCoroutine(ToggleDoorWithDelay(1f, false));
        source.PlayOneShot(burnClip);

        if (eatenObjectName == "RubberDuck(Clone)")
        {
            GameManager.Instance.StartGunSequence(false, 1.5f);
        }
        else
        {
            GameManager.Instance.StartGunSequence(true, 1.5f);
        }
    }

    private IEnumerator ToggleDoorWithDelay(float seconds, bool openState)
    {
        yield return new WaitForSeconds(seconds);
        doorIsOpen = openState;
    }

    public void Interact()
    {
        if (GameManager.Instance.isInGunSequence) return;
        if (Quaternion.Angle(door.localRotation, currentTargetRotation) > 12f) return;
        doorIsOpen = !doorIsOpen;
    }
}
