using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;

public class MouseFollow : MonoBehaviour
{
    private Transform playerCam;
    [SerializeField] private Collider ritualFloorCollider;
    [SerializeField] private ParticleSystem particles;
    public float delayTime;
    public bool onRitualCollider;
    private PlayerInputActions playerInput;
    private bool shouldStartEmission;

    private void Start()
    {
        playerInput = new PlayerInputActions();
        playerInput.Enable();
        playerCam = Camera.main.transform;
        particles.enableEmission = false;
    }

    private void Update()
    {
        if (playerInput.Player.LeftClick.triggered)
        {
            StartCoroutine(EnableEmissionCoroutine());
        }
        if (playerInput.Player.LeftClick.IsPressed())
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
        if (playerInput.Player.LeftClick.WasReleasedThisFrame())
        {
            particles.enableEmission = false;
        }
        
        
    }

    private IEnumerator EnableEmissionCoroutine()
    {
        yield return new WaitForSeconds(delayTime);
        particles.enableEmission = true;

    }
}