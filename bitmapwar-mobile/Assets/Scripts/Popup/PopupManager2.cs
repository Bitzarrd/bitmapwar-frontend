using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager2 : MonoBehaviour
{
    /// <summary>
    /// 预制体对象
    /// </summary>
    [Tooltip("弹窗预制体，与poolSizes对应")]
    public GameObject[] popupPrefabs = new GameObject[1];
    
    /// <summary>
    /// 预制体对象池 大小
    /// </summary>
    [Tooltip("弹窗预制体对象池大小，与popupPrefabs对应")]
    public int[] poolSizes = new int[1];

    /// <summary>
    /// 类型,队列
    /// </summary>
    private Dictionary<Type, Queue<PopupBase>> popupPools = new Dictionary<Type, Queue<PopupBase>>();

    /// <summary>
    /// 当前展示队列
    /// </summary>
    private Queue<PopupBase> currentPopupQueue = new Queue<PopupBase>();
    private PopupBase currentPopup;
    
    /// <summary>
    /// 显示时间
    /// </summary>
    [Tooltip("物体显示时间")]
    public float displayTime = 3f;

    /// <summary>
    /// 默认动画
    /// </summary>
    private bool autoAnimation = false;
    
    /// <summary>
    /// 物体初始化位置
    /// </summary>
    private Vector2 initPosition;
    
    private void Start()
    {
        InitializePools();
    }

    /// <summary>
    /// 初始化对象池
    /// </summary>
    private void InitializePools()
    {
        for (var i = 0; i < popupPrefabs.Length; i++)
        {
            var popupPrefab = popupPrefabs[i];
            var poolSize = poolSizes[i];

            Type popupType = popupPrefab.GetComponent<PopupBase>().GetType();
            var popupQueue = new Queue<PopupBase>();
            for (var j = 0; j < poolSize; j++)
            {
                var popupObject = Instantiate(popupPrefab, transform);
                var popup = popupObject.GetComponent<PopupBase>();
                popup.gameObject.SetActive(false);
                popupQueue.Enqueue(popup);
            }
            popupPools.Add(popupType, popupQueue);
        }
    }
    
    /// <summary>
    /// 展示弹窗
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void ShowPopup<T>(PopupDataBase dataBase = null) where T : PopupBase
    {
        Type popupType = typeof(T);
        if (!popupPools.ContainsKey(popupType))
        {
            Debug.LogError($"Popup prefab of type {popupType.Name} not found in the pool.");
            return;
        }

        Queue<PopupBase> popupQueue = popupPools[popupType];
        if (popupQueue.Count == 0)
        {
            Debug.LogWarning($"No available popup of type {popupType.Name} in the pool. Creating a new one.");
            GameObject popupObject = Instantiate(popupPrefabs[GetPopupPrefabIndex(popupType)], transform);
            var popup = popupObject.GetComponent<PopupBase>();
            popup.SetData(dataBase);
            popupQueue.Enqueue(popup);
        }

        var availablePopup = popupQueue.Dequeue();
        availablePopup.SetData(dataBase);
        availablePopup.gameObject.SetActive(false);
        currentPopupQueue.Enqueue(availablePopup);

        if (currentPopup == null)
        {
            ShowNextPopup();
        }
    }
    
    /// <summary>
    /// 下一个展示弹窗
    /// </summary>
    private void ShowNextPopup()
    {
        if (currentPopupQueue.Count == 0) return;

        currentPopup = currentPopupQueue.Dequeue();
        currentPopup.gameObject.SetActive(true);
        StartCoroutine(AnimatePopup());
    }

    private IEnumerator AnimatePopup()
    { 
        RectTransform rectTransform = currentPopup.GetComponent<RectTransform>();
        var x = rectTransform.anchoredPosition.x;
        var y = rectTransform.anchoredPosition.y;
        initPosition = new Vector3(x,y,0);
        Debug.Log(rectTransform.anchoredPosition);
        if (autoAnimation)
        {
            // 将提示移动到屏幕内
            while (rectTransform.anchoredPosition.x < 0)
            {
                rectTransform.anchoredPosition += new Vector2(5, 0f);
                yield return null;
            }
        }
        
        // 展示指定时间
        yield return new WaitForSeconds(displayTime);
        if (autoAnimation)
        {
            // 向上渐隐
            while (currentPopup.GetComponent<CanvasGroup>().alpha > 0)
            {
                rectTransform.anchoredPosition += new Vector2(0, 5f);
                var step = Time.deltaTime + 0.005f;
                currentPopup.GetComponent<CanvasGroup>().alpha -= step;
                yield return null;
            }
        }

        ReturnPopupToPool(currentPopup);
        currentPopup = null;
        ShowNextPopup();
    }



    private void ReturnPopupToPool(PopupBase popup)
    {
        //重置属性
        popup.GetComponent<CanvasGroup>().alpha = 1;
        var rect = popup.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(initPosition.x,initPosition.y);
        
        popup.gameObject.SetActive(false);
        Type popupType = popup.GetType();
        if (popupPools.ContainsKey(popupType))
        {
            Queue<PopupBase> popupQueue = popupPools[popupType];
            var index = GetPopupPrefabIndex(popupType);
            if (popupQueue.Count < poolSizes[index])
            {
                popupQueue.Enqueue(popup);
            }
            else
            {
                Destroy(popup.gameObject);
            }
        }
        else
        {
            Debug.LogWarning(
                $"Popup prefab of type {popupType.Name} not found in the pool. Destroying the popup object.");
            Destroy(popup.gameObject);
        }
    }

    private int GetPopupPrefabIndex(Type popupType)
    {
        for (var i = 0; i < popupPrefabs.Length; i++)
        {
            if (popupPrefabs[i].GetComponent<PopupBase>().GetType() == popupType)
            {
                return i;
            }
        }
        return -1;
    }
}