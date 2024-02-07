using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager instance;

    [SerializeField] private Texture2D pointCursor;
    [SerializeField] private Texture2D grabCursor;


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

    public void EnablePointCursor()
    {
        Cursor.SetCursor(pointCursor, Vector2.zero, CursorMode.Auto);
    }

    public void DisablePointCursor()
    {
        Cursor.SetCursor(grabCursor, Vector2.zero, CursorMode.Auto);
    }
}
