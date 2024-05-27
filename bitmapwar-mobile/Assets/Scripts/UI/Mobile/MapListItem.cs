using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapListItem : MonoBehaviour
{
    public ActionPanel actionPanel;
    public GameObject userPanel;
    public GameObject bitmapListPanel;
    
    public int currentChoose;
    
    public Text label;

    public Button btn;

    public void InitData(string mapData)
    {
        long tileId = long.Parse(mapData);
        string tileName = mapData;
        if (Game.Inst.playersOnBoard.ContainsKey(tileId))
        {
            tileName += " Soldier: ("  + Game.Inst.playersOnBoard[tileId].virus + ")";
        }
        else
        {
            tileName += " Soldier:(0)";
        }
        label.text = tileName;
        btn.onClick.AddListener(() =>
        {
            Debug.Log("Choose MapData: " + mapData);
            userPanel.SetActive(false);
            bitmapListPanel.SetActive(false);
            actionPanel.Open((int)tileId);
            currentChoose = int.Parse(mapData);
        });
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
