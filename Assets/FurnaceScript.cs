using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceScript : MonoBehaviour, IInteractable
{
    public float lerpSpeed;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    private Transform door;

    private bool open;


    // Start is called before the first frame update
    void Start()
    {
        door = GetComponent<HoveringObject>().hoveringObject.transform;
        startRotation = door.localRotation;
        targetRotation = Quaternion.Euler(0, 90, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(open)
        {
            door.localRotation = Quaternion.Lerp(door.localRotation, targetRotation, lerpSpeed * Time.deltaTime);
        }
        else
        {
            door.localRotation = Quaternion.Lerp(door.localRotation, startRotation, lerpSpeed * Time.deltaTime);
        }
    }

    public void Interact()
    {
        open = !open;
        Debug.Log(open);
    }
}
