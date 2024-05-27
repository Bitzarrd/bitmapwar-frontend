using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NativeWebSocket;
using UnityEngine;

public class WebSocketClient : MonoBehaviour
{
    static WebSocket websocket;

    public bool IsRelease = false; 

    public static WebSocketClient inst;
    public bool isConnected = false;

    public string serverURL;

    private float elapsedTime = 0.0f;

    private void Awake()
    {
        inst = this;
        /*
        if (IsRelease)
        {
            serverURL = "wss://test.bitmapwar.com/api";
        }
        else
        {
            serverURL = "wss://dev-server.bitmapwar.com";
        }
        */
    }

    public void Reconnect()
    {
        websocket = new WebSocket(serverURL);

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            LoadingPage.Inst.Close();
            //ProtoUtils.SendLogin();
        };

        websocket.OnError += (e) =>
        {
            if (!Game.Inst.isLogingOut)
            {
                Debug.Log("Error! " + e);
                MainPage.inst.OnLogout();
            }
        };

        websocket.OnClose += (e) =>
        {
            //MainPage.inst.OnLogout();
            Reconnect();
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            // Reading a plain text message
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            //Debug.Log("Message:" + message);
            ProtoUtils.Parse(message);
        };
        
        LoadingPage.Inst.Open();
        websocket.Connect();
    }

    private void Start()
    {
        websocket = new WebSocket(serverURL);

        websocket.OnOpen += () =>
        {
            isConnected = true;
            Debug.Log("Connection open!");
            //ProtoUtils.SendLogin();
        };

        websocket.OnError += (e) =>
        {
            //Toast.Inst.Open("Network Error!");
            Debug.Log("Error! " + e);
            //MainPage.inst.OnLogout();
        };

        websocket.OnClose += (e) =>
        {
            isConnected = false;
            if (!Game.Inst.isLogingOut)
            {
                Toast.Inst.Open("Network Error! Please Refresh Page");
            }

            //MainPage.inst.OnLogout();
            Debug.Log("Connection closed!");
            Reconnect();
            Debug.Log("Connection Created!");
        };
        
        websocket.OnMessage += (bytes) =>
        {
            // Reading a plain text message
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            //Debug.Log("Message:" + message);
            ProtoUtils.Parse(message);
        };
        
        Debug.Log("Connecting to Server...");

        websocket.Connect();
    }

    public void Send(string data)
    {
        Debug.Log("Send Message : " + data);
        websocket.SendText(data);
    }

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
        if (isConnected)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > 15f)
            {
                Debug.Log("Send Ping");
                websocket.SendText("{}");
                elapsedTime = 0f;
            }
        }
    }

    public void Close()
    {
        websocket.Close();
    }
}
