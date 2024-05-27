using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileToast : MonoBehaviour
{
    public Text content;
    public static MobileToast inst;
    
    public void Open(string msg)
    {
        content.text = msg;
        gameObject.SetActive(true);
        StartCoroutine(Toasting());
    }

    public void Awake()
    {
        inst = this;
    }
    

    IEnumerator Toasting()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
