using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using Newtonsoft.Json;

public class BuySoldierPage : Singleton<BuySoldierPage>
{
    private int CurrentBuyNum = 1;

    private GComponent view;
    private GList list;
    
    private GComponent addBtn;
    private GComponent minusBtn;
    private GTextField tBuyCount;
    private GTextInput inpAmount;

    private GComboBox methodComb;
    public bool isOpen = false;

    public void FreshPage()
    {
        //tBuyCount.text = CurrentBuyNum.ToString();
    }

    public void MakeBuySoldierHistoryItem(PurchaseLog data, GComponent item)
    {
        item.GetChildByPath("id").asTextField.text = data.id.ToString();
        item.GetChildByPath("txid").asTextField.text = data.txid;
        item.GetChildByPath("fee").asTextField.text = Consts.StrToDouble(data.fee);
        item.GetChildByPath("soldier").asTextField.text = data.virus.ToString();
        item.GetChildByPath("create_time").asTextField.text = GridUtils.Inst.GetRealTimeStamp(data.create_time);
        string strStatus = "Pending";
        if (data.status == 1)
        {
            if (Game.Inst.langIndex != 0)
            {
                strStatus = "成功";
            }
            else
            {
                strStatus = "Success";
            }
        }
        else if (data.status == 2)
        {
            strStatus = "Failed";
        }

        item.GetChildByPath("status").asTextField.text = strStatus;
    }

    public void RefreshPurchaseLog()
    {
        list = view.GetChildByPath("n0.n166").asList;
        for (int i = 0; i < Game.Inst.purchaseHistory.Length; i++)
        {
            var item = list.AddItemFromPool().asCom;
            var data = Game.Inst.purchaseHistory[i];
            MakeBuySoldierHistoryItem(data, item);
        }       
    }
    
    public void Open()
    {
       view = UIPackage.CreateObject("bitmapwar-web2-1920", "PurchasePage").asCom;
       GRoot.inst.AddChild(view);
       view.MakeFullScreen();
       isOpen = true;

       methodComb = view.GetChildByPath("n0.n170").asComboBox;

       inpAmount = view.GetChildByPath("n0.n163.n171").asTextInput;
       list = view.GetChildByPath("n0.n166").asList;
       for (int i = 0; i < Game.Inst.purchaseHistory.Length; i++)
       {
           var item = list.AddItemFromPool().asCom;
           var data = Game.Inst.purchaseHistory[i];
           MakeBuySoldierHistoryItem(data, item);
       }

       var price = view.GetChildByPath("n0.n165").asTextField;
       Debug.Log("Solder Price is " + Game.Inst.soldierPrice);
       price.SetVar("t", Game.Inst.soldierPrice).FlushVars();

       /*
       addBtn = view.GetChildByPath("n0.n163.n167").asCom;
       minusBtn = view.GetChildByPath("n0.n163.n169").asCom;
       tBuyCount = view.GetChildByPath("n0.n163.n166").asTextField;
       
       addBtn.onClick.Add((() =>
       {
           CurrentBuyNum++;
           FreshPage();
       }));
       
       minusBtn.onClick.Add((() =>
       {
           if (CurrentBuyNum > 0)
           {
               CurrentBuyNum--;
               FreshPage();
           }
       }));
       */

       view.GetChildByPath("n0.n144").asCom.onClick.Add(() =>
       {
           Close();
       });

       view.GetChildByPath("n0.n159").asCom.onClick.Add((() =>
       {
           onConfirm();
       }));
       
       view.GetChildByPath("n0.n161").asCom.onClick.Add((() =>
       {
           Close();
       }));
       
       FreshPage();
    }

    public void onConfirm()
    {
        var longNum = long.Parse(inpAmount.text);
        if (longNum > int.MaxValue)
        {
            return;
        }
        CurrentBuyNum = int.Parse(inpAmount.text);
        if (CurrentBuyNum <= 0)
        {
            Toast.Inst.Open("Invalid Number");
            return;
        }
        
        Debug.Log("Buy:" + CurrentBuyNum);
        if (methodComb.value.Equals("0"))
        {
            Debug.Log("Pay With Profit");
            var msg = new PurchaseVirusWithProfit();
            msg.amount = CurrentBuyNum;
            WebSocketClient.inst.Send(JsonConvert.SerializeObject(msg));
            return;
        }
        #if UNITY_WEBGL && !UNITY_EDITOR
            JSBridge.inst.BuySoldier(CurrentBuyNum);
        #endif
        //Close();
    }
    
    public void Close()
    {
        isOpen = false;
        GRoot.inst.RemoveChild(view);
    }
}
