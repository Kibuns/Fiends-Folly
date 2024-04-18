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
    [SerializeField] private float vanishDelay = 0.5f;
    [SerializeField] private float vanishSpeed = 20f;

    public bool isLoaded;

    private bool hasShotBlank;
    private bool startedVanishSequence;
    private Collider col;
    private Item item;
    private AudioSource source;
    private AudioSource[] allAudioSources;
    private bool isBeingHeld;
    private bool startedScaryMusic;
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
        col = GetComponent<Collider>();
        col.enabled = false;
        item = GetComponent<Item>();
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(item.isBeingHeld && !isBeingHeld)
        {
            isBeingHeld = true;
            col.enabled = true;
        }
        else if (!item.isBeingHeld && isBeingHeld)
        {
            isBeingHeld = false;
            col.enabled = false;
        }

        if(item.isBeingHeld && !startedScaryMusic)
        {
            startedScaryMusic = true;
            GameManager.Instance.PlayScaryMusic();
        }

        if(!startedVanishSequence && hasShotBlank && !isBeingHeld)
        {
            startedVanishSequence = true;
            Debug.Log("START VANISH GUN");
            StartCoroutine(VanishAfterBlankShotSequence());
        }
    }

    private IEnumerator VanishAfterBlankShotSequence()
    {
        yield return new WaitForSeconds(vanishDelay);
        HoveringObject hoveringObject = GetComponentInParent<HoveringObject>();
        hoveringObject.DisableHoverable();
        Transform parentTransform = hoveringObject.transform;
        Vector3 targetPosition = new Vector3(parentTransform.position.x, parentTransform.position.y + 20f, parentTransform.position.z);

        float timer = 0f;
        while(timer < 3f)
        {
            timer += Time.deltaTime;
            parentTransform.position = Vector3.Lerp(parentTransform.position, targetPosition, vanishSpeed * Time.deltaTime);
            yield return null;
        }
        GameManager.Instance.PlaySuccesSound();
        GameManager.Instance.RingPhoneForSeconds(1000f, 11f);
    }

    private void OnMouseOver()
    {
        if (!item.isBeingHeld) return;
        CursorManager.instance.EnableDeadCursor();
    }

    private void OnMouseExit()
    {
        if (!item.isBeingHeld) return;
        CursorManager.instance.EnableDefaultCursor();
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
            FindObjectOfType<SceneManageScript>().BackToStartScreen();
        }
        else
        {
            GameManager.Instance.StopScaryMusic();
            source.PlayOneShot(blankShotClip);
            DialogueManager.instance.StopCurrentDialogue();
            GameManager.Instance.isInGunSequence = false;
            hasShotBlank = true;
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
