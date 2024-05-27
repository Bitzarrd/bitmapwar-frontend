using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

public class ProfitPanel : MonoBehaviour
{
    public Button BtnClose;
    public LoopListView2 profitList;
    public static ProfitPanel inst;
    
    public UserHistoricalBenefit[] benefits;

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    public void OnData(GetUserHistoricalBenefitSuccess data)
    {
        benefits = data.benefits;
        profitList.InitListView(data.benefits.Length, OnGetItemByIndex);
    }
    
    public LoopListViewItem2 OnGetItemByIndex(LoopListView2 listview, int index)
    {
        LoopListViewItem2 item = listview.NewListViewItem("profitListItem");
        var profitItemCom = item.gameObject.GetComponent<ProfitListItem>();
        var bf = benefits[index];
        profitItemCom.FillInData(bf);

        return item;
    }

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (BtnClose != null)
        {
            BtnClose.onClick.AddListener(ClosePanel);
        }
        var msg = new GetUserHistoricalBenefit();
        WebSocketClient.inst.Send(JsonConvert.SerializeObject(msg));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
