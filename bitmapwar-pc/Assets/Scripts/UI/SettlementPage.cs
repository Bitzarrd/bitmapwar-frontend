using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using FairyGUI;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class SettlementPage : Singleton<SettlementPage>
{
    private Dictionary<string, string> colorName = new Dictionary<string, string>()
    {
        {"red", "r"},
        {"blue", "b"},
        {"purple", "p"},
        {"green", "y"},
    };

    private Dictionary<string, int> colorController = new Dictionary<string, int>()
    {
        { "red", 0 },
        { "blue", 1 },
        { "purple", 2 },
        { "green", 3 },
    };
    
    private GComponent view;
    public void Open(Settlement result)
    {
        view = UIPackage.CreateObject("bitmapwar-web2-1920", "SettlementPage").asCom;
        GRoot.inst.AddChild(view);
        view.MakeFullScreen();

        var closeBtn = view.GetChildByPath("n0.n118").asCom;
        closeBtn.onClick.Add(Close);
        
        //aTextField.SetVar("jin", "500").SetVar("yin", "500").FlushVars();
        
        var resultText = view.GetChildByPath("n0.textResult").asTextField;

        var list = view.GetChildByPath("n0.n119").asList;

        for (int i = 0; i < result.statistics.Length; i++)
        {
            var data = result.statistics[i];
            var item = list.AddItemFromPool().asCom;
            
            item.GetController("c1").SetSelectedIndex(colorController[data.team]);
            item.GetChildByPath("r1").asTextField.text = data.land.ToString();
            item.GetChildByPath("r2").asTextField.text = data.virus.ToString();
            item.GetChildByPath("r3").asTextField.text = data.loss.ToString();
            
            /*
            var s = result.statistics[i];
            var cn = colorName[s.team];
            var landStr = "n0." + cn + "1";
            var SoldierStr = "n0." + cn + "2";
            var lostStr = "n0." + cn + "3";

            view.GetChildByPath(landStr).asTextField.text = s.land.ToString();
            view.GetChildByPath(SoldierStr).asTextField.text = s.virus.ToString();
            view.GetChildByPath(lostStr).asTextField.text = s.loss.ToString();
            */
        }
        
        Game.Inst.onUpdateUser(result.user);
        
        var me = result.my_statistics;
        

        resultText.SetVar("myland", me.land.ToString());
        resultText.SetVar("mylost", me.loss.ToString());
        resultText.SetVar("mys", me.virus.ToString());

        Debug.Log("Earning is : " + result.earning);
        var myprof = Consts.StrToDouble(result.earning, "0.####################");
        view.GetChildByPath("n0.n120").asTextField.text = myprof;
        
        resultText.FlushVars();
    }

    public void Close()
    {
        GRoot.inst.RemoveChild(view);
    }
}
