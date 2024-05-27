using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using Unity.VisualScripting;

public class LogPage : Singleton<LogPage>
{
    public bool isOpen = false;
    public GComponent view;

    public GList logList;
    
    public void Open()
    {
        isOpen = true;
        view = UIPackage.CreateObject("bitmapwar-web1-1920", "BattleLogPage").asCom;
        GRoot.inst.AddChild(view);
        view.MakeFullScreen();

        var tips = view.GetChildByPath("n0.n29").asTextField;

        logList = view.GetChildByPath("n0.n27").asList;
            
        logList.itemRenderer = RenderListItem;
        if (Game.Inst.battleLogs == null)
        {
            
        }
        else
        {
            if (Game.Inst.battleLogs.Count == 0)
            {
                if (Game.Inst.langIndex == 0)
                {
                    tips.text = "Nothing here yet.";
                }
                else tips.text = "无记录";
            }
            else
            {
                tips.visible = false;
            }
        }
        logList.SetVirtual();
        UpdateList();
        
        view.GetChildByPath("n0.n26").asCom.onClick.Add(Close);
    }

    public void UpdateList()
    {
        if (Game.Inst.battleLogs == null)
        {
            logList.numItems = 0;
            return;
        }
        logList.numItems = Game.Inst.battleLogs.Count;
    }
    
    void RenderListItem(int index, GObject obj)
    {
        var item = obj.asCom;
        var battleLog = Game.Inst.battleLogs[Game.Inst.battleLogs.Count - index - 1];
        var spath = "n" + battleLog.state;
        item.GetController("c1").SetSelectedIndex(battleLog.state);

        var comp = item.GetChildByPath(spath).asCom;
        comp.GetChildByPath("n1").asTextField.text = GridUtils.Inst.GetRealTimeHoursAndMinuts(battleLog.create_time);
        comp.GetChildByPath("n3").asTextField.text = battleLog.my_map_id.ToString();
        comp.GetChildByPath("n5").asTextField.text = battleLog.enemy_map_id.ToString();
        comp.GetChildByPath("n6").asTextField.text = battleLog.virus_loss.ToString();
    }
    public void Close()
    {
        isOpen = false;
        GRoot.inst.RemoveChild(view);
    }
}
