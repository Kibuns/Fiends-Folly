using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckScript : MonoBehaviour
{
    [SerializeField] private AudioClip QuackSound;
    [SerializeField] private Dialogue smallOwieDialogue;
    [SerializeField] private Dialogue mediumOwieDialogue;
    [SerializeField] private Dialogue largeOwieDialogue;
    [SerializeField] private float pitchDecreaseAmount = 0.05f;
    private bool startedSequence;
    private HoveringObject hoveringObjectScript;
    private Item itemScript;
    private AudioSource source;

    private bool sequenceStarted;
    private bool gotPickedUp;
    private int peepCount;
    
    // Start is called before the first frame update
    void Start()
    {
        peepCount = 0;
        source = GetComponent<AudioSource>();
        hoveringObjectScript = GetComponentInParent<HoveringObject>();
        itemScript = GetComponent<Item>();
    }

    // Update is called once per frame
    void Update()
    {
        if(itemScript.isBeingHeld)
        {
            gotPickedUp = true;
            return;
        }
        if(GameManager.Instance.isTurnedAround && !startedSequence && gotPickedUp)
        {
            startedSequence = true;
            StartCoroutine(turnTowardsPlayer());
        }
        else if(!GameManager.Instance.isTurnedAround)
        {
            startedSequence = false;
        }
    }

    private IEnumerator turnTowardsPlayer()
    {
        yield return new WaitForSeconds(0.3f);
        if (!GameManager.Instance.isTurnedAround) yield break;
        // Get the direction from the duck to the camera
        Vector3 directionToCamera = FindObjectOfType<LookAroundCamera>().transform.position - hoveringObjectScript.transform.position;

        // Calculate the angle between the direction the duck is facing and the direction to the camera
        float angle = Vector3.Angle(hoveringObjectScript.transform.forward, directionToCamera);

        // Make the duck look at the camera
        hoveringObjectScript.transform.LookAt(FindObjectOfType<LookAroundCamera>().transform);
        hoveringObjectScript.ResetRestRotation();


        Debug.Log("angle: " + angle);

        // Check if the angle is above your specified threshold (e.g., 15 degrees)
        if (angle > 40f)
        {
            // Play the quack sound
            source.PlayOneShot(QuackSound, 0.5f);
            Debug.Log("turned duck toward player");
        }

        // Set x rotation to 0 in order to keep the duck leveled with the table
        hoveringObjectScript.transform.eulerAngles = new Vector3(0, hoveringObjectScript.transform.localEulerAngles.y, hoveringObjectScript.transform.localEulerAngles.z);
    }

    private void OnMouseDown()
    {
        if (!itemScript.isBeingHeld) return;
        itemScript.PlayPickupSound();
        if (source.pitch - pitchDecreaseAmount <= 0.2f)
        {
            if(!sequenceStarted) { StartCoroutine(StartGunSequenceAfterQuackSound()); }
            return;
        }
        source.pitch -= pitchDecreaseAmount;
        peepCount++;
        if( peepCount == 5 )
        {
            DialogueManager.instance.StartDialogue(smallOwieDialogue);
        }

        if (peepCount == 30)
        {
            DialogueManager.instance.StartDialogue(mediumOwieDialogue);
        }

        if (peepCount == 80)
        {
            DialogueManager.instance.StartDialogue(largeOwieDialogue);
        }

    }

    private IEnumerator StartGunSequenceAfterQuackSound()
    {
        sequenceStarted = true;
        while (source.isPlaying)
        {
            yield return null;
        }
        GameManager.Instance.StartGunSequence(true, 0f, GameManager.DeathReason.TooManyQuacks);
    }
}
