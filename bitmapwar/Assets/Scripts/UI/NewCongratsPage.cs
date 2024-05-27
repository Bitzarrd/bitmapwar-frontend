using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public class NewCongratsPage : Singleton<NewCongratsPage>
{
    private GComponent view;
    private GTextField winnerText;
    private GTextField jackpotText;
    private GTextField winTimeText;

    public bool isOpen = false;

    public void Open(JackpotLightUpWithoutPlayers data)
    {
        isOpen = true;
        view = UIPackage.CreateObject("bitmapwar-web2-1920", "NewCongratsPage").asCom;
        GRoot.inst.AddChild(view);

        winnerText = view.GetChildByPath("n0.n130").asTextField;
        jackpotText = view.GetChildByPath("n0.n126").asTextField;
        winTimeText = view.GetChildByPath("n0.n137").asTextField;

        winnerText.text = Consts.GetConnectWalletLabel(data.user.taproot_address);
        jackpotText.text = Consts.StrToDouble(data.amount, "0.##########");
        winTimeText.text = Consts.GetYMDHMSFromTime(data.create_now);
        
        view.GetChildByPath("n0.n131").asCom.onClick.Add(Close);
    }

    public void Close()
    {
        isOpen = false;
        GRoot.inst.RemoveChild(view);
    }
}
