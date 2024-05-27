using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MyComponent : MonoBehaviour
{
    public MyData myData;
    
    private MyData otherData;
    public PopupManager2 popupManager2;
    
    void Start()
    {
        // this.otherData = ScriptableObject.CreateInstance<MyData>();
        // this.otherData.myString = "other data...";
        // this.otherData.myInt = 888;
        //
        //
        // Print();
        // var myBnt = transform.GetComponentInChildren<Button>();
        // myBnt.onClick.AddListener(() =>
        // {
        //     Debug.Log("my data。。。。");
        //     myData.myString = "Hi,Children boy!";
        //     myData.myInt = 168;
        //     this.otherData.myString = "other data...1";
        //     this.otherData.myInt = 1888;
        //     Print();
        // });
    }

    public void OnClick()
    {
        var index = Random.Range(0, 10);
        Debug.Log(index);
        switch (index)
        {
            case >= 0 and < 3:
                popupManager2.ShowPopup<PopupType>(new PopupTypeData(){title = "...Fuck you!托尼..."});
                break;
            case >= 3 and < 5:
                popupManager2.ShowPopup<PopupType1>(new PopupType1Data(){message = "...环游世界..."});
                break;
            case >=5 and < 10:
                popupManager2.ShowPopup<PopupType2>(new PopupType2Data(){name = "中国"});
                break;
        }
    }

    void Print()
    {
        Debug.Log(myData.myString);
        Debug.Log(myData.myInt);
        Debug.Log("other data of :"+this.otherData.myString);
        Debug.Log("other data of :"+this.otherData.myInt);
    }
}
