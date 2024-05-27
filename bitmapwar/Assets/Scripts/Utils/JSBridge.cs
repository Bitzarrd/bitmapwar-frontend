using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class RentInfo
{
    public string txid;
    public int map_id;
}

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
    public static extern void RetryExtract(string infostr);
    
    [DllImport("__Internal")]
    public static extern void JSBuySoldier(int count);
    
    [DllImport("__Internal")]
    public static extern void Disconnect();
    
    [DllImport("__Internal")]
    public static extern void Rent(int id, int days);
    
    [DllImport("__Internal")]
    public static extern void GetBalance();

    public void OnBalance(string res)
    {
        Game.Inst.btcRemain = (double)BigInteger.Parse(res) /  (double)1e18;
        Debug.Log("OnBalance: " + res + " Btc: " + Game.Inst.btcRemain);
    }
    
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

    public void GetBtcBalance()
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
        GetBalance();
        #endif
    }

    public void JSRent(int id, int days)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        Rent(id, days);
        #endif
    }
    
    public void ReExtract(string acc, string amount, string sig, int n, string orderTime)
    {
        var info = new ExtractInfo()
        {
            addr = acc,
            amount = amount,
            sig = sig,
            nounce = n,
        };
        var infostr = JsonConvert.SerializeObject(info);
        Debug.Log("ReExtract from Unity: " + acc + " Amout:" + amount);
        #if UNITY_WEBGL && !UNITY_EDITOR
        RetryExtract(infostr);
        #endif
    }

    public void OnRetryExtract(string infostr)
    {
        Debug.Log("OnRetryExtract: " + infostr);

        var msg = JsonConvert.DeserializeObject<UpdateExtract>(infostr);
        msg.method = "UpdateExtract";
            /*
        {
            method = "UpdateExtract",
            id = data.id,
            txid = data.txid
        };
        */
        
        WebSocketClient.inst.Send(JsonConvert.SerializeObject(msg));
        
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

    public void OnExtractResponse(string info)
    {
        Debug.Log("OnExtract Response :" + info);
        var msg = JsonConvert.DeserializeObject<UpdateExtract>(info);
        msg.method = "UpdateExtract";
        
        WebSocketClient.inst.Send(JsonConvert.SerializeObject(msg));
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

    public void OnToast(string msg)
    {
        Toast.Inst.Open(msg);
    }

    public void OnRent(string res)
    {
        var ri = JsonConvert.DeserializeObject<RentInfo>(res);
        var msg = new BuyGoodsForRentMap()
        {
            map_id = ri.map_id,
            txid = ri.txid
        };
        WebSocketClient.inst.Send(JsonConvert.SerializeObject(msg));
    }
}
