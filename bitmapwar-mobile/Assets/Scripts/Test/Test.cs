using System.Collections;
using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;

public class Test : MonoBehaviour
{
    public LoopListView2 list;
    // Start is called before the first frame update
    void Start()
    {
        list.InitListView(100, (list, i) =>
        {
            var item = list.NewListViewItem("Item");
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
            }

            return item;
        });
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
