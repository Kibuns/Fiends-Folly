using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletScript : MonoBehaviour, IInteractable
{
    private AudioSource source;
    [SerializeField] private Dialogue gunSequenceToiletDialogue;

    private bool setDialogue;
    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (GameManager.Instance.isInGunSequence && !setDialogue)
        {
            setDialogue = true;
            GetComponent<HoveringObject>().dialogue = gunSequenceToiletDialogue;
        }
    }
    public void Interact()
    {
        if (!source.isPlaying)
        {
            source.Play();
        }
    }



}
