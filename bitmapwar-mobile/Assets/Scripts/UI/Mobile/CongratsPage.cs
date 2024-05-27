using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CongratsPage : MonoBehaviour
{
    public Button closeBtn;

    public void OnClose()
    {
        gameObject.SetActive(false);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (closeBtn != null)
        {
            closeBtn.onClick.AddListener(OnClose);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
