using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager instance;

    [SerializeField] private Texture2D pointCursor;
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D dragCursor;
    [SerializeField] private Texture2D deadCursor;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void EnableDragCursor()
    {
        Cursor.SetCursor(dragCursor, Vector2.zero, CursorMode.Auto);
    }

    public void EnablePointCursor()
    {
        Cursor.SetCursor(pointCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SwitchToDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    public void EnableDeadCursor()
    {
        Cursor.SetCursor(deadCursor, Vector2.zero, CursorMode.Auto);
    }
}
