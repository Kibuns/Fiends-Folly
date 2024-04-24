using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackboxScript : MonoBehaviour
{
    [SerializeField] private GameObject crank;
    private Item item;
    private float currentCrankSpeed = 0f;
    [SerializeField] private float maxCrankSpeed = 30f;

    [Header("Task Settings")]
    public float targetMinutes = 15f;
    public float minimumMinutesForFailure = 6f;
    public float marginForError = 1f;
    public float minutesWhenPhoneRings = 10f;
    public float phoneRingTime = 12.5f;

    private bool cranking;
    private AudioSource crankSource;
    private float crankStartTime;
    private bool calledDuringCranking;

    private float lerpSpeed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        item = GetComponent<Item>();
        crankSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(cranking && item.isBeingHeld)
        {
            currentCrankSpeed = Mathf.Lerp(currentCrankSpeed, maxCrankSpeed, lerpSpeed * Time.deltaTime);
            float minutesCranked = GameManager.Instance.GetMinutesPassed() - crankStartTime;
            if(minutesCranked > minutesWhenPhoneRings && !calledDuringCranking)
            {
                calledDuringCranking = true;
                GameManager.Instance.RingPhoneForSeconds(phoneRingTime, 0f);
            }
        }
        else
        {
            currentCrankSpeed = Mathf.Lerp(currentCrankSpeed, 0, lerpSpeed * 2.5f * Time.deltaTime);
        }
        crank.transform.Rotate(new Vector3(-currentCrankSpeed * Time.deltaTime, 0, 0));
        crankSource.pitch = Mathf.Lerp(0.75f, 1, currentCrankSpeed / maxCrankSpeed);
        crankSource.volume = Mathf.Lerp(0, 1, currentCrankSpeed / maxCrankSpeed);
        if (currentCrankSpeed / maxCrankSpeed < 0.5 && !cranking) { crankSource.Pause(); }
    }

    private void OnMouseDown()
    {
        if (!item.isBeingHeld) return;
        cranking = true;
        crankSource.Play();
        crankStartTime = GameManager.Instance.GetMinutesPassed();
        CursorManager.instance.EnableDragCursor();
    }

    private void OnMouseEnter()
    {
        if (!item.isBeingHeld) return;
        CursorManager.instance.EnablePointCursor();
    }

    private void OnMouseUp()
    {
        if (!item.isBeingHeld) return;
        if (!cranking) return;
        CursorManager.instance.EnablePointCursor();
        cranking = false;
        CallCrankTime();
    }

    private void OnMouseExit()
    {
        if (!item.isBeingHeld) return;
        CursorManager.instance.EnableDefaultCursor();
        if (!cranking) return;
        cranking = false;
        CallCrankTime();
    }

    private void CallCrankTime()
    {
        float minutesCranked = GameManager.Instance.GetMinutesPassed() - crankStartTime;
        Debug.Log("Cranked for minutes: " + minutesCranked.ToString());

        if(minutesCranked < minimumMinutesForFailure) { return; }
        if (Mathf.Abs(minutesCranked - targetMinutes) < marginForError)
        {
            GameManager.Instance.SpawnGeneralItem();
            GameManager.Instance.PlaySuccesSound();
        }
        else
        {
            if (FindObjectOfType<PhoneScript>().isRinging) return; // <<<<<<EXCEPTION TO THE RULE
            GameManager.Instance.StartGunSequence(true, 2f, GameManager.DeathReason.TooLongJackBox);
        }
    }
}
