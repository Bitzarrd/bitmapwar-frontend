using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderRankingItem : MonoBehaviour
{
    public Text rankNum;

    public Text addr;

    public Text data;

    public void FillData(string _rankNum, string _addr, string _data)
    {
        if (rankNum != null)
        {
            rankNum.text = _rankNum;
        }

        addr.text = _addr;
        data.text = _data;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
