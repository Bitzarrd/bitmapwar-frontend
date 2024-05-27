using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleLogItem : MonoBehaviour
{
    public Text Time;

    public Text myTileId;

    public Text otherTileId;

    public Text ResultText;
    public Text LossSoldier;

    public void InitData(ActionLog al)
    {
        Time.text = GridUtils.Inst.GetRealTimeHoursAndMinuts(al.create_time);
        myTileId.text = "You:" + al.my_map_id.ToString();
        otherTileId.text = "Enemy:" + al.enemy_map_id.ToString();
        LossSoldier.text = "Soldier Loss:" + al.virus_loss.ToString();
        string resStr = "Draw";
        if (al.state == 2)
        {
            resStr = "Loss";
        }
        else if (al.state == 1)
        {
            resStr = "Win";
        }

        ResultText.text = "Result:" + resStr;
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
