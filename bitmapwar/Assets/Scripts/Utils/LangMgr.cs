using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class EnToZh
{
    public Dictionary<string, string> data;
}

public class LangMgr : MonoBehaviour
{
    public static LangMgr inst;
    public TextAsset stringTable;

    public EnToZh dic;
    void Start()
    {
        inst = this;
        dic = JsonConvert.DeserializeObject<EnToZh>(stringTable.text);
        /*
        dic.data = new();
        dic.data.Add("Invalid Number!", "非法数字");
        dic.data.Add("Invalid number!", "非法数字");
        dic.data.Add("Network Error! Please Refresh Page", "网络错误请刷新网页");
        dic.data.Add("Network Error!", "网络错误");
        dic.data.Add("Successfully extracted BTC, please check in wallet.", "成功提现，请检查网络");
        dic.data.Add("Purchase successful, soldier has entered account.", "支付成功");
        dic.data.Add("You Must Be Logged in.", "您必须先登录");
        dic.data.Add("Share Success !", "分享成功");
        dic.data.Add("Please click on the color to select the camp, otherwise soldiers cannot be placed.", "请选择颜色");
        Debug.Log("Jerry: " + JsonConvert.SerializeObject(dic));
        */

        //dic = JsonConvert.DeserializeObject<EnToZh>(stringTable.text);
    }
    
    public string GetString(string key)
    {
        if (Game.Inst.langIndex == 0)
        {
            return key;
        }
        else
        {
            Debug.Log("GetKey:" + key);
            if (dic.data.ContainsKey(key))
            {
                return dic.data[key];
            }
            else
            {
                return key;
            }
        }
    }
    
}
