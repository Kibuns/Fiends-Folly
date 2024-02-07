using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private Vector3 offset;
    private bool isPlaced;

    private void OnMouseDown()
    {
        isPlaced = true;
    }

    private void OnMouseDrag()
    {
        
    }

    void Start()
    {
        offset = transform.position - BuildingSystem.GetMouseWorldPosition();
        Debug.Log(offset);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isPlaced = true;
        }
        if (isPlaced) return;
        Vector3 pos = BuildingSystem.GetMouseWorldPosition() + offset;
        transform.position = BuildingSystem.current.SnapCoordinateToGrid(pos);
    }
}
