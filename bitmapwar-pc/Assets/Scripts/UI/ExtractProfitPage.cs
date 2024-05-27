using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using FairyGUI;
using Newtonsoft.Json;
using UnityEngine;

public class ExtractProfitPage : Singleton<ExtractProfitPage>
{
    private GComponent view;
    private GTextInput amountText;
    public bool isOpen;
    public GList list;

    public void RefreshItem(ExtractsLog data)
    {
        var msg = new UpdateExtract()
        {
            method = "UpdateExtract",
            id = data.id,
            txid = data.txid
        };
        
        WebSocketClient.inst.Send(JsonConvert.SerializeObject(msg));
    }

    public void UpdateItem(UpdateExtractSuccess data)
    {
        Game.Inst.extractHistory = data.extracts;
        list.numItems = 0;
        for (int i = 0; i < Game.Inst.extractHistory.Length; i++)
        {
            var item = list.AddItemFromPool().asCom;
            MakeExtractHistoryItem(Game.Inst.extractHistory[i], item);
        }
        /*
        for (int i = 0; i < Game.Inst.extractHistory.Length; i++)
        {
            var log = Game.Inst.extractHistory[i];
            if (log.id == data.id && log.txid.Equals(data.txid))
            {
                log.status = data.status;
            }
        }
        */
    }

    private void MakeExtractHistoryItem(ExtractsLog data, GComponent item)
    {
        item.GetChildByPath("id").asTextField.text = data.id.ToString();
        item.GetChildByPath("txid").asTextField.text = data.txid;
        item.GetChildByPath("amount").asTextField.text = Consts.StrToDouble(data.amount);
        string strStatus = "Pending";
        if (data.status == 0)
        {
            strStatus = "Success";
        }

        item.GetChildByPath("n72").asCom.onClick.Add(() =>
        {
            RefreshItem(data);
        });
        

        item.GetChildByPath("status").asTextField.text = strStatus;
        item.GetChildByPath("create_time").asTextField.text = GridUtils.Inst.GetRealTimeStamp(data.create_time);
        item.GetChildByPath("operations").asTextField.text = "Refresh";
    }

    public void MakeRefreshPage()
    {
        var msg = new GetExtractPurchaseLog();
        WebSocketClient.inst.Send(JsonConvert.SerializeObject(msg));
    }
    
    public void Open()
    {
        isOpen = true;
        view = UIPackage.CreateObject("bitmapwar-web2-1920", "ExtractPage").asCom;

        amountText = view.GetChildByPath("n0.n86").asTextInput;
        
        GRoot.inst.AddChild(view);
        view.MakeFullScreen();

        var myAccount = Game.Inst.MyWallet;
        var user = Game.Inst.userOnBoard[myAccount];
        var prof = Consts.StrToDouble(user.profit);
        view.GetChildByPath("n0.n84").asTextField.text = prof;

        list = view.GetChildByPath("n0.n87").asList;
        for (int i = 0; i < Game.Inst.extractHistory.Length; i++)
        {
            var item = list.AddItemFromPool().asCom;
            MakeExtractHistoryItem(Game.Inst.extractHistory[i], item);
        }

        var confirmBtn = view.GetChildByPath("n0.n78").asCom;
        confirmBtn.onClick.Add(onConfirm);
        
        var cancelBtn= view.GetChildByPath("n0.n80").asCom;
        cancelBtn.onClick.Add(Close);
        
        view.GetChildByPath("n0.n65").asCom.onClick.Add(() =>
        {
            Close();
        });
    }

    public void onConfirm()
    {
        Debug.Log("Extract : " + amountText.text);
        if (string.IsNullOrEmpty(amountText.text))
        {
            return;
        }

        var myprof = (double)BigInteger.Parse(Game.Inst.userMySelf.profit) / (double)1e18;
        if (double.Parse(amountText.text) > myprof)
        {
            Toast.Inst.Open("Invalid Number!");
            return;
        }

        ProtoUtils.SendExtract(Game.Inst.MyWallet, amountText.text);
        
        //Close();
    }

    public void Close()
    {
        isOpen = false;
        GRoot.inst.RemoveChild(view);
    }
        
}
