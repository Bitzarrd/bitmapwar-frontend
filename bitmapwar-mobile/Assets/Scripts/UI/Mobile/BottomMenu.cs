using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomMenu : MonoBehaviour
{
    public Button btnProfit;
    public Button btnMsg;
    public Button btnChat;
    public Button btnAcc;
    public Button btnAction;
    public Button btnSearch;

    public Button btnShowTotalRank;

    public GameObject ProfitPanel;
    public GameObject MsgPanel;
    public GameObject ChatPanel;
    public GameObject AccountPanel;
    public GameObject ActionPanel;
    public GameObject SearchPanel;
    public GameObject TotalRankingPanel;
    
    
    // Start is called before the first frame update
    void Start()
    {
        btnShowTotalRank.onClick.AddListener(() =>
        {
            var t = TotalRankingPanel.gameObject.activeSelf;
            TotalRankingPanel.gameObject.SetActive(!t);
        });
        btnProfit.onClick.AddListener(() =>
        {
            var t = ProfitPanel.gameObject.activeSelf;
            ProfitPanel.SetActive(!t);
        });
        
        btnMsg.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(Game.Inst.myPubKey))
            {
                Toast.Inst.Open("Please Login");
                return;
            }
            var t = MsgPanel.gameObject.activeSelf;
            MsgPanel.SetActive(!t);
        });
        btnChat.onClick.AddListener(() =>
        {
            var t = ChatPanel.gameObject.activeSelf;
            ChatPanel.SetActive(!t);
        });
        btnAcc.onClick.AddListener(() =>
        {
            var t = AccountPanel.gameObject.activeSelf;
            AccountPanel.SetActive(!t);
            if (t)
            {
                var uicom = AccountPanel.GetComponent<UserInfoPanel>();
                uicom.RefreshUserInfo();
            }
        });
        btnAction.onClick.AddListener(() =>
        {
            /*
            var t = ActionPanel.gameObject.activeSelf;
            ActionPanel.SetActive(!t);
            */
        });
        btnSearch.onClick.AddListener(() =>
        {
            SearchPanel.SetActive(true);
        });
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
