using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private Dialogue clockDialogue;
    public void Interact()
    {
        DialogueManager.instance.StartDialogue(clockDialogue);
    }
}
