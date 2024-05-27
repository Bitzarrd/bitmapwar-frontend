using UnityEngine;

public abstract class PopupBase : MonoBehaviour
{
    public abstract void Show();

    public abstract void Hide();

    public abstract void SetData(PopupDataBase dataBase);
    // 可以在此处添加其他自定义方法和属性
}