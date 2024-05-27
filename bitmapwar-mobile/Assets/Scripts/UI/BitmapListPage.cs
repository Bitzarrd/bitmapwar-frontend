using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class info
{
    public string address;
    public string inscription_id;
    public string bitmap_id;
}

[System.Serializable]
public class Info
{
    public info info;
}

[System.Serializable]
public class Data
{
    public int totalCount;
    public info[] items;
}

[System.Serializable]
public class Root
{
    public int code;
    public string msg;
    public Data data;
}

public class BitmapListPage : Singleton<BitmapListPage>
{
    private int selMapId;
    private string listUrl = "https://global.bitmap.game/service/open/bitmap/list";

    private Root TileListData;

    public string CurrentChoose;

    private string[] myTileList;

    public bool isOpen;

    public void OnChooseTileItem(int pos)
    {
    }

    public void CloseDropDown()
    {
    }

    public void FillTilesList(string[] data)
    {
    }
    
    public void Open()
    {
        isOpen = true;
        var param = new Dictionary<string, string>();
        param.Add("address", Game.Inst.MyWallet);
        
        ProtoUtils.SendLoadMap(Game.Inst.MyWallet);
    }
    

    public void Close()
    {
    }

    public void Confirm()
    {
        Close();
        MainPage.inst.LerpCameraTo(CurrentChoose);
        Game.Inst.currentSelect = GridUtils.Inst.GetCoordByTileId(int.Parse(CurrentChoose));
        MainPage.inst.RefreshTileInfo();
    }
}