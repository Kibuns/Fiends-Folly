using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DrawWithMouse : MonoBehaviour
{
    private LineRenderer line;
    private Vector3 previousPosition;
    private Vector3 StartPosition;
    [SerializeField] Transform mouseFollowObject;
    private MouseFollow mouseFollow;
    [SerializeField] float minDistance;
    private PlayerInputActions input;
    private bool started;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        line.renderingLayerMask = 6;
        line.positionCount = 1;
        input = new PlayerInputActions();
        input.Enable();
        mouseFollow = mouseFollowObject.gameObject.GetComponent<MouseFollow>();
    }

    private void Update()
    {

        Vector3 currentPosition = mouseFollowObject.position;
        if (!mouseFollow.onRitualCollider) { return; }
        if ( input.Player.LeftClick.triggered )
        {
            line.SetPosition(0, currentPosition);
        }

        if (input.Player.LeftClick.IsPressed())
        {
            if (Vector3.Distance(previousPosition, currentPosition) > minDistance)
            {

                if(previousPosition == transform.position)
                {
                    line.SetPosition(0, currentPosition);
                }
                else
                {
                    AddLineSection(currentPosition);
                }
            }

            previousPosition = currentPosition;
        }
        //if(input.Player.LeftClick.WasReleasedThisFrame() && line != null)
        //{
        //    LineRenderer newLine = gameObject.AddComponent<LineRenderer>();
        //    line = newLine;
        //    line.positionCount = 1;
            
        //}



    }

    private void AddLineSection(Vector3 currentPosition)
    {
        line.positionCount++;
        line.SetPosition(line.positionCount - 1, currentPosition);
    }
}
