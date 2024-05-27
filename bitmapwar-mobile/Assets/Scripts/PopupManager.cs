using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PopupManager : MonoBehaviour
{
    public GameObject popupPrefab;
    public float displayTime = 3f;

    private Queue<GameObject> popupQueue = new Queue<GameObject>();
    private GameObject currentPopup;
    private List<string> tempText = new List<string>();

    private void Awake()
    {
        tempText.Add("Fuck you! 托尼!");

        tempText.Add("Fuck you! 托尼! too2!");

        tempText.Add("Fuck you! 托尼! too3!");

        tempText.Add("Fuck you! 托尼! too4!");
    }

    private void ShowPopup(string message)
    {
        GameObject popup = Instantiate(popupPrefab, transform);
        popup.GetComponentInChildren<Text>().text = message;
        popup.gameObject.SetActive(false);
        popupQueue.Enqueue(popup);

        if (currentPopup == null)
        {
            ShowNextPopup();
        }
    }

    public void OnShowPopup()
    {
        int index = Random.Range(0, tempText.Count);
        Debug.Log(index);
        ShowPopup(tempText[index]);
    }

    private void ShowNextPopup()
    {
        if (popupQueue.Count == 0) return;

        currentPopup = popupQueue.Dequeue();
        currentPopup.gameObject.SetActive(true);
        StartCoroutine(AnimatePopup());
    }

    private IEnumerator AnimatePopup()
    {
        RectTransform rectTransform = currentPopup.GetComponent<RectTransform>();
        Debug.Log(rectTransform.anchoredPosition);
        // 将提示移动到屏幕内
        while (rectTransform.anchoredPosition.x < 0)
        {
            rectTransform.anchoredPosition += new Vector2(5, 0f);
            yield return null;
        }
        // 展示指定时间
        yield return new WaitForSeconds(displayTime);

        // 向上渐隐
        while (currentPopup.GetComponent<CanvasGroup>().alpha > 0)
        {
            rectTransform.anchoredPosition += new Vector2(0, 5f);
            var step = Time.deltaTime + 0.005f;
            currentPopup.GetComponent<CanvasGroup>().alpha -= step;
            yield return null;
        }
        
        Destroy(currentPopup);
        currentPopup = null;
        ShowNextPopup();
    }
}