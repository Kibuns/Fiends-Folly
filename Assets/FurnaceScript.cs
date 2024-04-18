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
    private Quaternion targetRotation;
    private Transform door;
    private AudioSource source;

    public bool doorIsOpen;


    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        door = GetComponent<HoveringObject>().hoveringObject.transform;
        startRotation = door.localRotation;
        targetRotation = Quaternion.Euler(0, 90, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(doorIsOpen)
        {
            door.localRotation = Quaternion.Lerp(door.localRotation, targetRotation, lerpSpeed * Time.deltaTime);
        }
        else
        {
            door.localRotation = Quaternion.Lerp(door.localRotation, startRotation, lerpSpeed * Time.deltaTime);
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
        doorIsOpen = !doorIsOpen;
    }
}
