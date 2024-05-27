using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class LoadingPage : Singleton<LoadingPage>
{
    public bool isOpen = false;
    public GComponent view;

    public void Open()
    {
        if (isOpen)
        {
            return;
        }
        isOpen = true;
        view = UIPackage.CreateObject("bitmapwar-web1-1920", "LoadingPage").asCom;
        GRoot.inst.AddChild(view);
        view.MakeFullScreen();
    }

    public void Close()
    {
        isOpen = false;
        GRoot.inst.RemoveChild(view);
    }
}
