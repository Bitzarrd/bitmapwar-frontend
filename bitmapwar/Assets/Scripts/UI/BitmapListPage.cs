using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FairyGUI;
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
    private GComponent view;
    private int selMapId;
    private GComponent comboComp;
    private string listUrl = "https://global.bitmap.game/service/open/bitmap/list";

    private Root TileListData;

    private GList tileList;

    public string CurrentChoose;

    private GTextField tChooseTile;

    private string[] myTileList;

    public bool isOpen;

    public void OnChooseTileItem(int pos)
    {
        //Debug.Log("You Choose: " + pos);
        var tileInfo = myTileList[pos];
        CurrentChoose = tileInfo;
        tChooseTile.text = tileInfo;
        CloseDropDown();
    }

    public void CloseDropDown()
    {
        view.GetChild("n0").asCom.GetController("c1").SetSelectedIndex(0);
    }

    public void FillTilesList(string[] data, RentalBitmap[] rentals, RentalBitmap[] occupide)
    {
        tileList.numItems = 0;
        var list = data;
        var totalTiles = new List<string>();
        int btnIndex = 0;
        for (int i = 0; i < rentals.Length; i++)
        {
            var l = rentals[i].bitmap_id.ToString();
            totalTiles.Add(l);
            var item = tileList.AddItemFromPool().asCom;
            var tileName = l;
            var tileId = long.Parse(tileName);
            if (Game.Inst.playersOnBoard.ContainsKey(tileId))
            {
                tileName += " Soldier: ("  + Game.Inst.playersOnBoard[tileId].virus + ")";
            }
            else
            {
                tileName += " Soldier:(0)";
            }

            tileName += " (" + Consts.GetDaysByStamp(rentals[i].timeout) + " ) ";
            item.GetChild("title").asTextField.text = tileName;
            int t = btnIndex++;
            item.onClick.Add(() =>
            {
                OnChooseTileItem(t);
            });
        }

        for (int i = 0; i < occupide.Length; i++)
        {
            var l = occupide[i].bitmap_id.ToString();
            totalTiles.Add(l);
            var item = tileList.AddItemFromPool().asCom;
            var tileName = l;
            var tileId = long.Parse(tileName);
            /*
            if (Game.Inst.playersOnBoard.ContainsKey(tileId))
            {
                tileName += " Soldier: ("  + Game.Inst.playersOnBoard[tileId].virus + ")";
            }
            else
            {
                tileName += " Soldier:(0)";
            }
            */

            tileName += " (" + Consts.GetDaysByStamp(occupide[i].timeout) + " ) ";
            item.GetChild("title").asTextField.text = tileName;
            int t = btnIndex++;
            item.onClick.Add(() =>
            {
                //OnChooseTileItem(t);
            });
        }
        for (int i = 0; i < list.Length; i++) 
        {
            var l = list[i];
            totalTiles.Add(l);
            var item = tileList.AddItemFromPool().asCom;
            var tileName = l;
            var tileId = long.Parse(tileName);
            if (Game.Inst.playersOnBoard.ContainsKey(tileId))
            {
                tileName += " Soldier: ("  + Game.Inst.playersOnBoard[tileId].virus + ")";
            }
            else
            {
                tileName += " Soldier:(0)";
            }
            item.GetChild("title").asTextField.text = tileName;
            int t = btnIndex++;
            item.onClick.Add(() =>
            {
                OnChooseTileItem(t);
            });
        }

        myTileList = totalTiles.ToArray();
    }
    
    public void Open()
    {
        isOpen = true;
        var param = new Dictionary<string, string>();
        param.Add("address", Game.Inst.MyWallet);
        
        ProtoUtils.SendLoadMap(Game.Inst.MyWallet);
        
        /*
        HttpUtils.inst.SendGetRequest(listUrl, param, (res) =>
        {
            var r = JsonConvert.DeserializeObject<Root>(res);
            FillTilesList(r);
        });
        */
        
        view = UIPackage.CreateObject("bitmapwar-web2-1920", "BitmapListPage").asCom;
        GRoot.inst.AddChild(view);
        view.MakeFullScreen();

        var confirmBtn = view.GetChildByPath("n0.n7").asCom;
        confirmBtn.onClick.Add(Confirm);

        var cancelBtn = view.GetChildByPath("n0.n9").asCom;
        cancelBtn.onClick.Add(Close);

        var dropDownBtn = view.GetChildByPath("n0.n12").asCom;
        dropDownBtn.onClick.Add(() =>
        {
            view.GetChild("n0").asCom.GetController("c1").SetSelectedIndex(1);
        });

        tileList = view.GetChildByPath("n0.n14.n15").asList;

        tChooseTile = view.GetChildByPath("n0.n12.n13").asTextField;

        /*
        comboList = view.GetChildByPath("n0.n11").asComboBox;
        var t = new List<string>();

        foreach(var k in MainPage.inst.mySoldiers.Keys)
        {
            t.Add(k.ToString());
        }
        comboList.items = t.ToArray();
        comboList.values = t.ToArray();

        comboList.onChanged.Add(OnChoose);
        */

    }
    
    private void OnChoose(EventContext e)
    {

    }

    public void Close()
    {
        isOpen = false;
        GRoot.inst.RemoveChild(view);
    }

    public void Confirm()
    {
        Close();
        MainPage.inst.LerpCameraTo(CurrentChoose);
        Game.Inst.currentSelect = GridUtils.Inst.GetCoordByTileId(int.Parse(CurrentChoose));
        MainPage.inst.RefreshTileInfo();
    }
}