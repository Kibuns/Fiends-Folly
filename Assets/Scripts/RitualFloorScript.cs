using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RitualFloorScript : MonoBehaviour
{
    private void OnMouseOver()
    {
        if(GameManager.Instance.isBleeding)
        {
            CursorManager.instance.EnableRitualCursor();
        }
        else
        {
            CursorManager.instance.EnableDefaultCursor();
        }
    }

    private void OnMouseExit()
    {
        CursorManager.instance.EnableDefaultCursor();
    }
}
