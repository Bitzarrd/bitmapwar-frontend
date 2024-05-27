using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopMenu : MonoBehaviour
{
    public Button LoginUniSat;

    public Button LoginOkx;

    public void CloseSelf()
    {
        gameObject.SetActive(false);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        LoginUniSat.onClick.AddListener(()=>
        {
            MainPage.inst.ConnectWallet(0);
            CloseSelf();
        });
        LoginOkx.onClick.AddListener(()=>
        {
            MainPage.inst.ConnectWallet(1);
            CloseSelf();
        });
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
