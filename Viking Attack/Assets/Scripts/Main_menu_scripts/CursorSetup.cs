using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSetup : MonoBehaviour
{
    [SerializeField] private Texture2D cursor;
    void Start()
    {
        Cursor.SetCursor(cursor, new Vector2(3,3), CursorMode.ForceSoftware);
    }

}
