using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class Toast : Singleton<Toast> 
{
    private GComponent view;
    public void Open(string msg = "")
    {
        view = UIPackage.CreateObject("bitmapwar-error-tips-1920", "error_tips").asCom;
        GRoot.inst.AddChild(view);
        view.MakeFullScreen();
        if (!string.IsNullOrEmpty(msg))
        {
            view.GetChildByPath("n0.n3").asTextField.text = LangMgr.inst.GetString(msg);
        }
        else
        {
            view.GetChildByPath("n0.n3").asTextField.text = msg;
        }
        
        MainPage.inst.StartCoroutine(Toasting());
    }

    IEnumerator Toasting()
    {
        yield return new WaitForSeconds(2f);
        GRoot.inst.RemoveChild(view);
    }
}
