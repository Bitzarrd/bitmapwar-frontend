using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class HttpUtils : MonoBehaviour
{
    public static HttpUtils inst;
    private void Awake()
    {
        inst = this;
    }

    public void SendGetRequest(string url, Dictionary<string, string> p, Action<string> cb)
    {
        string fullUrl = url;
        if (p.Count > 0) fullUrl += "?";
        var keys = p.Keys.ToArray();
        for(int i = 0; i < keys.Length; i++)
        {
            var pp = keys[i];
            fullUrl += pp + "=" + p[pp];
            if (i < keys.Length - 1)
            {
                fullUrl += "&";
            }
        }
        Debug.Log("Make Request : " + fullUrl);

        UnityWebRequest request = UnityWebRequest.Get(fullUrl);

        StartCoroutine(SendRequest(request, cb));
    }

    private IEnumerator SendRequest(UnityWebRequest request, Action<string> cb)
    {
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error sending GET request: " + request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            cb.Invoke(request.downloadHandler.text);
        }
    }
}
