using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RazorScript : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject razorBlade;
    [SerializeField] private Vector3 shownBladeEulerRotation;
    public float lerpSpeed;
    public float showBladeDelay;
    private bool bladeShowing;
    private PlayerInputActions playerInputActions;
    // Start is called before the first frame update
    void Start()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        LerpBladeRotation(bladeShowing);
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
        StartCoroutine(ShowBladeDelay(showBladeDelay));
    }

    private IEnumerator ShowBladeDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowBlade();
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
