using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private Dialogue clockDialogue;
    public void Interact()
    {
        Debug.Log("Clock interact");
        DialogueManager.instance.StartDialogue(clockDialogue);
    }
}
