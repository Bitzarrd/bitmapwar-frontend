using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFightPage: Singleton<GameFightPage> 
{
    //private GComponent view;
    public void Open()
    {
        //view = UIPackage.CreateObject("bitmapwar-web1-1920", "GameStart").asCom;
        //GRoot.inst.AddChild(view);
        //view.MakeFullScreen();
        
        //MainPage.inst.StartCoroutine(Toasting());
    }

    IEnumerator Toasting()
    {
        yield return new WaitForSeconds(1f);
        //GRoot.inst.RemoveChild(view);
    }
}
