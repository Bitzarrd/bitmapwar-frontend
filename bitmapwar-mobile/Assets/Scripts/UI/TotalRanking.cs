using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class TotalRanking : Singleton<TotalRanking>
{
    public bool isOpen = false;

    private bool isOnRanking = false;

    private User[] profitUsers;
    private User[] pointsUsers;
    private User[] jackpotUsers;
    private User[] jackpotBwUsers;

    private int currentTab;

    public void Open()
    {
        isOpen = true;
        isOnRanking = false;
        /*
        view = UIPackage.CreateObject("bitmapwar-web2-1920", "LeaderRankingPage").asCom;
        GRoot.inst.AddChild(view);
        view.MakeFullScreen();
        */
        DoRequest("profit");
        /*
        view.GetChildByPath("n0.n17").asCom.onClick.Add(Close);
        rankinglist = view.GetChildByPath("n0.n22").asList;

        mySelfRank = view.GetChildByPath("n0.n24").asTextField;
        
        view.GetChildByPath("n0.n18").asButton.onClick.Add(()=> { DoRequest("profit");});
        view.GetChildByPath("n0.n19").asButton.onClick.Add(()=> { DoRequest("points");});
        view.GetChildByPath("n0.n20").asButton.onClick.Add(()=> { DoRequest("jackpot");});
        view.GetChildByPath("n0.n21").asButton.onClick.Add(()=> { DoRequest("jackpot_bw");});
        */
    }

    public void OnResponse(GetLeaderBoardSuccess res)
    {
        /*
        rankinglist.numItems = 0;
        for (int i = 0; i < res.users.Length; i++)
        {
            var p = res.users[i];
            if (Game.Inst.IsUserMyself(p))
            {
                isOnRanking = true;
            }
            var item = rankinglist.AddItemFromPool().asCom;
            AddItem(i, p, item, res.tab);
        }

        if (isOnRanking)
        {
            mySelfRank.text = res.my_self_rank.ToString();
        }
        else
        {
            mySelfRank.text = "--";
        }
        */
    }

    /*
    private void AddItem(int i, User u, GComponent item, string tab)
    {
        string p3 = u.total_profit;
        if (tab.Equals("points"))
        {
            p3 = Consts.IntToAbrevMode(u.points);
        }
        else if (tab.Equals("jackpot"))
        {
            p3 = u.jackpot;
        }
        else if (tab.Equals("jackpot_bw"))
        {
            p3 = u.jackpot_bw;
        }

        int ri = i + 1;
        if (i >= 0 && i <= 2)
        {
            item.GetController("c1").SetSelectedIndex(i);
            item.GetChild("owner_" + ri).asTextField.text = Consts.GetConnectWalletLabel(u.taproot_address);
            if (tab.Equals("points"))
            {
                item.GetChild("profit_" + ri).asTextField.text = p3;
            }
            else
            {
                item.GetChild("profit_" + ri).asTextField.text = Consts.StrToDouble(p3);    
            }
        }
        else if(i >= 3)
        {
            item.GetController("c1").SetSelectedIndex(3);
            item.GetChild("owner_x").asTextField.text = Consts.GetConnectWalletLabel(u.taproot_address);
            if (tab.Equals("points"))
            {
                item.GetChild("profit_x").asTextField.text = p3;
            }
            else
            {
                item.GetChild("profit_x").asTextField.text = Consts.StrToDouble(p3);
            }
            item.GetChild("rank").asTextField.text = (i + 1).ToString();
        }
    }
    */
    
    private void DoRequest(string tab)
    {
        var msg = new GetLeaderBoard()
        {
            method = "GetLeaderBoard",
            tab = tab,
        };
        
        WebSocketClient.inst.Send(JsonConvert.SerializeObject(msg));
    }
    
    public void Close()
    {
        isOpen = false;
    }
}
