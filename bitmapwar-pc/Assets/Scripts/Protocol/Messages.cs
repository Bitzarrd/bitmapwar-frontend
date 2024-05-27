using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class LastRankComparer : IComparer<LastRank>
{
    public int Compare(LastRank x, LastRank y)
    {
        var a = BigInteger.Parse(x.profit);
        var b = BigInteger.Parse(y.profit);
        
        if (a > b)
        {
            return -1;
        }
        else if (a < b)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}

public class ProtoUtils
{
    public static byte[] Decompress(byte[] data)
    {
        using (var compressedStream = new MemoryStream(data))
        using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
        using (var resultStream = new MemoryStream())
        {
            gzipStream.CopyTo(resultStream);
            byte[] resultBytes = resultStream.ToArray();
            Debug.Log("Decompress Result Lenght:" + resultBytes.Length);
            return resultBytes;
        }
    }

    public static void SendBatchPlace(int num, string c)
    {
        var msg = new JoinGameBatch()
        {
            method = "JoinGameBatch",
            virus = num,
            owner = Game.Inst.MyWallet,
            color = c,
        };
        Debug.Log("Batch Place: " + num + " color: " + c);
        
        WebSocketClient.inst.Send(JsonConvert.SerializeObject(msg));
    }

    public static void ParseReload(Reload data)
    {
        MapMesh.inst.gridSizeX = data.gridWidth;
        MapMesh.inst.gridSizeY = data.gridHeight;

        GridUtils.Inst.gridSizeX = data.gridWidth;
        GridUtils.Inst.gridSizeY = data.gridHeight;

        Game.Inst.invinciableTile = new List<string>();
        
        MapMesh.inst.Generate();
        var gridStr = Convert.FromBase64String(data.grid[0]); //higher
        var lowStr = Convert.FromBase64String(data.grid[1]); //lower
        var gridArray = Decompress(gridStr);
        var low_gridArray = Decompress(lowStr);
        
        Timer.inst.SetServerTime(data.now_time);
        Game.Inst.mapHeight = data.gridHeight;
        Game.Inst.mapWidth = data.gridWidth;

        for (int i = 0; i < data.invincibility_maps.Length; i++)
        {
            var t = data.invincibility_maps[i];
            var tp = GridUtils.Inst.GetCoordByTileId(int.Parse(t));
            MoveAnimMgr.inst.PlaceLockFx(tp);
        }
        
        /*
        data.dead_cells = new DeadCenterCell[1];
        data.dead_cells[0] = new DeadCenterCell();
        data.dead_cells[0].x = 100;
        data.dead_cells[0].y = 100;
        */

        Game.Inst.joinLogs = new LinkedList<JoinLog>();

        for (int i = 0; i < data.join_Logs.Length; i++)
        {
            var joinL = data.join_Logs[i];
            Game.Inst.joinLogs.AddLast(joinL);
        }
        
        MainPage.inst.OnUpdateLastPlacePlayer();
        
        for (int y = 0; y < data.gridHeight; y++)
        {
            for (int x = 0; x < data.gridWidth; x++)
            {
                var index = y * data.gridWidth + x; 
                var t = low_gridArray[index] + (gridArray[index] << 8);

                if (t != 0)
                {
                    //Debug.Log("Tile: " + t + "at :" + x + " : " + y);
                    if (data.players.Length <= t - 1)
                    {
                        Debug.Log("Canno t Font Player At " + (t - 1).ToString() + " Players Length:" + data.players.Length);
                    }
                    else
                    {
                        var p = data.players[t - 1];
                        var c = p.color;
                        Game.Inst.UpdateCell(x, y, c);
                    }
                }
            }
        }

        for (int i = 0; i < data.statistics.Length; i++)
        {
            var s = data.statistics[i];
            if (Game.Inst.initStatistics.ContainsKey(s.team))
            {
                
                Game.Inst.initStatistics[s.team] = s;
            }
            else
            {
                Game.Inst.initStatistics.Add(s.team, s);
            }
        }

        /*
        data.last_rank = new[]
        {
            new LastRank() { rank = 1, owner = "bc1w8qkzfysg7cvdqkll8mp89pjfxk9flqxh0z",profit = "7920000000000" },
            new LastRank() { rank = 2, owner = "bc1w8qkzfysg7cvdqkll8mp89pjfxk9flqxh0z",profit = "7920000000000" },
            new LastRank() { rank = 4, owner = "bc1w8qkzfysg7cvdqkll8mp89pjfxk9flqxh0z",profit = "7920000000000" },
            new LastRank() { rank = 3, owner = "bc1qkzfysg7cvdqkll8mp89pjfxk9flqxh0z",profit = "7920000000000" },
            new LastRank() { rank = 6, owner = "bc1sssss8qkzfysg7cvdqkll8mp89pjfxk9flqxh0z",profit = "7920000000000" },
            new LastRank() { rank = 5, owner = "bc1w8qk",profit = "7920000000000" },
        };
        */

        Array.Sort(data.last_rank, new LastRankComparer());

        for (int i = 0; i < data.last_rank.Length; i++)
        {
            var r = data.last_rank[i];
            MainPage.inst.AddRanking(r, i + 1);
        }

        Game.Inst.lastRanks = data.last_rank;
        
        ParseDefeat(data.dead_cells);
        MainPage.inst.UpdateJackPot(data.jackpot);
        MainPage.inst.UpdateTotalBonus(data.total_bonus);

        for (int i = 0; i < data.players.Length; i++)
        {
            var pp = data.players[i];
            Game.Inst.UpdatePlayerOnBoard(long.Parse(pp.bitmap), data.players[i]);
        }
        //如果Stoptime比现在早
        int duration = data.stop_time - (int)data.now_time;
        if(duration > 0) {
            Timer.inst.SetEndTime(data.stop_time);
            Timer.inst.StartGame();
            AudioMgr.inst.PlayBattle();
        }
        else {
            AudioMgr.inst.PlayNormal();
            Timer.inst.EndGame(data.next_round);
        }

        Game.Inst.soldierPrice = Consts.StrToDouble(data.virus_price, "0.###########");
        Debug.Log("virus price is :" + data.virus_price + " Res: " + Game.Inst.soldierPrice);
    }

    public static void SendPayReulst(string orderID)
    {
        var msg = new Purchase()
        {
            method = "Purchase",
            txid = orderID
        };
        Debug.Log("Send Pay Resulst order: " + orderID);
        
        WebSocketClient.inst.Send(JsonConvert.SerializeObject(msg));
    }

    public static void SendExtract(string addr, string amount)
    {
        var msg = new ExtractProfit()
        {
            method = "ExtractProfit",
            address = addr,
            amount = amount,
        };
        WebSocketClient.inst.Send(JsonConvert.SerializeObject(msg));
    }

    public static void SendLogin(string wallet = "02b13a59a27e6268117b1abc19f1b147f56bb65f89136ec574457a7466401d6652")
    {
        var msg = new Login()
        {
            method = "Login",
            address = wallet,
        };
        Game.Inst.myPubKey = wallet;
        WebSocketClient.inst.Send(JsonConvert.SerializeObject(msg));
    }

    public static void SendLoadMap(string myWallet)
    {
        var loadMp = new LoadMap2()
        {
            method = "LoadMap2",
            owner = myWallet,
        };
        WebSocketClient.inst.Send(JsonConvert.SerializeObject(loadMp));
    }

    public static void SendPlaceSoldier(int mapId, int count, string col)
    {
        var jg = new JoinGame2()
        {
            method = "JoinGame2",
            map_id = mapId,
            virus = count,
            owner = Game.Inst.MyWallet,
            color = col,
        };

        MainPage.inst.mySoldiers[mapId] = col;
        Game.Inst.turnColor = col;
        MapMesh.inst.SetFocus();
        
        WebSocketClient.inst.Send(JsonConvert.SerializeObject(jg));
    }

    public static void ParseDefeat(DeadCenterCell[] cs)
    {
        for (int i = 0; i < cs.Length; i++)
        {
            var c = cs[i];
            var tileId = GridUtils.Inst.GetTileIDByPos(c.x, c.y);
            if (!Game.Inst.defeatCells.ContainsKey(tileId))
            {
                Game.Inst.defeatCells.Add(tileId, true);
                MoveAnimMgr.inst.PlayDefeat(c.x, c.y);
            }
        }
    }

    private static int updateCount = 0;

    public static void ParseUpdate(Update data)
    {
        updateCount++;
        //Debug.Log("update Count is:" + updateCount);
        //Debug.Log("Parse Update: " + data.method);
        if (data.payload != null)
        {
            //Debug.Log("Play load : " + data.payload.Length);
            for (int i = 0; i < data.payload.Length; i++)
            {
                var c = data.payload[i];
                var index = Consts.colorDic[c.color];
                Game.Inst.AppendUpdateCell(c);
            }
        }
        
        MainPage.inst.UpdateTurn(data.turn);
        MainPage.inst.UpdateTotalBonus(data.total_bonus);
        MainPage.inst.UpdateJackPot(data.jackpot);
        
        Game.Inst.ParseBattleLog(data.action_logs);
        
        ParseDefeat(data.dead_cells);
        
        for (int i = 0; i < data.statistics.Length; i++)
        {
            var s = data.statistics[i];
            MainPage.inst.UpdateStatatics(s);
        }
    }

    public static void PlaceBatchOK(JoinedGameBatchSuccess jgbs)
    {
        foreach (var p in jgbs.players)
        {
            Game.Inst.AppendUpdateCell(new Cell()
            {
                x =  p.x,
                y = p.y,
                color = p.color
            });    
            Game.Inst.UpdatePlayerOnBoard(int.Parse(p.bitmap), p);

            if (!Game.Inst.initStatistics.ContainsKey(p.color))
            {
                Game.Inst.initStatistics[p.color] = new Statistics();
            }

            var s = Game.Inst.initStatistics[p.color];
            s.virus += p.virus;
            s.land += p.land;
            MainPage.inst.UpdateStatatics(s);
        }
        var jl = new JoinLog()
        {
            address = jgbs.user.taproot_address,
            create_time = jgbs.create_time
        };
        
        Game.Inst.AppendJoinLog(jl);
        MainPage.inst.OnUpdateLastPlacePlayer();
        
        
        Game.Inst.onUpdateUser(jgbs.user);
        MainPage.inst.RefreshTileInfo();
        MainPage.inst.OnUpdatePlayerSoldier(jgbs.user);
        for (int i = 0; i < jgbs.statistics.Length; i++)
        {
            var s = jgbs.statistics[i];
            MainPage.inst.UpdateStatatics(s);
        }
    }

    public static void PlacePlayerOK(JoinedGameSuccess data)
    {
        var p = data.player;
        Game.Inst.AppendUpdateCell(new Cell()
        {
            x =  p.x,
            y = p.y,
            color = p.color
        });
        var jl = new JoinLog()
        {
            address = data.user.taproot_address,
            create_time = data.create_time
        };
        
        Game.Inst.AppendJoinLog(jl);
        MainPage.inst.OnUpdateLastPlacePlayer();

        /*
        if (!Game.Inst.initStatistics.ContainsKey(p.color))
        {
            Game.Inst.initStatistics[p.color] = new Statistics();
        }

        var s = Game.Inst.initStatistics[p.color];
        s.virus = p.virus;
        s.land = p.land;
        */
        foreach (var s in data.statistics)
        {
            MainPage.inst.UpdateStatatics(s);
        }
            
        Game.Inst.UpdatePlayerOnBoard(int.Parse(p.bitmap), p);
        Game.Inst.onUpdateUser(data.user);
        MainPage.inst.RefreshTileInfo();
        MainPage.inst.OnUpdatePlayerSoldier(data.user);
    }

    public static void Parse(string data)
    {
        var msg = JsonConvert.DeserializeObject<MsgBase>(data);
        switch (msg.method)
        {
            case "Reload":
                Debug.Log("Parse Reload:" + data);
                var reload = JsonConvert.DeserializeObject<Reload>(data);
                ParseReload(reload);
                break;
            case "Update":
                var update = JsonConvert.DeserializeObject<Update>(data);
                //1350325 Length
                //Debug.Log("Update:" + data.Length);
                ParseUpdate(update);
                break;
            case "LoginSuccess":
                Debug.Log("LoginSuccess...." + data);
                var loginInfo = JsonConvert.DeserializeObject<LoginSuccess>(data);
                Game.Inst.MyWallet = loginInfo.user.address;
                Game.Inst.MyTapRootWallet = loginInfo.user.taproot_address;
                MainPage.inst.UpdateUserInfo(loginInfo.user);
                MainPage.inst.SetLoginOk(loginInfo.user.address, loginInfo.user.taproot_address);
                MainPage.inst.OnLogin();
                Game.Inst.rentals = loginInfo.rentals;
                if (!String.IsNullOrEmpty(loginInfo.exist_color))
                {
                    Game.Inst.turnColor = loginInfo.exist_color;
                    Game.Inst.myColor = loginInfo.exist_color;
                    MainPage.inst.SetColor(loginInfo.exist_color);
                }

                MainPage.inst.myTeamMessage = loginInfo.message_team.ToList();
                MainPage.inst.globalMessage = loginInfo.message_global.ToList();
                
                PCStartUp.inst.gameObject.SetActive(false);
                
                /*
                else
                {
                    MainPage.inst.RandomColor();
                }
                */
                    
                Game.Inst.extractHistory = loginInfo.extracts;
                Game.Inst.purchaseHistory = loginInfo.purchase;
                Game.Inst.onUpdateUser(loginInfo.user);
                Game.Inst.IsLogin = true;
                
                /*
                Game.Inst.battleLogs = new ()
                {
                    new ActionLog() { create_time = 100, enemy_map_id = "bsoe", my_map_id = "woijo1my", state = 0, virus_loss = 100 },
                    new ActionLog() { create_time = 100, enemy_map_id = "bsoe", my_map_id = "woijo1my", state = 2, virus_loss = 100 },
                    new ActionLog() { create_time = 100, enemy_map_id = "bsoe", my_map_id = "woijo1my", state = 1, virus_loss = 100 },
                    new ActionLog() { create_time = 100, enemy_map_id = "bsoe", my_map_id = "woijo1my", state = 0, virus_loss = 100 },
                    new ActionLog() { create_time = 100, enemy_map_id = "bsoe", my_map_id = "woijo1my", state = 2, virus_loss = 100 },
                    new ActionLog() { create_time = 100, enemy_map_id = "bsoe", my_map_id = "woijo1my", state = 1, virus_loss = 100 },
                    new ActionLog() { create_time = 100, enemy_map_id = "bsoe", my_map_id = "woijo1my", state = 1, virus_loss = 100 },
                    new ActionLog() { create_time = 100, enemy_map_id = "bsoe", my_map_id = "woijo1my", state = 0, virus_loss = 100 },
                    new ActionLog() { create_time = 100, enemy_map_id = "bsoe", my_map_id = "woijo1my", state = 0, virus_loss = 100 },
                };
                */
                /*
                var tt = new ActionLog[]
                {
                    new ActionLog() { create_time = 100, enemy_map_id = "bsoe", my_map_id = "woijo1my", state = 2, virus_loss = 100 },
                };
                */

                Game.Inst.battleLogs = new();
                //Game.Inst.ParseBattleLog(tt);
                Game.Inst.ParseBattleLog(loginInfo.action_logs);

                Debug.Log("Login Res:" + data);
                break;
            case "GameStarted":
                var gs = JsonConvert.DeserializeObject<GameStarted>(data);
                //Debug.Log("Game Started...");
                Game.Inst.IsStarted = true;
                //GameFightPage.Inst.Open();
                MainPage.inst.PlayGameStartAnim();
                AudioMgr.inst.PlayBattle();
                Game.Inst.ClearAllMap();
                for (int i = 0; i < gs.players.Length; i++)
                {
                    var pp = gs.players[i];
                    Game.Inst.UpdatePlayerOnBoard(long.Parse(pp.bitmap), pp);
                }
                Timer.inst.SetEndTime(gs.stop_time);
                Timer.inst.StartGame();
                Game.Inst.invinciableTile = new();
                for (int i = 0; i < gs.invincibility_maps.Length; i++)
                {
                    var t = gs.invincibility_maps[i];
                    Game.Inst.invinciableTile.Add(gs.invincibility_maps[i]);
                    var tp = GridUtils.Inst.GetCoordByTileId(int.Parse(t));
                    MoveAnimMgr.inst.PlaceLockFx(tp);
                }
                break;
            case "JoinedGameSuccess":
                //Debug.Log("Place OK!" + data);
                var jgs = JsonConvert.DeserializeObject<JoinedGameSuccess>(data);
                PlacePlayerOK(jgs);
                break;
            case "SetNextRoundSuccess":
                var snrs = JsonConvert.DeserializeObject<SetNextRoundSuccess>(data);
                AudioMgr.inst.PlayNormal();
                Game.Inst.turnColor = "";
                Timer.inst.EndGame(snrs.next_round);
                break;
            case "Settlement":
                var res = JsonConvert.DeserializeObject<Settlement>(data);
                Game.Inst.ClearStatics();
                
                Array.Sort(res.rank, new LastRankComparer());
                MainPage.inst.UpdateRanking(res);
                SettlementPage.Inst.Open(res);
                Game.Inst.battleLogs.Clear();
                Game.Inst.lastRanks = res.rank;
                break;
            case "ErrorMsg":
                var err = JsonConvert.DeserializeObject<ErrorResponse>(data);
                Toast.Inst.Open(err.error_message);
                break;
            case "BroadcastChatMessage":
                //Debug.Log("Get Chat Message: " + data);
                var bcm = JsonConvert.DeserializeObject<BroadcastChatMessage>(data);
                MainPage.inst.AppendChatMessage(bcm.message);
                break;
            case "LoadMapSuccess":
                /* Debug.Log(data);
                var lms = JsonConvert.DeserializeObject<LoadMapSuccess>(data);
                if (BitmapListPage.Inst.isOpen)
                {
                    BitmapListPage.Inst.FillTilesList(lms.result_data);
                }

                Game.Inst.myLandList = lms.result_data;*/
                break;
            case "LoadMap2Success":
                //Debug.Log(data);
                var lm2s = JsonConvert.DeserializeObject<LoadMap2Success>(data);
                var totalTiles = new List<string>();
                for (int i = 0; i < lm2s.maps.Length; i++)
                {
                    totalTiles.Add(lm2s.maps[i]);
                }
                if (BitmapListPage.Inst.isOpen)
                {
                    BitmapListPage.Inst.FillTilesList(lm2s.maps, lm2s.rentals, lm2s.occupied);
                }

                for (int i = 0; i < lm2s.rentals.Length; i++)
                {
                    totalTiles.Add(lm2s.rentals[i].bitmap_id.ToString());
                }

                var tiles = totalTiles.ToArray();
                Game.Inst.myLandList = tiles;
                MainPage.inst.UpdateBitmapsCount(totalTiles.Count);
                for (int i = 0; i < totalTiles.Count; i++)
                {
                    var t = GridUtils.Inst.GetCoordByTileId(int.Parse(totalTiles[i]));
                    MoveAnimMgr.inst.PlaceOriginFx(t);
                }
                break;
            case "ExtractProfitSuccess":
                //Debug.Log("Extract Profit:" + data);
                var eps = JsonConvert.DeserializeObject<ExtractProfitSuccess>(data);
                Toast.Inst.Open("Successfully extracted BTC, please check in wallet.");
                #if UNITY_WEBGL
                JSBridge.inst.Extract(eps.user.merlin_address, eps.amount, eps.signature, eps.nonce, eps.create_time);
                #endif
                MainPage.inst.UpdateUserInfo(eps.user);
                if (ExtractProfitPage.Inst.isOpen)
                {
                    ExtractProfitPage.Inst.MakeRefreshPage();
                }
                break;
            case "PurchaseSuccess":
                Debug.Log("Buy Result:" + data);
                var ps = JsonConvert.DeserializeObject<PurchaseSuccess>(data);
                Toast.Inst.Open("Purchase successful, soldier has entered account.");
                Game.Inst.onUpdateUser(ps.user);
                Game.Inst.purchaseHistory = ps.purchases;
                if (BuySoldierPage.Inst.isOpen)
                {
                    BuySoldierPage.Inst.RefreshPurchaseLog();
                }
                break;
            case "UpdateExtractSuccess":
                Debug.Log("Update Extract Success");
                var updateData = JsonConvert.DeserializeObject<UpdateExtractSuccess>(data);
                ExtractProfitPage.Inst.UpdateItem(updateData);
                break;
            case "ShareSuccess":
                Toast.Inst.Open("Share Success !");
                break;
            case "InvincibilityCompromised":
                MoveAnimMgr.inst.ClearLockFx();
                break;
            case "JackpotLightUp":
                //Debug.Log("Jackpot light Up:" + data);
                var jlu = JsonConvert.DeserializeObject<JackpotLightUp>(data);
                JackpotPage.Inst.Open(jlu);
                break;
            case "JoinedGameBatchSuccess":
                //Debug.Log("Batch Join: " + data);
                var jgbs = JsonConvert.DeserializeObject<JoinedGameBatchSuccess>(data);
                PlaceBatchOK(jgbs);
                break;
            case "GetLeaderBoardSuccess":
                //Debug.Log("GetLeadre BoardSuccess: " + data);
                if (TotalRanking.Inst.isOpen)
                {
                    var glbs = JsonConvert.DeserializeObject<GetLeaderBoardSuccess>(data);
                    TotalRanking.Inst.OnResponse(glbs);
                }
                break;
            case "RentBitmapSuccess":
                Debug.Log(data);
                var rbs = JsonConvert.DeserializeObject<RentBitmapSuccess>(data);
                Game.Inst.userMySelf = rbs.user;
                Toast.Inst.Open("Rent Success!");
                break;
            case "BuyGoodsForRentMapSuccess":
                //Debug.Log(data);
                var bgfrms = JsonConvert.DeserializeObject<BuyGoodsForRentMapSuccess>(data);
                Toast.Inst.Open("Rent Success!");
                break;
            case "GetUserHistoricalBenefitSuccess":
                //Debug.Log("Get History Benefist");
                //TODO fill in the page of history profits;
                var guhbs = JsonConvert.DeserializeObject<GetUserHistoricalBenefitSuccess>(data);
                if (ProfitHistoryPage.Inst.isOpen)
                {
                    ProfitHistoryPage.Inst.OnData(guhbs);
                }
                
                break;
            case "PurchaseVirusWithProfitSuccess":
                var pvwps = JsonConvert.DeserializeObject<PurchaseVirusWithProfitSuccess>(data);
                //Debug.Log("Purchase Virus With Profit");
                Toast.Inst.Open("Purchase successful, soldier has entered account.");
                MainPage.inst.UpdateUserInfo(pvwps.user);
                Game.Inst.purchaseHistory = pvwps.purchases;
                if (BuySoldierPage.Inst.isOpen)
                {
                    BuySoldierPage.Inst.RefreshPurchaseLog();
                }
                break;
            case "QueryBitmapAvailableForRentResponse":
                var qbafrr = JsonConvert.DeserializeObject<QueryBitmapAvailableForRentResponse>(data);
                Debug.Log(data);
                RentPage.Inst.UpdateTileStatus(qbafrr);
                break;
            case "JackpotLightUpWithoutPlayers":
                Debug.Log("Without Players : " + data);
                var jluwp = JsonConvert.DeserializeObject<JackpotLightUpWithoutPlayers>(data);
                if (NewCongratsPage.Inst.isOpen)
                {
                    NewCongratsPage.Inst.Close();
                }
                MainPage.inst.UpdateJackPot(jluwp.jackpot);
                if (Game.Inst.IsUserMyself(jluwp.user))
                {
                    Game.Inst.userMySelf = jluwp.user;
                    MainPage.inst.UpdateUserInfo(jluwp.user);
                }
                NewCongratsPage.Inst.Open(jluwp);
                break;
            case "AskForWebSuccess":
                var afws = JsonConvert.DeserializeObject<AskForWebSuccess>(data);
                Game.Inst.PCCode = afws.code;
                Game.Inst.PCMainPageHTML = afws.url;
                Application.OpenURL(afws.url);
                //PCStartUp.inst.gameObject.SetActive(false);
                break;
            default:
                Debug.Log("Unknow Method:" + msg.method);
                break;
        }
    }
}

public class SetNextRoundSuccess : MsgBase
{
    //{"method":"SetNextRoundSuccess","next_round":1705573703,"turn":0}
    
    public int next_round { get; set; }
    public int turn { get; set; }
}

public class JoinGameBatch : MsgBase
{
    public int virus { get; set; }
    public string color { get; set; }
    public string owner { get; set; }
}

public class GetExtractPurchaseLog : MsgBase
{
    public GetExtractPurchaseLog()
    {
        method = "GetExtractPurchaseLog";
    }
}

public class GetExtractPurchaseLogSuccess : MsgBase
{
}

public class JoinedGameBatchSuccess : MsgBase
{
    public Player[] players { get; set; }
    public User user { get; set; }
    
    public int create_time { get; set; }
    public Statistics[] statistics { get; set; }
}

public class ErrorResponse : MsgBase
{
    public int error_code { get; set; }
    public string error_message { get; set; }
}

public class Purchase : MsgBase
{
    public string txid { get; set; }
}

public class ExtractProfit : MsgBase
{
    public string amount { get; set; }
    public string address { get; set; }
}

public class PurchaseSuccess : MsgBase
{
    public User user { get; set; }
    
    public PurchaseLog[] purchases { get; set; }
}

public class ExtractProfitSuccess : MsgBase
{
    public string amount;
    public string signature;
    public int nonce;
    public string create_time;
    public User user;
}

public class InvincibilityCompromised :MsgBase
{
    
}

public class ExtractsLog : MsgBase
{
    public int id;
    public string address;
    public string amount;
    public string txid;
    public int status;
    public int create_time;
    public string signature;
}

public class UpdateExtract : MsgBase
{
    public int id;
    public string txid;
}

public class UpdateExtractSuccess : MsgBase
{
    public int id;//  | int  | 就是nonce  |
    public string txid;//  | string  | 订单号  |
    public int status;// | int | 状态 0:Pending中 1:Success已完成 |
    public ExtractsLog[] extracts;
}

public class PurchaseLog : MsgBase
{
    public int id;
    public string txid;
    public string fee;
    public int create_time;
    public string owner;
    public int virus;
    public int status;
}

public class Reload : MsgBase
{
    public string[] grid { get; set; }
    public int gridWidth { get; set; }
    public int gridHeight { get; set; }
    public Player[] players { get; set; }
    public int next_round { get; set; }
    public Statistics[] statistics { get; set; }
    public int stop_time { get; set; }
    public LastRank[] last_rank { get; set; }
    public string jackpot { get; set; }
    public string total_bonus { get; set; }
    public long now_time { get; set; }

    public string virus_price { get; set; }
    
    public JoinLog[] join_Logs { get; set; }

    public DeadCenterCell[] dead_cells;
    
    public string[] invincibility_maps;
    
}

public class LastRank {
    public string owner { get; set; }
    public string taproot_address { get; set; }
    public string profit { get; set; }
    public int land { get; set; }
    public int rank { get; set; }
}

public class GetUserHistoricalBenefit : MsgBase
{
    public GetUserHistoricalBenefit()
    {
        method = "GetUserHistoricalBenefit";
    }
}

public class UserHistoricalBenefit
{ 
    public int create_time; //  | int  | 时间戳  |
    public string team; // | string  | 颜色  |
    public string profit; //  | string  | 表示btc金额 单位是聪 1表示 0.000000000000000001btc  |
    public int init_virus; // | int | 初始化兵力 ｜
}

public class PurchaseVirusWithProfit : MsgBase
{
    public PurchaseVirusWithProfit() {method = "PurchaseVirusWithProfit";}
    public int amount { get; set; }
}

public class PurchaseVirusWithProfitSuccess : MsgBase
{
    public User user;
    public PurchaseLog[] purchases; // 充值记录
}

public class GetUserHistoricalBenefitSuccess : MsgBase
{
    public UserHistoricalBenefit[] benefits;
}

public class Login : MsgBase
{
    public string address { get; set; }
}

public class JoinLog
{
    public string address;
    public int create_time;
}

public class LoginSuccess : MsgBase
{
    public User user { get; set; }
    public bool has_login_gift { get; set; }
    public ExtractsLog[] extracts { get; set; }
    public PurchaseLog[] purchase { get; set; }
    
    public string exist_color { get; set; }
    
    public ActionLog[] action_logs { get; set; }
    public Message[] message_global;// | []Message | 全局消息 ｜
    public Message[] message_team; //| []Message | 自队消息 ｜
    public RentalBitmap[] rentals;
}

public class Share : MsgBase
{
    public string owner { get; set; }
}

public class ShareSuccess : MsgBase
{
    public User user { get; set; }
}

public class GameStarted : MsgBase
{
    public int gridWidth { get; set; }
    public int gridHeight { get; set; }
    public int turn { get; set; }
    public int start_time { get; set; }
    public int stop_time { get; set; }
    public Player[] players { get; set; }
    public string[] invincibility_maps { get; set; }
}

public class JoinGame2 : MsgBase
{
    public int map_id { get; set; }
    public int virus { get; set; }
    public string owner { get; set; }
    public string color { get; set; }
}

public class JoinedGameSuccess :MsgBase
{
    public Player player { get; set; }
    public User user { get; set; }
    
    public Statistics[] statistics { get; set; }
    
    public int create_time { get; set; }
}

public class Message : MsgBase
{
    public string from;
    public string color;
    public string content;
}

public class SendChatMessage : MsgBase
{
    public SendChatMessage()
    {
        method = "SendChatMessage";
    }

    public string color = "global";
    public string content;
}

public class BroadcastChatMessage : MsgBase
{
    public Message message;
}

public class Update : MsgBase
{
    public Cell[] payload { get; set; }
    public int turn { get; set; }
    public Statistics[] statistics { get; set; }
    public string total_bonus { get; set; }
    public string jackpot { get; set; }
    
    public DeadCenterCell[] dead_cells { get; set; }
    
    public ActionLog[] action_logs { get; set; }
}

public class Settlement : MsgBase
{
    public Statistics[] statistics { get; set; }
    public Statistics my_statistics { get; set; }
    public int next_round { get; set; }
    public User user { get; set; }
    public string earning { get; set; }
    public LastRank[] rank { get; set; }
}

public class LoadMap : MsgBase
{
    public string owner { get; set; }
}

public class LoadMap2 : MsgBase
{
    public string owner { get; set; }
}


public class LoadMapSuccess : MsgBase
{
    public Root result_data { get; set; }
}

public class LoadMap2Success : MsgBase
{
    public string[] maps { get; set; }
    public RentalBitmap[] rentals { get; set; }
    public RentalBitmap[] occupied{ get; set; }
}

public class JackpotLightUp : MsgBase
{
    public int land { get; set; }
    public string jackpot { get; set; }
    public string team { get; set; }
    public User[] users { get; set; }
    public User bw_user { get; set; }
}

public class Cell {
    public int x { get; set; }
    public int y { get; set; }
    public string color { get; set; }
    
    public bool fight { get; set; }
}

public class DeadCenterCell
{
    public int x { get; set; }
    public int y { get; set; }
    public int player_index { get; set; }
    public string color { get; set; }
}

public class Player {
    public string bitmap { get; set; }
    public string color { get; set; }
    public int init_virus { get; set; }
    public int virus { get; set; }
    public int loss { get; set; }
    public int land { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public string owner { get; set; }

    public bool invincibility; //|bool | 是否无敌
}

public class Statistics {
    public string team { get; set; }
    public int virus { get; set; }
    public int loss { get; set; }
    public int land { get; set; }
}

public class User {
    public string address { get; set; }
    public string taproot_address { get; set; }
    public string merlin_address {get; set;}
    public string profit { get; set; }
    public int virus { get; set; }
    public int land { get; set; }
    public string jackpot { get; set; }
    public string jackpot_bw { get; set; }
    public int points { get; set; }
    public string public_key { get; set; }
    public string total_profit { get; set; }
    
    public int energy { get; set; } //  | int    | 能量     
}

public class GetLeaderBoard : MsgBase
{
    public string tab { get; set; }
}

public class GetLeaderBoardSuccess : MsgBase
{
    public int my_self_rank { get; set; }
    public string tab { get; set; }
    public User[] users { get; set; }
}

public class ActionLog : MsgBase
{
    public int create_time { get; set; }
    public int state { get; set; }//|int |状态 1=胜利 2=失败 0=平|
    public int virus_loss { get; set; }
    public string my_map_id { get; set; }//|string |我的地块ID|
    public string enemy_map_id { get; set; }//lstring !敌人的地块ID
}

#region Rent
public class QueryBitmapAvailableForRent : MsgBase
{
    public QueryBitmapAvailableForRent()
    {
        method = "QueryBitmapAvailableForRent";
    }

    public int map_id;
}

public class QueryBitmapAvailableForRentResponse : MsgBase
{
    public int map_id;
    public bool available;
}

public class JackpotLightUpWithoutPlayers : MsgBase
{
    public User user;
    public string amount;
    public int create_now;
    public string jackpot;
}

public class BuyGoodsForRentMapSuccess : MsgBase
{
    public int map_id;//  | int    | 地图ID                         |
    public string type;//    | string | 'btc'  |
    public int day;//     | int    | 7 or 15 or 30                |
    public int timeout;// | int    | 到期时间，时间戳                     |
}

public class BuyGoodsForRentMap : MsgBase
{
    public BuyGoodsForRentMap()
    {
        method = "BuyGoodsForRentMap";
    }

    public int map_id;// | int    | 地图ID
    public string txid;//   | string | 订单号                     |
}

public class RentalBitmap : MsgBase
{
    public int bitmap_id;// | int    | 地图ID          |
    public int days; //    | int    | 7 or 15 or 30 |
    public int timeout;// | int    | 到期时间，时间戳      |
    public string owner; //   | string | 用户钱包地址        |
}

public class RentBitmap : MsgBase
{
    public RentBitmap()
    {
        method = "RentBitmap";
    }
    public int map_id;// | int    | 地图ID                          |
    public string type;//   | string | 'energy' or 'btc' or 'profit' |
    public int day;//    | int    | 7 or 15 or 30                 |
    
}
public class RentBitmapSuccess : MsgBase
{
    public int map_id;//  | int    | 地图ID                    |
    public string type;// 'energy' or 'profit' |
    public int day;//     | int    | 7 or 15 or 30           |
    public int timeout;// | int    | 到期时间，时间戳      
    public User user;
}
#endregion