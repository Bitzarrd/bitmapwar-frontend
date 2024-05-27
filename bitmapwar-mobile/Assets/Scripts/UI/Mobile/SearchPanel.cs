using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SearchPanel : MonoBehaviour
{
    public Button searchBtn;
    public TMP_InputField inpNum;

    public void OnClose()
    {
        gameObject.SetActive(false);
    }

    public void onSearch()
    {
        var tileId = int.Parse(inpNum.text);
    }
    
    void Start()
    {
        searchBtn.onClick.AddListener(onSearch);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
