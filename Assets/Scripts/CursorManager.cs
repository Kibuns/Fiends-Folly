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
    [SerializeField] private Texture2D ritualCursor;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        EnableDefaultCursor();
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

    public void EnableDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    public void EnableRitualCursor()
    {
        Cursor.SetCursor(ritualCursor, Vector2.zero, CursorMode.Auto);
    }

    public void EnableDeadCursor()
    {
        Cursor.SetCursor(deadCursor, Vector2.zero, CursorMode.Auto);
    }
}
