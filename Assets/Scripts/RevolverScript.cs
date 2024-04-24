using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolverScript : MonoBehaviour
{
    [SerializeField] private Transform cylinderTransform;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioClip shotClip;
    [SerializeField] private AudioClip blankShotClip;
    [SerializeField] private Vignette vignette;
    [SerializeField] private float fadeToBlackDelay = 0.04f;
    [SerializeField] private float vanishDelay = 0.5f;
    [SerializeField] private float vanishSpeed = 20f;

    public bool isLoaded;

    public bool hasShotBlank;
    private bool startedVanishSequence;
    private Collider col;
    private Item item;
    private AudioSource source;
    private AudioSource[] allAudioSources;
    private bool isBeingHeld;
    private bool startedScaryMusic;
    private int blankShotCounter;
    private Vector3 targetRotation;
    // Start is called before the first frame update

    void Awake()
    {
        if(vignette == null)
        {
            vignette = FindObjectOfType<Vignette>();
        }
    }

    private void StopAllAudio()
    {
        allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allAudioSources)
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
        LerpCylinderToTarget();
        if (item.isBeingHeld && !isBeingHeld)
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
            GetComponentInParent<HoveringObject>().gameObject.GetComponent<Collider>().enabled = false; //stops player from pickup up gun again
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
        GameManager.Instance.RingPhoneForSeconds(1000f, 11f);
    }

    private void LerpCylinderToTarget()
    {
        cylinderTransform.localEulerAngles = Vector3.Lerp(cylinderTransform.localEulerAngles, targetRotation, 30 * Time.deltaTime);
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
        if (!item.isBeingHeld || GameManager.Instance.isDead) return;
        if (isLoaded)
        {
            ShootLoaded();
        }
        else
        {
            TarotManager.Instance.CompleteSacrificeTarot(2.5f);
            item.RMBToolTip = "Drop Gun";
            ItemManager.Instance.SetToolTipsInCanvas();
            blankShotCounter++;
            targetRotation = new Vector3(0, 0, targetRotation.z + 60);
            if(blankShotCounter == 6) { ShootLoaded(); }
            GameManager.Instance.StopScaryMusic();
            source.PlayOneShot(blankShotClip);
            DialogueManager.instance.StopCurrentDialogue();
            GameManager.Instance.isInGunSequence = false;
            hasShotBlank = true;
        }
    }

    private void ShootLoaded()
    {
        GameManager.Instance.isDead = true;
        source.PlayOneShot(shotClip);
        DialogueManager.instance.StopCurrentDialogue();
        muzzleFlash.Play();
        StartCoroutine(FadeToBlack());
        FindObjectOfType<SceneManageScript>().BackToStartScreen();
    }

    private IEnumerator FadeToBlack()
    {
        yield return new WaitForSeconds(fadeToBlackDelay);
        vignette.roundness = 0f;
        StopAllAudio();
        GameManager.Instance.ToggleUIVisibility(false);
    }

    //maybe event system? on event item picked up, set the collider active so the OnMouse thingies work
}
