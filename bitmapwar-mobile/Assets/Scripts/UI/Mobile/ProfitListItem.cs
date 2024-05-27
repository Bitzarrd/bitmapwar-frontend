using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfitListItem : MonoBehaviour
{
    public List<Image> teamColor;

    public Text time;

    public Text profit;

    public Text lossSoldier;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void FillInData(UserHistoricalBenefit data)
    {
        int colorIndex = Consts.colorDic[data.team];
        for (int i = 0; i < 4; i++)
        {
            teamColor[i].gameObject.SetActive(false);
        }
        teamColor[colorIndex - 1].gameObject.SetActive(true);
        time.text = GridUtils.Inst.GetRealTimeDateAndTime(data.create_time);
        profit.text = Consts.StrToDouble(data.profit);
        lossSoldier.text = data.init_virus.ToString();
    }
}
