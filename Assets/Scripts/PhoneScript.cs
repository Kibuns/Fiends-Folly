using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class PhoneScript : MonoBehaviour
{

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip defaultToneClip;
    [SerializeField] private AudioClip ringingClip;
    [SerializeField] private AudioClip voiceClip;
    [SerializeField] private Dialogue endDialogue;

    public bool startRing;

    int ringCount;

    public bool isRinging;
    public bool isAnswering;

    private Item item;

    void Start()
    {
        ringCount = 0;
        item = GetComponent<Item>();
    }

    void Update()
    {
        if(startRing)
        {
            startRing = false;
            StartRing();
        }

        if (item.isBeingHeld && isRinging && !isAnswering)
        {
            StopRing();
            PickUpRingingPhone();
        }
        if (item.isBeingHeld && !isRinging && !isAnswering)
        {
            StartDefaultTone();
        }

        if(!item.isBeingHeld && !isRinging)
        {
            isAnswering = false;
            source.Stop();
        }
    }

    private void PickUpRingingPhone()
    {
        if (ringCount < 3)
        {
            StartBreatheTone();
        }
        else
        {
            StartSilentTone();
        }
    }

    private void StartDefaultTone()
    {
        isAnswering = true;
        source.clip = defaultToneClip;
        source.volume = 0.3f;
        source.Play();
    }

    private void StartSilentTone()
    {
        isAnswering = true;
        source.clip = defaultToneClip;
        source.volume = 0f;
        source.Play();
        StartCoroutine(DialogueDelay(1.5f));
    }

    private IEnumerator DialogueDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        DialogueManager.instance.StartDialogue(endDialogue);
        yield return new WaitForSeconds(10);
        TarotManager.Instance.CompletePhoneTarot(0f);
    }

    private void StartBreatheTone()
    {
        isAnswering = true;
        source.clip = voiceClip;
        source.volume = 1f;
        source.Stop();
        source.Play();
        GameManager.Instance.StartGunSequence(true, 7f, GameManager.DeathReason.WrongPhonePickup);
    }

    public void StartRing()
    {
        isRinging = true;
        ringCount++;
        source.clip = ringingClip;
        source.Play();
    }

    public void StopRing()
    {
        isRinging = false;
        if (isAnswering) return;
        source.Stop();
    }
}
