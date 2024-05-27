using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ProfitHistoryPage : Singleton<ProfitHistoryPage>
{
    public bool isOpen = false;

    private void MakeRequest()
    {
        var msg = new GetUserHistoricalBenefit();
        WebSocketClient.inst.Send(JsonConvert.SerializeObject(msg));
    }

    public void Open()
    {
        isOpen = true;
        MakeRequest();
        
        /*
        view = UIPackage.CreateObject("bitmapwar-web5-1920", "GetProfitPage").asCom;
        GRoot.inst.AddChild(view);
        view.MakeFullScreen();
        list = view.GetChildByPath("n0.n43").asList;
        view.GetChildByPath("n0.n12").asCom.onClick.Add(Close);
        */
    }

    /*
    private void MakeItem(GComponent item, UserHistoricalBenefit data)
    {
        if(data.team.Equals("red")) item.GetController("c1").SetSelectedIndex(0);
        if(data.team.Equals("blue")) item.GetController("c1").SetSelectedIndex(2);
        if(data.team.Equals("purple")) item.GetController("c1").SetSelectedIndex(3);
        if(data.team.Equals("green")) item.GetController("c1").SetSelectedIndex(1);

        item.GetChildByPath("n14").asTextField.text = GridUtils.Inst.GetRealTimeDateAndTime(data.create_time);
        item.GetChildByPath("n17").asTextField.text = Consts.StrToDouble(data.profit);
        item.GetChildByPath("n18").asTextField.text = data.init_virus.ToString();
    }
    */

    public void OnData(GetUserHistoricalBenefitSuccess data)
    {
        for (int i = 0; i < data.benefits.Length; i++)
        {
            /*
            var record = data.benefits[i];
            var item = list.AddItemFromPool().asCom;
            MakeItem(item, record);
            */
        }
    }

    public void Close()
    {
        isOpen = false;
        //GRoot.inst.RemoveChild(view);
    }
}
