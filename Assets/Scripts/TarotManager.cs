using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarotManager : MonoBehaviour
{
    public HoveringObject timeTarot;
    public HoveringObject sacrificeTarot;
    public HoveringObject phoneTarot;
    public HoveringObject ritualTarot;

    public GameObject flameVFXPrefab;
    [SerializeField] private Transform completeTargetPoint;
    [SerializeField] private float lerpSpeed;

    Coroutine sacrificeCoroutine;

    public bool activate;
    public static TarotManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (activate)
        {
            activate = false;
            StartCoroutine(CompleteTarrotCardSequence(timeTarot, 0));
        }

    }

    public void CompleteTimeTarot(float delay)
    {
        StartCoroutine(CompleteTarrotCardSequence(timeTarot, delay));
    }

    public void CompleteSacrificeTarot(float delay)
    {
        if (sacrificeCoroutine != null) return;
        sacrificeCoroutine = StartCoroutine(CompleteTarrotCardSequence(sacrificeTarot, delay));
    }

    public void CompletePhoneTarot(float delay)
    {
        StartCoroutine(CompleteTarrotCardSequence(phoneTarot, delay));
    }

    public void CompleteRitualTarot(float delay)
    {
        StartCoroutine(CompleteTarrotCardSequence(ritualTarot, delay));
    }





    private IEnumerator CompleteTarrotCardSequence(HoveringObject tarotCard, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (GameManager.Instance.isDead) yield break;
        LookAroundCamera cam = FindObjectOfType<LookAroundCamera>();
        while (cam.turnedAround)
        {
            yield return null;
            //stuck in loop until player is facing the table
        }

        //Drop held item if it's the tarot card we're about to complete
        if (ItemManager.Instance.currentlyHeldItem == tarotCard.attachedItem)
        {
            ItemManager.Instance.DropHeldItem();
        }
        yield return new WaitForSeconds(0.3f);
        tarotCard.ToggleArtificialHover(true);
        yield return new WaitForSeconds(0.01f);

        Transform tarrotItemPoint = tarotCard.GetComponentInChildren<TarotCardScript>().transform;
        tarrotItemPoint.parent = null;
        GameObject flameVFXObject = Instantiate(flameVFXPrefab);

        
        Vector3 newFramePosition = tarotCard.transform.position;
        Vector3 newFrameRotation = tarotCard.transform.eulerAngles;
        while (Vector3.Distance(newFramePosition, completeTargetPoint.position) > 0.1f)
        {
            Debug.Log(Vector3.Distance(newFramePosition, completeTargetPoint.position));
            newFramePosition = Vector3.Slerp(newFramePosition, completeTargetPoint.position, lerpSpeed * Time.deltaTime);
            tarotCard.SetRestPostion(newFramePosition);
            flameVFXObject.transform.position = newFramePosition;

            newFrameRotation = Vector3.Slerp(newFrameRotation, completeTargetPoint.eulerAngles, lerpSpeed * Time.deltaTime);
            tarotCard.SetHoverRotation(newFrameRotation);
            yield return null;
        }
        if (GameManager.Instance.isDead) yield break;
        GameManager.Instance.PlaySuccesSound();

        Vector3 endposition = new Vector3(completeTargetPoint.position.x, completeTargetPoint.position.y + 10, completeTargetPoint.position.z);
        AudioSource vfxSource = flameVFXObject.GetComponent<AudioSource>();
        while (vfxSource.volume > 0f)
        {
            vfxSource.volume -= Time.deltaTime;
            newFramePosition = Vector3.Slerp(newFramePosition, endposition, lerpSpeed * Time.deltaTime);
            tarotCard.SetRestPostion(newFramePosition);
            flameVFXObject.transform.position = newFramePosition;
            yield return null;
        }
    }
}
