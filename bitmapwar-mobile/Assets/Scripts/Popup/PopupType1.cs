using UnityEngine;
using UnityEngine.UI;

public class PopupType1 : PopupBase
{
    public Text message;
    public override void Show()
    {
        gameObject.SetActive(true);
        // 添加PopupType1特定的显示逻辑
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
        // 添加PopupType1特定的隐藏逻辑
    }

    public override void SetData(PopupDataBase dataBase)
    {
        var data = (PopupType1Data)dataBase ;
        Debug.Log("message:"+data.message);
        this.message.text = data.message;
    }
}

public class PopupType1Data : PopupDataBase
{
    public string message { set; get; }
}