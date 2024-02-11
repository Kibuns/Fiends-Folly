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
    [SerializeField] private GameObject bloodLinePrefab;
    [SerializeField] private GameObject bloodLettingPrefab;
    public float delayTime;
    public float lerpSpeed;
    public bool onRitualCollider;
    private PlayerInputActions playerInput;
    private bool shouldStartEmission;

    private void Start()
    {
        playerInput = new PlayerInputActions();
        playerInput.Enable();
        playerCam = Camera.main.transform;
        particles.enableEmission = false;
        bloodLettingPrefab.GetComponent<ParticleSystem>().enableEmission = false;
        bloodLettingPrefab.transform.parent = null;
    }

    private void Update()
    {
        bool triggeredThisFrame = false;
        if (playerInput.Player.LeftClick.triggered)
        {
            triggeredThisFrame = true;
            StartCoroutine(EnableEmissionCoroutine());
            Instantiate(bloodLinePrefab);
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
                    SetBloodLettingPosition(hit.point);
                    if (triggeredThisFrame)
                    {
                        transform.position = hit.point;
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(transform.position, hit.point, lerpSpeed * Time.deltaTime);
                    }
                    

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
            bloodLettingPrefab.GetComponent<ParticleSystem>().enableEmission = false;
        }
        triggeredThisFrame = false;


    }

    private void SetBloodLettingPosition(Vector3 hitPoint)
    {
        bloodLettingPrefab.transform.position = new Vector3(hitPoint.x, bloodLettingPrefab.transform.position.y, hitPoint.z);
    }

    private IEnumerator EnableEmissionCoroutine()
    {
        yield return new WaitForSeconds(delayTime);
        particles.enableEmission = true;
        bloodLettingPrefab.GetComponent<ParticleSystem>().enableEmission = true;

    }
}