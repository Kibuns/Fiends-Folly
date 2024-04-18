using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class TarotCardScript : MonoBehaviour
{
    private Item attachedItem;
    private bool turned;
    // Start is called before the first frame update
    void Start()
    {
        attachedItem = GetComponent<Item>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!attachedItem.isBeingHeld && turned)
        {
            turned = false;
            attachedItem.offsetRotation = new Vector3(attachedItem.offsetRotation.x, 0, attachedItem.offsetRotation.z);
        }
    }

    private void OnMouseEnter()
    {
        Debug.Log("enter");
        CursorManager.instance.EnablePointCursor();
    }

    private void OnMouseExit()
    {
        Debug.Log("exit");
        CursorManager.instance.EnableDefaultCursor();
    }

    private void OnMouseDown()
    {
        Debug.Log("turn");
        turned = !turned;
        if(turned)
        {
            attachedItem.offsetRotation = new Vector3(attachedItem.offsetRotation.x, 180, attachedItem.offsetRotation.z);
        }
        else
        {
            attachedItem.offsetRotation = new Vector3(attachedItem.offsetRotation.x, 0, attachedItem.offsetRotation.z);
        }
        
    }
}
