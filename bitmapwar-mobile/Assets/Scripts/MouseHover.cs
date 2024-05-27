using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHover : MonoBehaviour
{
    public Texture2D cur;
    public bool isHover = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    void setCursor()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Cursor.SetCursor(cur, Vector2.zero, CursorMode.ForceSoftware);
#else
        Cursor.SetCursor(cur, Vector2.zero, CursorMode.Auto);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(GRoot.inst.touchTarget != null && !GRoot.inst.touchTarget.onClick.isEmpty)
        {
            if(!isHover) {
                isHover = true;
                //Cursor.SetCursor(cur, Vector2.zero, CursorMode.Auto);
                setCursor();
            }
        }
        else {
            if(isHover)
            {
                isHover = false;
                Cursor.SetCursor(null,  Vector2.zero, CursorMode.Auto);
            }
            
        }
        */
    }
}
