using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using UnityEngine;

public class ExtractProfitPage : Singleton<ExtractProfitPage>
{
    public bool isOpen;

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
        for (int i = 0; i < Game.Inst.extractHistory.Length; i++)
        {
            //var item = list.AddItemFromPool().asCom;
            //MakeExtractHistoryItem(Game.Inst.extractHistory[i], item);
        }
    }

    private void MakeExtractHistoryItem(ExtractsLog data)
    {
        string strStatus = "Pending";
        if (data.status == 0)
        {
            strStatus = "Success";
        }
    }

    public void MakeRefreshPage()
    {
        var msg = new GetExtractPurchaseLog();
        WebSocketClient.inst.Send(JsonConvert.SerializeObject(msg));
    }
    
    public void Open()
    {
        isOpen = true;

        var myAccount = Game.Inst.MyWallet;
        var user = Game.Inst.userOnBoard[myAccount];
        var prof = Consts.StrToDouble(user.profit);
    }

    public void onConfirm()
    {
        /*
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
        */
        
        //Close();
    }

    public void Close()
    {
        isOpen = false;
    }
        
}
