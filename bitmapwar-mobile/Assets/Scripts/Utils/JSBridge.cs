using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class ConnectInfo
{
    public string btcAddress;
    public string merlinAddress;
    public string publicKey;
    public string message;
    public string sig;
}

[Serializable]
public class ExtractInfo
{
    public string addr;
    public string amount;
    public string sig;
    public int nounce;
}

public class JSBridge : MonoBehaviour
{
    public static JSBridge inst;

    [DllImport("__Internal")]
    public static extern void ConnectWallet(string message);
    
    [DllImport("__Internal")]
    public static extern void ConnectWalletV2(int index);
    
    [DllImport("__Internal")]
    public static extern void BitmapExtract(string infostr);
    
    [DllImport("__Internal")]
    public static extern void JSBuySoldier(int count);
    
    [DllImport("__Internal")]
    public static extern void Disconnect();
    
    private void Awake()
    {
        inst = this;
    }

    public void JSDisconnect()
    {
        Debug.Log("Disconnect Partical SDK From Unity");
        #if UNITY_WEBGL && !UNITY_EDITOR
            Disconnect();
        #endif
    }

    public void Extract(string acc, string amount, string sig, int n, string orderTime)
    {
        var info = new ExtractInfo()
        {
            addr = acc,
            amount = amount,
            sig = sig,
            nounce = n,
        };
        var infostr = JsonConvert.SerializeObject(info);
        Debug.Log("Extract from Unity: " + acc + " Amout:" + amount);
        #if UNITY_WEBGL && !UNITY_EDITOR
        BitmapExtract(infostr);
        #endif
    }

    public void BuySoldier(int amount)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        JSBuySoldier(amount);
        #endif
    }
    
    public void ConnectToWallet(int index = 0)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        #if BITMAP_PAY
        ConnectWalletV2(index);
        #else
        ConnectWallet("Connect Wallet");
        #endif
        #endif
    }
    
    public void WalletResult(string jsonString)
    {
        string[] stringArray = JsonConvert.DeserializeObject<string[]>(jsonString);
        
        foreach (string str in stringArray)
        {
            Debug.Log("Account: " + str);
        }

        var acc = stringArray[0];
        ProtoUtils.SendLogin(acc);
        //ProtoUtils.SendLogin(jsonString);
    }
    public void WalletResultV2(string resInfo)
    {
        Debug.Log("WalletResult V2 Connection: " + resInfo);
        var ri = JsonConvert.DeserializeObject<ConnectInfo>(resInfo);
        ProtoUtils.SendLogin(ri.publicKey, ri.message, ri.sig);
        //ProtoUtils.SendLogin(acc);
    }

    public void OnPayResult(string orderID)
    {
        ProtoUtils.SendPayReulst(orderID);
    }
}
