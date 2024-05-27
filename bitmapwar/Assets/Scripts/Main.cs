using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnApplicationQuit()
    {
        WebSocketClient.inst.Close();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
