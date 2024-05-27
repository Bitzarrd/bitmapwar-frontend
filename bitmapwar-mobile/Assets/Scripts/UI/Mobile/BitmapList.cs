using System.Collections;
using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

public class BitmapList : MonoBehaviour
{
    public LoopListView2 list;
    public Button CloseBtn;
    public bool isCreate = false;
    
    // Start is called before the first frame update
    void Start()
    {
        if (isCreate)
        {
            list.SetListItemCount(Game.Inst.myLandList.Length);
            list.RefreshAllShownItem();
            return;
        }
        CloseBtn.onClick.AddListener(()=> gameObject.SetActive(false));
        list.InitListView(Game.Inst.myLandList.Length, (view, i) =>
        {
            LoopListViewItem2 item = view.NewListViewItem("maplistitem");
            var mapView = item.GetComponent<MapListItem>();
            mapView.InitData(Game.Inst.myLandList[i]);
    
            return item;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
