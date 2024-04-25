using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookPileScript : MonoBehaviour
{
    private Rigidbody[] rbs;
    public float force;
    public float distanceCheckDelay;
    public float distanceThreshold;

    private bool activated;

    void Start()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
    }

    private void OnMouseDown()
    {
        if (activated) return;
        activated = true;
        GetComponent<Collider>().enabled = false;
        foreach(Rigidbody rb in rbs)
        {
            rb.AddForce(Vector3.right * force);
        }

    }


}
