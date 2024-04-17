using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private Dialogue clockDialogue;
    [SerializeField] private Dialogue gunSequenceClockDialogue;
    public void Interact()
    {
        if (GameManager.Instance.isInGunSequence)
        {
            DialogueManager.instance.StartDialogue(gunSequenceClockDialogue);
        }
        else
        {
            DialogueManager.instance.StartDialogue(clockDialogue);
        }
    }
}
