using UnityEngine;
using UnityEngine.UI;

public class PopupType2 : PopupBase
{
    public override void Show()
    {
        gameObject.SetActive(true);
        // 添加PopupType2特定的显示逻辑
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
        // 添加PopupType2特定的隐藏逻辑
    }

    public override void SetData(PopupDataBase dataBase)
    {
        var data = (PopupType2Data)dataBase;
        var text = transform.GetComponent<Text>();
        text.text = data.name;
        Debug.Log("name:"+ data.name);
    }
}

public class PopupType2Data : PopupDataBase
{
    public string name { set; get; }
}