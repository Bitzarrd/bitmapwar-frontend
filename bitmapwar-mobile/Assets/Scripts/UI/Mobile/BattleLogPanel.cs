using System;
using System.Collections;
using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

public class BattleLogPanel : MonoBehaviour
{
    public Button BtnClose;
    public LoopListView2 messageList;

    public static BattleLogPanel inst;

    public bool isInited = false;

    private void Awake()
    {
        inst = this;
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (BtnClose != null)
        {
            BtnClose.onClick.AddListener(ClosePanel);
        }

        var count = Game.Inst.battleLogs.Count;
        messageList.InitListView(count, GetItemByIndex);
        isInited = true;
    }

    private void OnEnable()
    {
        UpdateRecord();
    }

    public void UpdateRecord()
    {
        if (!isInited)
        {
            return;
        }

        if (Game.Inst.battleLogs == null)
        {
            return;
        }
        var count = Game.Inst.battleLogs.Count;
        messageList.SetListItemCount(count);
        messageList.RefreshAllShownItem();
    }

    private LoopListViewItem2 GetItemByIndex(LoopListView2 view, int index)
    {
        LoopListViewItem2 item = view.NewListViewItem("BattleLogItem");
        var battleLogView = item.GetComponent<BattleLogItem>();
        battleLogView.InitData(Game.Inst.battleLogs[Game.Inst.battleLogs.Count - index - 1]);
        
        return item;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
