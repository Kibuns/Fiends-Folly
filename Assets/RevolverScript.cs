using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolverScript : MonoBehaviour
{
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioClip shotClip;
    [SerializeField] private AudioClip blankShotClip;
    [SerializeField] private Vignette vignette;
    [SerializeField] private float fadeToBlackDelay = 0.04f;

    public bool isLoaded;

    private Collider collider;
    private Item item;
    private AudioSource source;
    private AudioSource[] allAudioSources;
    private bool isBeingHeld;
    // Start is called before the first frame update

    void Awake()
    {
        allAudioSources = FindObjectsOfType<AudioSource>();
        if(vignette == null)
        {
            vignette = FindObjectOfType<Vignette>();
        }
    }

    private void StopAllAudio()
    {
        foreach(AudioSource source in allAudioSources)
        {
            source.Stop();
        }
    }
    void Start()
    {
        collider = GetComponent<Collider>();
        collider.enabled = false;
        item = GetComponent<Item>();
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(item.isBeingHeld && !isBeingHeld)
        {
            isBeingHeld = true;
            collider.enabled = true;
            Debug.Log("collider enabled");
        }
        else if (!item.isBeingHeld && isBeingHeld)
        {
            isBeingHeld = false;
            collider.enabled = false;
            Debug.Log("collider disabled");
        }
    }

    private void OnMouseOver()
    {
        if (!item.isBeingHeld) return;
        CursorManager.instance.EnableDeadCursor();
    }

    private void OnMouseExit()
    {
        if (!item.isBeingHeld) return;
        CursorManager.instance.SwitchToDefaultCursor();
    }

    private void OnMouseDown()
    {
        if (!item.isBeingHeld) return;
        if (isLoaded)
        {
            source.PlayOneShot(shotClip);
            DialogueManager.instance.StopCurrentDialogue();
            muzzleFlash.Play();
            StartCoroutine(FadeToBlack());
        }
        else
        {
            source.PlayOneShot(blankShotClip);
            DialogueManager.instance.StopCurrentDialogue();
            GameManager.Instance.isInGunSequence = false;
        }

    }

    private IEnumerator FadeToBlack()
    {
        yield return new WaitForSeconds(fadeToBlackDelay);
        vignette.roundness = 0f;
        StopAllAudio();
    }

    //maybe event system? on event item picked up, set the collider active so the OnMouse thingies work
}
