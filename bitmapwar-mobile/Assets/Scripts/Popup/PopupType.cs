using UnityEngine;
using UnityEngine.UI;

public class PopupType : PopupBase
{
    public Text title;
    
    public override void Show()
    {
        Debug.Log("Show:"+typeof(PopupType));
    }

    public override void Hide()
    {
        
        Debug.Log("Hide:"+typeof(PopupType));
    }

    public override void SetData(PopupDataBase dataBase)
    {
        var data = (PopupTypeData)dataBase ;
        Debug.Log("title:"+data.title);
        this.title.text = data.title;
    }
}

public class PopupTypeData : PopupDataBase
{
    public string title { set; get; }
}