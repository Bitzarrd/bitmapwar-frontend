using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Web;
using Newtonsoft.Json;
using UnityEngine;

public class Game : Singleton<Game>
{
    public bool IsStarted = false;
    public string MyWallet;
    public string MyTapRootWallet;
    public int langIndex = 0;
    public double btcRemain;

    public string myPubKey;

    public LinkedList<JoinLog> joinLogs;
    public RentalBitmap[] rentals;

    public void AppendJoinLog(JoinLog jl)
    {
        if (joinLogs.Count >= 5)
        {
            joinLogs.RemoveLast();
        }
        joinLogs.AddFirst(jl);
    }

    public void AutoLogin()
    {
        if (string.IsNullOrEmpty(myPubKey))
        {
            return;
        }
        ProtoUtils.SendLogin(myPubKey);
    }

    public List<string> invinciableTile = new();

    public List<ActionLog> battleLogs;

    public bool isLogingOut = false;

    public Dictionary<string, Statistics> initStatistics = new();

    //public Dictionary<Vector2Int, Cell> cellDic = new();

    public void ClearStatics()
    {
        foreach (var v in initStatistics.Values)
        {
            v.land = 0;
            v.virus = 0;
            v.loss = 0;
            
            MainPage.inst.UpdateStatatics(v);
        }
    }

    public string myColor = "none";

    public string turnColor = "";

    public string soldierPrice;

    public Dictionary<int, int> parentList = new();

    public Dictionary<long, Player> playersOnBoard = new();

    public Dictionary<string, User> userOnBoard = new();

    public Dictionary<int, bool> defeatCells = new();

    public Vector2Int currentSelect = new();

    public bool IsLogin = false;
    public int mapHeight;
    public int mapWidth;

    public List<Cell> updateCell = new();

    public ExtractsLog[] extractHistory;
    public PurchaseLog[] purchaseHistory;

    public string[] myLandList;

    public User userMySelf;

    public LastRank[] lastRanks;

    public void onUpdateUser(User u)
    {
        if (userOnBoard.ContainsKey(u.address))
        {
            userOnBoard[u.address] = u;
        }
        else
        {
            userOnBoard.Add(u.address, u);
        }

        if (IsUserMyself(u))
        {
            //Debug.Log("user total Land : " + u.land);
            MainPage.inst.UpdateTotalLands(u.land);
            MainPage.inst.UpdateUserInfo(u);
        }
    }

    //分析战报
    public void ParseBattleLog(ActionLog[] logs)
    {
        if (logs.Length <= 0)
        {
            return;
        }
        Debug.Log("Add Logs Count:" + logs.Length);
        for (int i = 0; i < logs.Length; i++)
        {
            var item = logs[i];
            if (item.state == 1)
            {
                MainPage.inst.view.GetTransition("win").Play();
            }
            else if (item.state == 2)
            {
                MainPage.inst.view.GetTransition("lost").Play();
            }
            battleLogs.Add(logs[i]);
            var c1 = GridUtils.Inst.GetCoordByTileId(int.Parse(item.my_map_id));
            var c2 = GridUtils.Inst.GetCoordByTileId(int.Parse(item.enemy_map_id));
            MoveAnimMgr.inst.PlayExplosion(c1.x, c1.y);
            MoveAnimMgr.inst.PlayExplosion(c2.x, c2.y);
        }

        if (LogPage.Inst.isOpen)
        {
            LogPage.Inst.UpdateList();
        }
    }
    

    public int GetSolderByTile(string tileId)
    {
        foreach (var k in playersOnBoard.Keys)
        {
            var p = playersOnBoard[k];
            if (p.bitmap.Equals(tileId))
            {
                return p.virus;
            }
        }

        return 0;
    }

    public int GetSoldilerOfTile(string owner, string tileId)
    {
        foreach (var k in playersOnBoard.Keys)
        {
            var p = playersOnBoard[k];
            if (p.owner.Equals(owner) && p.bitmap.Equals(tileId))
            {
                return p.virus;
            }
        }

        return 0;
    }

    public bool IsTileMine(string tile)
    {
        if (myLandList == null)
        {
            return false;
        }

        var list = myLandList;
        for (int i = 0; i < list.Length; i++)
        {
            var l = list[i];
            if (l.Equals(tile))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsUserMyself(User u)
    {
        if (u.address.Equals(Game.Inst.MyWallet))
        {
            return true;
        }
        
        return false;
    }

    //private Vector2Int cellDicKey = new Vector2Int(0, 0);

    public void AppendUpdateCell(Cell c)
    {
        //cellDicKey.x = c.x;
        //cellDicKey.y = c.y;
        //cellDic[cellDicKey] = c;
        updateCell.Add(c);
    }

    public void UpdatePlayerOnBoard(long tileId, Player p)
    {
        if (!playersOnBoard.ContainsKey(tileId))
        {
            playersOnBoard.Add(tileId, p);
        }
        else
        {
            playersOnBoard[tileId] = p;
        }

        if (p.invincibility)
        {
            MoveAnimMgr.inst.PlaceLockFx(new Vector2Int(p.x, p.y));
        }
    }
    
    public void PostToTwitter(string text)
    {
        Share();
        text = HttpUtility.UrlEncode(text);

        string url = $"https://twitter.com/intent/tweet?text={text}";
        Application.OpenURL(url);
    }

    public void Share()
    {
        var s = new Share()
        {
            method = "Share"
        };
        if (MyWallet == null)
        {
            return;
        }
        s.owner = MyWallet;
        WebSocketClient.inst.Send(JsonConvert.SerializeObject(s));
    }

    public void UpdateCell(int x, int y, string col)
    {
        var c = new Cell()
        {
            x = x,
            y = y,
            color = col
        };
        
        updateCell.Add(c);
    }

    public void ClearAllMap()
    {
        updateCell.Clear();
        var map = GameObject.FindObjectOfType<MapMesh>();
        playersOnBoard.Clear();
        map.ClearAll();
        //map.Generate();
        defeatCells.Clear();
        //cellDic.Clear();
    }
}
