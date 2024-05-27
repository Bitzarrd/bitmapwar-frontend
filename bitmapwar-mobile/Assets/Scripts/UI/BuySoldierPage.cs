using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class BuySoldierPage : Singleton<BuySoldierPage>
{
    private int CurrentBuyNum = 1;

    public bool isOpen = false;

    public void FreshPage()
    {
        //tBuyCount.text = CurrentBuyNum.ToString();
    }

    //TODO: Jerry Filling the data
    public void MakeBuySoldierHistoryItem(PurchaseLog data)
    {
        
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
    }

    public void RefreshPurchaseLog()
    {
              
    }
    
    public void Open()
    {
       isOpen = true;

       
       FreshPage();
    }

    public void onConfirm()
    {
        /*
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
        */
        //Close();
    }
    
    public void Close()
    {
        isOpen = false;
    }
}
