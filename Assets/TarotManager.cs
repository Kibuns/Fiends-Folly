using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarotManager : MonoBehaviour
{
    public GameObject timeTarot;


    public bool activate;
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
            StartCoroutine(CompleteTarrotCard(timeTarot.GetComponent<HoveringObject>()));
        }

    }

    private IEnumerator CompleteTarrotCard(HoveringObject tarotCard)
    {
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
        tarotCard.GetComponent<HoveringObject>().ToggleArtificialHover(true);
    }
}
