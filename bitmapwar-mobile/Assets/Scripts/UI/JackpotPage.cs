using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;

public class JackpotPage : Singleton<JackpotPage>
{
    private bool isOpen;
    //private GComponent view;
    //private GComponent colorRes;
    
    public void Open(JackpotLightUp data)
    {
        //view = UIPackage.CreateObject("bitmapwar-web2-1920", "JackpotPage").asCom;
        //GRoot.inst.AddChild(view);
        //view.MakeFullScreen();

        //colorRes = view.GetChildByPath("n0.n121").asCom;

        //view.GetChildByPath("n0.n125").asTextField.text = data.land.ToString();
        string text = "";
        for (int i = 0; i < data.users.Length; i++)
        {
            text += Consts.GetConnectWalletLabel(data.users[i].taproot_address);
            if (i < data.users.Length - 1)
            {
                text += " , ";
            }
        }

        /*
        view.GetChildByPath("n0.n130").asTextField.text = text;
        view.GetChildByPath("n0.n126").asTextField.text = Consts.StrToDouble(data.jackpot);
        if (data.bw_user == null)
        {
            view.GetChildByPath("n0.n134").asTextField.text = "None";
            view.GetChildByPath("n0.n132").asTextField.text = "None";
        }
        else
        {
            view.GetChildByPath("n0.n134").asTextField.text = Consts.GetConnectWalletLabel(data.bw_user.taproot_address);
            view.GetChildByPath("n0.n132").asTextField.text = Consts.StrToDouble(data.bw_user.jackpot_bw);
            
        }

        for (int i = 0; i < MainPage.inst.colorDic.Count; i++)
        {
            var t = MainPage.inst.colorDic[i];
            if (t.Equals(data.team))
            {
                colorRes.GetController("c1").SetSelectedIndex(i);
            }
        }
        
        var closeBtn = view.GetChildByPath("n0.n131").asCom;
        closeBtn.onClick.Add((() =>
        {
            Close();
        }));
        
        view.GetChildByPath("n0.n123").asCom.onClick.Add(() =>
        {
            Share(data.jackpot);
        });
        */
    }

    public void Share(string amount)
    {
        string msg =
            $"In @BitmapWar, a lucky winner won the {amount} (BTC) Jackpot reward. Feeling lucky? Join and try your luck! #bitmap #bitcoin";
        var text = HttpUtility.UrlEncode(msg);
        string url = $"https://twitter.com/intent/tweet?text={text}";
        Application.OpenURL(url);
    }
    
    public void Close()
    {
        isOpen = false;
        //GRoot.inst.RemoveChild(view);
    }
}
