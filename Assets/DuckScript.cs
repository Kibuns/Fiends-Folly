using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DuckScript : MonoBehaviour
{
    [SerializeField] private AudioClip QuackSound;
    [SerializeField] private float pitchDecreaseAmount = 0.05f;
    private bool startedSequence;
    private HoveringObject hoveringObjectScript;
    private Item itemScript;
    private AudioSource source;

    private bool sequenceStarted;
    private bool gotPickedUp;
    
    // Start is called before the first frame update
    void Start()
    {
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
    }

    private IEnumerator StartGunSequenceAfterQuackSound()
    {
        sequenceStarted = true;
        while (source.isPlaying)
        {
            yield return null;
        }
        GameManager.Instance.StartGunSequence(true, 0f);
    }
}
