using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    private Transform playerCam;
    [SerializeField] private Collider ritualFloorCollider;
    public bool onRitualCollider;

    private void Start()
    {
        playerCam = Camera.main.transform;
    }

    private void Update()
    {
        // Raycast from mouse position into the world
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the raycast hits the ritualFloorCollider
            if (hit.collider == ritualFloorCollider)
            {
                onRitualCollider = true;
                CursorManager.instance.EnablePointCursor(); //BUGGYYYYYYYYYYYYYYYYYYYYYYYYYYY
                // Set the position of this object to the hit point
                transform.position = hit.point;
                
            }
            else
            {
                onRitualCollider = false;
            }
        }
        else
        {
            onRitualCollider = false;
        }
        
    }
}