using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseRazorScript : MonoBehaviour
{
    private bool usedItem;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if(usedItem) { return; }
        usedItem = true;
        GetComponentInParent<RazorScript>().UseBladeToCut();
    }

    private void OnMouseEnter()
    {
        CursorManager.instance.EnablePointCursor();
    }

    private void OnMouseExit()
    {
        CursorManager.instance.EnableDefaultCursor();
    }
}
