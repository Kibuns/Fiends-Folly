using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DrawWithMouse : MonoBehaviour
{
    private LineRenderer line;
    private Vector3 previousPosition;
    private Vector3 StartPosition;
    private Transform mouseFollowObject;
    private MouseFollow mouseFollow;
    [SerializeField] float minDistance;
    private PlayerInputActions input;
    private Material lineMaterial;
    private bool doneWithWriting;
    private bool writtenCorrectSymbol;
    private Color currentColor;


    private void OnEnable()
    {
        input = new PlayerInputActions();
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
    private void Start()
    {
        mouseFollowObject = GameObject.FindObjectOfType<MouseFollow>().transform;
        mouseFollow = mouseFollowObject.gameObject.GetComponent<MouseFollow>();
        line = GetComponent<LineRenderer>();
        line.renderingLayerMask = 6;
        line.positionCount = 1;
        lineMaterial = line.material;
    }

    private void Update()
    {
        if (doneWithWriting)
        {
            FadeOutAndSpawnItem();
            return;
        }
        if (!GameManager.Instance.isBleeding) return;

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
        if (input.Player.LeftClick.WasReleasedThisFrame() && line != null)
        {
            doneWithWriting = true;
            if (line.positionCount > 100)
            {
                writtenCorrectSymbol = true;
            }

            if (writtenCorrectSymbol)
            {
                GameManager.Instance.isBleeding = false;
            }
            
            
            // Change render mode to fade
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_ZWrite", 0);
            lineMaterial.DisableKeyword("_ALPHATEST_ON");
            lineMaterial.EnableKeyword("_ALPHABLEND_ON");
            lineMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            lineMaterial.renderQueue = 3000;

            // Update the material color to red with alpha value 0.5
            currentColor = new Color(1f, 0f, 0f, 0.9f);
            lineMaterial.SetColor("_Color", currentColor);
            return;
        }



    }

    private void FadeOutAndSpawnItem()
    {
        currentColor = new Color(1f, 0f, 0f, currentColor.a - (Time.deltaTime / 2));
        lineMaterial.SetColor("_Color", currentColor);
        if(currentColor.a < 0f)
        {
            //TEMP DEMO CODEvv

            if (writtenCorrectSymbol)
            {
                GameManager.Instance.SpawnRitualItem();
                GameManager.Instance.PlayBellSound();
            }


            //TEMP DEMO CODE^^
            Destroy(gameObject);
        }
    }

    private void AddLineSection(Vector3 currentPosition)
    {
        line.positionCount++;
        line.SetPosition(line.positionCount - 1, currentPosition);
    }
}
