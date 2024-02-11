using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class RazorScript : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject razorBlade;
    [SerializeField] private Vector3 shownBladeEulerRotation;
    [SerializeField] private AudioClip showBladeClip;
    [SerializeField] private Dialogue firstCuttingDialogue;
    [SerializeField] private AudioClip cutBladeClip;
    public float lerpSpeed;
    public float showBladeDelay;
    private bool bladeShowing;
    private PlayerInputActions playerInputActions;
    private bool isCutting;
    private Transform childRazorTransform;
    private Vector3 downPosition;
    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        childRazorTransform = transform.GetChild(0);
        downPosition = childRazorTransform.localPosition + Vector3.down;
    }

    // Update is called once per frame
    void Update()
    {
        LerpBladeRotation(bladeShowing);
        if (isCutting)
        {
            LerpDown();
        }
    }

    private void LerpDown()
    {
        childRazorTransform.localPosition = Vector3.Lerp(childRazorTransform.localPosition, downPosition, lerpSpeed * Time.deltaTime);
    }

    public void ShowBlade()
    {
        bladeShowing = true;
    }

    public void HideBlade()
    {
        bladeShowing = false;
    }

    private void LerpBladeRotation(bool bladeShowing)
    {
        if (bladeShowing)
        {
            razorBlade.transform.localEulerAngles = AngleLerp(razorBlade.transform.localEulerAngles, shownBladeEulerRotation, lerpSpeed * Time.deltaTime);
        }
        else
        {
            razorBlade.transform.localEulerAngles = AngleLerp(razorBlade.transform.localEulerAngles, Vector3.zero, lerpSpeed * Time.deltaTime);
        }
        
    }

    void IInteractable.Interact()
    {
        StartCoroutine(ShowBladeAnimation(showBladeDelay));
    }

    private IEnumerator ShowBladeAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowBlade();
        source.PlayOneShot(showBladeClip);
        yield return new WaitForSeconds(delay);
        DialogueManager.instance.StartDialogue(firstCuttingDialogue);
        yield return new WaitForSeconds(7);
        isCutting = true;
        yield return new WaitForSeconds(delay);
        source.PlayOneShot(cutBladeClip);
        GameManager.instance.isBleeding = true;

    }

    //Replace Vector3.Lerp because that one cant cross an angle of 0, so for instance lerp from 10 to -10 is only possible with this method without weird behaviour
    private Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
    {
        float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
        float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
        float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
        Vector3 Lerped = new Vector3(xLerp, yLerp, zLerp);
        return Lerped;
    }
}
