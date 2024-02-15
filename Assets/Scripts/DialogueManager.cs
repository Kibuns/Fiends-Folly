using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] TMPro.TMP_Text dialogueTextField;
    [SerializeField] float shakeWaitTime;

    private Coroutine currentDialogueCoroutine;
    private Coroutine typingCoroutine;
    private Coroutine shakingCoroutine;
    private Vector3 originalPosition;
    private AudioSource audioSource;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        dialogueBox.SetActive(false);
        originalPosition = dialogueTextField.rectTransform.localPosition;
    }

    void Update()
    {
        
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if(currentDialogueCoroutine != null)
        {
            StopCoroutine(currentDialogueCoroutine);
            Debug.Log("Started a new dialogue coroutine before the current one was finished");
        }
        currentDialogueCoroutine = StartCoroutine(DialogueCoroutine(dialogue));
    }

    private IEnumerator DialogueCoroutine(Dialogue dialogue)
    {
        audioSource.clip = dialogue.typingSound;
        foreach (Sentence sentence in dialogue.sentences)
        {
            ShowSentence(sentence);
            yield return new WaitForSeconds(sentence.duration);
        }
        Debug.Log("dialogue done");
        dialogueBox.SetActive(false);

        if(typingCoroutine != null) { StopCoroutine(typingCoroutine); }

        if(shakingCoroutine != null) { StopCoroutine(shakingCoroutine); }
        
    }

    private void ShowSentence(Sentence sentence)
    {   
        if (shakingCoroutine != null)
        {
            StopCoroutine(shakingCoroutine);
        }
        dialogueBox.SetActive(true);
        dialogueTextField.color = sentence.textColor;
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeSentence(sentence));

        if (sentence.shouldShake)
        {
            shakingCoroutine = StartCoroutine(ShakeCoroutine(sentence));
        }

    }

    private IEnumerator TypeSentence(Sentence sentence)
    {
        dialogueTextField.text = "";
        string sentenceText = sentence.GetConvertedText().ToUpper(); //does the clock time thingy to replace "{clocktext}"
        foreach (char c in sentenceText)
        {
            dialogueTextField.text += c;
            audioSource.Play();
            yield return new WaitForSeconds(sentence.characterTypingTime);
        }
    }

    private IEnumerator ShakeCoroutine(Sentence sentence)
    {
        while (true)
        {
            yield return new WaitForSeconds(shakeWaitTime);
            Vector3 shakeOffset = new Vector3(Random.Range(-sentence.shakeOffsetDelta, sentence.shakeOffsetDelta), Random.Range(-sentence.shakeOffsetDelta, sentence.shakeOffsetDelta), 0);
            dialogueTextField.rectTransform.localPosition = originalPosition + shakeOffset;
        }
    }
}
