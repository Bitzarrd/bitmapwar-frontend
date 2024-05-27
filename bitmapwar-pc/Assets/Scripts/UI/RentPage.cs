using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class RentPage : Singleton<RentPage>
{
    private Dictionary<string, Dictionary<string, float>> price = new Dictionary<string, Dictionary<string, float>>()
    {
        {
            "7", 
            new Dictionary<string, float>() { { "energy", 500.0f }, { "btc", 0.0004f }, { "profit", 0.0004f } }
        },
        {
            "15", new Dictionary<string, float>() { {"energy",800.0f}, {"btc" , 0.0006f }, {"profit", 0.0006f} }
        },
        {
            "30",
            new Dictionary<string, float>() { { "energy", 1200.0f }, { "btc", 0.001f }, { "profit", 0.001f } }
        }
    };
    
    private GComponent view;
    private GComboBox paymethod;
    private GComboBox RentLength;

    private GTextInput bitmapInp;
    private GTextField mStatus;
    private GTextField mPrice;

    public void ChangePrice()
    {
        Debug.Log(RentLength.value + " Type: " + paymethod.value);
        if (paymethod.value.Equals("energy"))
        {
            mPrice.text = price[RentLength.value][paymethod.value].ToString() + "/" + Game.Inst.userMySelf.energy;
        }
        else
        {
            mPrice.text = price[RentLength.value][paymethod.value].ToString();
        }
    }

    private void OnConfirm()
    {
        if (paymethod.value.Equals("btc"))
        {
            var id = int.Parse(bitmapInp.text);
            var day = int.Parse(RentLength.value);
            
#if !UNITY_EDITOR && UNITY_WEBGL
            if(price[RentLength.value]["btc"] > Game.Inst.btcRemain)
            {
                Toast.Inst.Open("Wallet Error: Money is not enough!");
            }
            JSBridge.inst.JSRent(id, day);
#endif
            string url = $"https://dev.bitmapwar.com/login?code={Game.Inst.PCCode}&mapId={bitmapInp.text}&days={RentLength.value}";
            Application.OpenURL(url);
        }
        else
        {
            var rb = new RentBitmap()
            {
                map_id = int.Parse(bitmapInp.text),
                type = paymethod.value,
                day = int.Parse(RentLength.value)
            };

            var msg = JsonConvert.SerializeObject(rb);
            WebSocketClient.inst.Send(msg);

        }
        OnClose();
    }

    private void OnCancel()
    {
        OnClose();
    }

    void OnClose()
    {
        GRoot.inst.RemoveChild(view);
    }

    public void UpdateTileStatus(QueryBitmapAvailableForRentResponse data)
    {
        mStatus.visible = true;
        if (data.available)
        {
            mStatus.color = new Color(0x0, 0xff, 0x0);
            if (Game.Inst.langIndex == 0)
            {
                mStatus.text = "*Free";
            }
            else
            {
                mStatus.text = "*闲置";
            }
        }
        else
        {
            mStatus.color = new Color(0xff, 0x00, 0x00);
            if (Game.Inst.langIndex == 0)
            {
                mStatus.text = "Occupied";
            }
            else
            {
                mStatus.text = "*占用";
            }
        }
    }

    void QueryStatus()
    {
        var val = int.Parse(bitmapInp.text);
        if (val < 0 || val >= GridUtils.Inst.gridSizeX * GridUtils.Inst.gridSizeY)
        {
            return;
        }

        var msg = new QueryBitmapAvailableForRent()
        {
            map_id = val
        };
        WebSocketClient.inst.Send(JsonConvert.SerializeObject(msg));
    }

    public void SetMapID(string mapid)
    {
        bitmapInp.text = mapid;
    }
    
    
    public void Open()
    {
        #if UNITY_WEBGL
        JSBridge.inst.GetBtcBalance();
        #endif
        view = UIPackage.CreateObject("bitmapwar-web7-1920", "RentPage").asCom;
        GRoot.inst.AddChild(view);
        view.MakeFullScreen();
        InitView();
        ChangePrice();
    }

    public void InitView()
    {
        RentLength = view.GetChildByPath("n0.n34").asComboBox;
        paymethod = view.GetChildByPath("n0.n36").asComboBox;
        
        RentLength.onChanged.Add(ChangePrice);
        paymethod.onChanged.Add(ChangePrice);

        bitmapInp = view.GetChildByPath("n0.n33").asTextInput;
        view.GetChildByPath("n0.n13").asCom.onClick.Add(OnConfirm);
        view.GetChildByPath("n0.n15").asCom.onClick.Add(OnCancel);

        mPrice = view.GetChildByPath("n0.n23").asTextField;

        mStatus = view.GetChildByPath("n0.n27").asTextField;
        
        view.GetChildByPath("n0.n10").asCom.onClick.Add(QueryStatus);
    }
}
