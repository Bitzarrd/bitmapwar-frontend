using System;
using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class AskForWebLogin : MsgBase
{
    public AskForWebLogin()
    {
        method = "AskForWebLogin";
    }
}

public class AskForWebSuccess : MsgBase
{
    public string code;
    public string url;
}

public class PCStartUp : MonoBehaviour
{
    public Button startLogin;
    public Button ExitGameBtn;
    public Button LoadingBtn;
    
    public static PCStartUp inst;

    private void Awake()
    {
        inst = this;
    }

    public void OnConnected()
    {
        startLogin.gameObject.SetActive(true);
        LoadingBtn.gameObject.SetActive(false);
    }
    
    public void QueryAccessToken()
    {
        if (WebSocketClient.inst.getState() != WebSocketState.Open)
        {
            Debug.Log("Not Connected");
            return;
        }
        var msg = new AskForWebLogin();
        WebSocketClient.inst.Send(JsonConvert.SerializeObject(msg));
    }
    
    // Start is called before the first frame update
    void Start()
    {
        startLogin.onClick.AddListener(QueryAccessToken);
        //ExitGameBtn.onClick.AddListener(() => Application.Quit(0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
