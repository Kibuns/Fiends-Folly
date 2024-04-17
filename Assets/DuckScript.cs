using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DuckScript : MonoBehaviour
{
    private bool startedSequence;
    private HoveringObject hoveringObjectScript;
    private Item itemScript;
    // Start is called before the first frame update
    void Start()
    {
        hoveringObjectScript = GetComponentInParent<HoveringObject>();
        itemScript = GetComponent<Item>();
    }

    // Update is called once per frame
    void Update()
    {
        if(itemScript.isBeingHeld) { return; }
        if(GameManager.Instance.isTurnedAround && !startedSequence)
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
        transform.LookAt(FindObjectOfType<LookAroundCamera>().transform);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z); //set x rotation to 0 in order to keep the duck leveled with the table
        hoveringObjectScript.SetRestRotation(transform.localEulerAngles);
        Debug.Log("turned duck toward player");
    }
}
