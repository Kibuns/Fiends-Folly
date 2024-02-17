using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneScript : MonoBehaviour
{

    [SerializeField] private AudioSource toneSource;

    private Item item;
    // Start is called before the first frame update
    void Start()
    {
        item = GetComponent<Item>();
    }

    // Update is called once per frame
    void Update()
    {
        if (item.isBeingHeld && !toneSource.isPlaying)
        {
            toneSource.Play();
        }
        else if (!item.isBeingHeld && toneSource.isPlaying)
        {
            toneSource.Stop();
        }
    }
}
