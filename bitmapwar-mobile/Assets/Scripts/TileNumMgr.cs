using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileNumMgr : MonoBehaviour
{
    public Camera MainCam;

    public GameObject TileNum;

    private int width = 30;
    private int height = 20;

    public Queue<GameObject> tileNumObjs = new Queue<GameObject>();


    public static TileNumMgr inst;
    private void Awake()
    {
        inst = this;
    }

    public void ShowTileNum(int cx, int cy)
    {
        if(MainCam.orthographicSize > 0.5f )
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        var tmp = new Queue<GameObject>();
        for(int x = cx - 15; x < cx + 15; x ++)
        {
            for(int y = cy - 10; y < cy + 10; y++)
            {
                var lc = GridUtils.Inst.GetLogicCoords(new Vector2Int(x, y));
                
                var tileId = lc.y * 1000 + lc.x;
                var pos = new Vector2Int(lc.x, lc.y);
                var go = tileNumObjs.Dequeue();
                tmp.Enqueue(go);
                go.transform.position = GridUtils.Inst.GetTileScenePosByCoords(x, y);
                SetTileNum(go, tileId);
                if (GridUtils.Inst.IsCoordValid(lc.x, lc.y))
                {
                    go.SetActive(true);
                }
                else
                {
                    go.SetActive(false);
                }
            }
        }

        tileNumObjs = tmp;
    }

    public void SetTileNum(GameObject go, int tileNum)
    {
        var t = go.transform.GetChild(0).gameObject;
        var tt = t.GetComponent<TMP_Text>();
        tt.text = tileNum.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                var go = Instantiate(TileNum);
                go.transform.SetParent(transform);
                tileNumObjs.Enqueue(go);
            }
        }

        //ShowTileNum(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
