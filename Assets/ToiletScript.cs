using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletScript : MonoBehaviour, IInteractable
{
    private AudioSource source;
    private void Start()
    {
        source = GetComponent<AudioSource>();
    }
    public void Interact()
    {
        if (!source.isPlaying)
        {
            source.Play();
        }
    }

}
