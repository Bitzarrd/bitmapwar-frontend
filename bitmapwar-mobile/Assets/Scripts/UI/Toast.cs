using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toast : Singleton<Toast> 
{
    public void Open(string msg = "")
    {
        /*
        view = UIPackage.CreateObject("bitmapwar-error-tips-1920", "error_tips").asCom;
        GRoot.inst.AddChild(view);
        view.MakeFullScreen();
        */
        if (!string.IsNullOrEmpty(msg))
        {
            //view.GetChildByPath("n0.n3").asTextField.text = LangMgr.inst.GetString(msg);
        }
        
        //MainPage.inst.StartCoroutine(Toasting());
        if (MobileToast.inst == null)
        {
            MobileToast.inst = GameObject.FindObjectOfType<MobileToast>();
        }
        MobileToast.inst.Open(msg);
    }

    IEnumerator Toasting()
    {
        yield return new WaitForSeconds(2f);
    }
}
