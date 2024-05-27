using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

public class TotalProfitPanel : MonoBehaviour
{
    public bool isOpen = false;
    public List<string> tabList = new List<string>()
    {
        "profit",
        "points",
        "jackpot",
        "jackpot_bw",
    };
    
    public int radioIndex = 0;
    public List<GameObject> RadioBtns;
    public List<Button> tabBtns;

    public Button CloseBtn;

    public LoopListView2 list;

    private GetLeaderBoardSuccess resultData;

    public void OnData(GetLeaderBoardSuccess data)
    {
        resultData = data;
        if (list.ItemTotalCount > 0)
        {
            list.SetListItemCount(data.users.Length);
            list.RefreshAllShownItem();
            return;
        }
        list.InitListView(data.users.Length, (view, i) =>
        {
            int rankingIndex = i + 1;
            string name = "Ranking";
            if (rankingIndex < 4)
            {
                name += rankingIndex.ToString();
            }
            else
            {
                name = "Ranking4";
            }
            
            var item = view.NewListViewItem(name);
            var leaderItem = item.GetComponent<LeaderRankingItem>();

            string rawData = Consts.StrToDouble(resultData.users[i].total_profit);
            if (radioIndex == 1)
            {
                rawData = Consts.IntToAbrevMode(resultData.users[i].points);
            }
            else if (radioIndex == 2)
            {
                rawData = Consts.StrToDouble(resultData.users[i].jackpot);
            }
            else if (radioIndex == 3)
            {
                rawData = Consts.StrToDouble(resultData.users[i].jackpot_bw);
            }
            leaderItem.FillData(rankingIndex.ToString(), Consts.GetConnectWalletLabel(data.users[i].taproot_address), rawData);

            return item;
        });
    }
    public void onChooseTab(int index)
    {
        radioIndex = index;
        RefreshRadioBtns();
        Debug.Log("Choose Tab: " + index);
        
        var msg = new GetLeaderBoard()
        {
            method = "GetLeaderBoard",
            tab = tabList[index],
        };
        WebSocketClient.inst.Send(JsonConvert.SerializeObject(msg));
    }

    public void RefreshRadioBtns()
    {
        for (int i = 0; i < RadioBtns.Count; i++)
        {
            var b = RadioBtns[i];
            b.transform.GetChild(1).gameObject.SetActive(false);
        }
        
        RadioBtns[radioIndex].transform.GetChild(1).gameObject.SetActive(true);

        for (int i = 0; i < tabBtns.Count; i++)
        {
            var b = tabBtns[i];
            int _index = i;
            b.onClick.AddListener(() => onChooseTab(_index));
        }
    }

    public static TotalProfitPanel inst;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        radioIndex = 0;
        RefreshRadioBtns();
        onChooseTab(0);
        CloseBtn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            isOpen = false;
        });
        isOpen = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
