using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class PlaceSoldierPage : Singleton<PlaceSoldierPage>
{
    private GComponent view;
    private int _mapID;
    private int _cout;
    private string _color;

    public void Open(int mapId, int solider, string color, int mode = 0)
    {
        _mapID = mapId;
        _cout = solider;
        _color = color;
        
        view = UIPackage.CreateObject("bitmapwar-web2-1920", "PlaceSoliderPage").asCom;
        GRoot.inst.AddChild(view);
        view.MakeFullScreen();
        var info = view.GetChildByPath("n0.n41").asTextField;
        
        var confirmBtn = view.GetChildByPath("n0.n37").asCom;
        if (mode == 0)
        {
            confirmBtn.onClick.Add(OnConfirm);
        }
        else
        {
            confirmBtn.onClick.Add(OnConfirmBatch);
            if (Game.Inst.langIndex == 1)
            {
                info.text = "将批量将士兵放在所有Bitmap中，请确认。";
            }
            else
            {
                info.text = "Doing this will put the soldiers in all bitmaps, please confirm.";
            }
        }

        var cancelBtn = view.GetChildByPath("n0.n39").asCom;
        cancelBtn.onClick.Add(Close);
    }

    public void OnConfirm()
    {
        ProtoUtils.SendPlaceSoldier(_mapID, _cout, _color);
        Close();
    }
    public void OnConfirmBatch()
    {
        ProtoUtils.SendBatchPlace(_cout, _color);
        Close();
    }
    public void Close()
    {
        GRoot.inst.RemoveChild(view);
    }
    
}
