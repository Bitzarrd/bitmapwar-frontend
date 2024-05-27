using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Numerics;
using DG.Tweening;
using FairyGUI;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class MainPage : MonoBehaviour
{
    public GameObject PrefabSparkFx;
    public GameObject SparkFx;
    public static MainPage inst;
    public Camera uiCam;
    
    private bool isLoginMenuOpened = false;
    private bool isSearchInputOnFocus = false;

    public TextAsset lang_zh_string;
    public TextAsset lang_en_string;

    public InputField chatInputCtrl;

    private string listUrl = "https://global.bitmap.game/service/open/bitmap/list";
    private GComboBox walletList;

    private GComboBox ComboPlaceMethod;
    private GTextField bitmapCount;

    private GTextInput chatInputText;
    
    public Dictionary<int, string> mySoldiers = new Dictionary<int, string>();
    private GComponent btnSubmit;
    private GTextField TurnText;
    public GTextField DurationTimer;
    private GTextField NextRoundTimer;

    private GTextInput batchInput;
    

    private GTextInput placeSoldier;
    private GTextInput tiWalletAddress;

    private GComponent actionPanel;

    private GComponent btnConnect;

    private GTextField MySoldierOfTile;

    private GTextField txtMapID;
    private GTextField JackPot;
    private GTextField TotalBonus;

    private GTextField InfoWallet;
    private GTextField InfoUserProfit;

    private bool isHelpOpen = false;

    private GComboBox loginCombo;

    private GComponent btnLogout;

    public GComponent view;

    private GComponent colorPanel;

    //My virus remaining.
    private GTextField myLeftSoldier;

    private GList lastRankingList;

    private GTextField tChoosedTile;

    private GList LastPlaceList;
    
    //Chat
    private GComponent chatPanel;

    public List<string> colorDic = new List<string>()
    {
        "red",
        "blue",
        "purple",
        "green",
    };

    private List<string> helpItem = new List<string>()
    {
        "n143",
        "n144",
        "n145",
        "n146",
    };

    public List<Message> globalMessage = new();
    public List<Message> myTeamMessage = new();

    public GList chatBubbleList;

    public int currentChannel = -1;

    private Dictionary<string, string> colorChatName = new()
    {
        { "red", "#ffff0000" },
        { "blue", "#ff0000ff" },
        { "purple", "#FF784AE2" },
        { "green", "#FFF0EB68" },
        { "global", "#FFFF961B" },
    };

    private void ComposeChatBubble(GComponent item, Message m)
    {
        var t = item.GetChildByPath("n0").asTextField;
        if (m.from != null)
        {
            var from = Consts.GetConnectWalletLabel(m.from);
            t.text = $"[color={colorChatName[m.color]}]{from}[/color]:{m.content}";
        }
    }
    public void AppendChatMessage(Message msg)
    {
        if (msg.color.Equals("global"))
        {
            globalMessage.Add(msg);
            if (currentChannel == 0)
            {
                if (msg.from != null)
                {
                    var item = chatBubbleList.AddItemFromPool().asCom;
                    ComposeChatBubble(item, msg);
                }
            }
        }
        else if (msg.color.Equals(Game.Inst.myColor))
        {
            myTeamMessage.Add(msg);
            if (currentChannel == 1)
            {
                if (msg.from != null)
                {
                    var item = chatBubbleList.AddItemFromPool().asCom;
                    ComposeChatBubble(item, msg);
                }
            }
        }
    }

    private List<GComponent> btnColors = new();

    private int currentColorIndex = 0;
    private GButton currentColorBtn;

    private GComponent tweetShareBtn;

    private GComponent topBar;
    private GComboBox helpCombo;

    private GComponent UserInfoPanel;

    private GMovieClip fightMovie;

    private void Awake()
    {
        inst = this;
    }

    public void OnUpdatePlayerSoldier(User u)
    {
        if (Game.Inst.IsUserMyself(u))
        {
            var t = UserInfoPanel.GetChildByPath("n81").asTextField;
            t.text = u.virus.ToString();
        }
    }

    public void OnUpdateLastPlacePlayer()
    {
        if(Game.Inst.joinLogs == null) return;
        LastPlaceList.numItems = 0;
        var jls = Game.Inst.joinLogs.ToArray();
        for (int i = 0; i < jls.Length; i++)
        {
            var jl = jls[i];
            var t = LastPlaceList.AddItemFromPool().asCom;
            t.GetChildByPath("n170").asTextField.text = Consts.GetConnectWalletLabel(jl.address);
            t.GetChildByPath("n171").asTextField.text = GridUtils.Inst.GetRealTimeHoursAndMinuts(jl.create_time);
        }
    }

    void Update()
    {
        var screenPos = chatInputText.LocalToGlobal(UnityEngine.Vector2.zero);
        var uiElement = chatInputCtrl.GetComponent<RectTransform>();
        screenPos.y = Screen.height - screenPos.y;
        uiElement.position= screenPos;
        //Detect when the Return key has been released
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (isSearchInputOnFocus)
            {
                doSearchWhenPressEnter();
            }
        }

        if (string.IsNullOrEmpty(Game.Inst.MyWallet))
        {
            UserInfoPanel.height = 0;
            UserInfoPanel.visible = false;
        }
        else
        {
            UserInfoPanel.height = 357;
            UserInfoPanel.visible = true;
        }
    }

    public void UpdateTotalLands(int lands)
    {
        var tl = view.GetChildByPath("n6.n109").asTextField;
        tl.text = Consts.IntToAbrevMode(lands);
    }


    public void SetDurationTime(long seconds)
    {
        if (DurationTimer != null)
        {
            var sec = seconds > 0 ? seconds : 0;
            TimeSpan time = TimeSpan.FromSeconds(sec);

            // 格式化时间
            string timeFormatted = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
            DurationTimer.text = timeFormatted;
        }
    }

    public void UpdateTurn(int turn)
    {
        TurnText.text = turn.ToString();
    }

    private List<string> helpUrl = new List<string>()
    {
        /*"https://bitzarrd.gitbook.io/bitmapwar/game-resources/soldiers",
        "https://bitzarrd.gitbook.io/bitmapwar/game-resources/factions",
        "https://bitzarrd.gitbook.io/bitmapwar/game-resources/tiles",
        "https://bitzarrd.gitbook.io/bitmapwar/game-resources/the-jackpot",
        */
        "https://bitmapwar.bitzarrd.xyz/game-resources/camps",
        "https://bitmapwar.bitzarrd.xyz/users-guide/users-guide",
        "https://bitmapwar.bitzarrd.xyz/gameplay/reward-distribution",
        "https://bitmapwar.bitzarrd.xyz/game-resources/the-jackpot",
    };

    public void CleanUp()
    {
        Game.Inst.IsLogin = false;
        Game.Inst.isLogingOut = true;
        //btnConnect.GetController("logState").SetSelectedIndex(0);
        topBar.GetController("c1").SetSelectedIndex(0);
        InfoWallet.text = "";
        InfoUserProfit.text = "0";
        walletList.title = "Connect Wallet";
        Game.Inst.MyWallet = "";
        lastRankingList.numItems = 0;
        #if UNITY_WEBGL
        JSBridge.inst.JSDisconnect();
        #endif
    }

    public void OnLogout()
    {
        WebSocketClient.inst.Close();
        CleanUp();
    }
    
    
    public void OnChooseWallet(EventContext e)
    {
        var index = int.Parse(walletList.value);
        Debug.Log("Choose Wallet" + index);
        //topBar.GetController("c1").SetSelectedIndex(1);
        ConnectWallet(index);
    }

    public void OnChooseHelp(EventContext e)
    {
        var index = int.Parse(helpCombo.value);
        //topBar.GetController("HelpMenu").SetSelectedIndex(0);
        Application.OpenURL(helpUrl[index]);
        helpCombo.title = "Help";
    }

    public void UpdateNextRoundTimer(int sec)
    {
        if (NextRoundTimer != null)
        {
            TimeSpan time = TimeSpan.FromSeconds(sec > 0 ? sec : 0);

            // 格式化时间
            string timeFormatted = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
            NextRoundTimer.text = timeFormatted;
        } 
    }

    public void UpdateRanking(Settlement data)
    {
        lastRankingList.numItems = 0;
        for(int i = 0; i < data.rank.Length; i++)
        {
            var u = data.rank[i];
            AddRanking(u, i + 1);
        }
    }

    public void UpdateLastRankingForLanguage()
    {
        if (Game.Inst.lastRanks == null) return;
        for (int i = 0; i < Game.Inst.lastRanks.Length; i++)
        {
            var u = Game.Inst.lastRanks[i];
            AddRanking(u, i + 1);
        }
    }

    public void AddRanking(LastRank r, int _rank)
    {
        var ri = lastRankingList.AddItemFromPool().asCom;
        if (_rank >= 1 && _rank <= 3)
        {
            ri.GetController("c1").SetSelectedIndex(_rank - 1);
            ri.GetChild("owner_" + _rank).asTextField.text = Consts.GetConnectWalletLabel(r.taproot_address);
            ri.GetChild("profit_" + _rank).asTextField.text = Consts.StrToDouble(r.profit);    
        }
        else if(_rank > 3)
        {
            ri.GetController("c1").SetSelectedIndex(3);
            ri.GetChild("owner_x").asTextField.text = Consts.GetConnectWalletLabel(r.taproot_address);
            ri.GetChild("profit_x").asTextField.text = Consts.StrToDouble(r.profit);
            ri.GetChild("rank").asTextField.text = _rank.ToString();
        }
    }

    public void UpdateTotalBonus(string jackpot)
    {
        TotalBonus.text = Consts.StrToDouble(jackpot);
    }

    public void UpdateJackPot(string jackpot)
    {
        JackPot.text = Consts.StrToDouble(jackpot);
    }

    public void UpdateWalletInfo(string wallet)
    {
        var res = wallet;
        if (wallet.Length > 16)
        {
            res = wallet.Substring(0, 8) + "..." + wallet.Substring(wallet.Length - 8, 8);
        }
        InfoWallet.text = res;
    }

    public void UpdateUserInfo(User u)
    {
        if (Game.Inst.IsUserMyself(u))
        {
            UpdateWalletInfo(u.taproot_address);
            myLeftSoldier.text = u.virus.ToString();
            var profit = Consts.StrToDouble(u.profit);
            InfoUserProfit.text = profit;
            Game.Inst.userMySelf = u;
        }
    }


    private GTextInput searchInput;
    private GComponent searchBtn;

    public void LerpCameraTo(string tileStr)
    {
        try
        {
            int tile = int.Parse(tileStr);
            var pos = GridUtils.Inst.GetCoordByTileId(tile);
            Debug.Log("ps: " + pos);
            var deltaPos = GridUtils.Inst.GetCenterPos(pos);
            var scenePos = GridUtils.Inst.GetTileScenePosByCoords(deltaPos);
            scenePos.z = -10f;
            Camera.main.transform.DOMove(scenePos, 1f);
            ZoomAnim.Inst.LerpZoom();
            
            MapMesh.inst.SetCursor(deltaPos);
        }
        catch (Exception e)
        {
            Debug.Log("Cannot Parse tile");
        }
    }

    public void InitViewSearchBar()
    {
        GComponent searchbar = view.GetChild("n56").asCom;

        searchBtn = searchbar.GetChild("n57").asCom;
        searchInput = searchbar.GetChildByPath("n78").asTextInput;
        searchBtn.onClick.Add(() =>
        {
            //Debug.Log("Search:{" + searchInput.text + "}");

            var address = int.Parse(searchInput.text);
            if (address <= 0 || address > GridUtils.Inst.gridSizeX * GridUtils.Inst.gridSizeY)
            {
                Toast.Inst.Open("Invalid number!");
                return;
            }
            
            try
            {
                Game.Inst.currentSelect = GridUtils.Inst.GetCoordByTileId(int.Parse(searchInput.text));
                LerpCameraTo(searchInput.text);
                RefreshTileInfo();
            }
            catch (Exception ex)
            {
                Toast.Inst.Open("Invalid number!");
            }
        });

        searchInput.onFocusIn.Add(searchInputOnFocusIn);
        searchInput.onFocusOut.Add(searchInputOnFocusOut);
    }
    private void doSearchWhenPressEnter()
    {
        GComponent searchbar = view.GetChild("n56").asCom;

        searchBtn = searchbar.GetChild("n57").asCom;

        searchInput = searchbar.GetChildByPath("n78").asTextInput;

        // Should be same as "onClick.Add" in InitViewSearchBar():
        try
        {
            Game.Inst.currentSelect = GridUtils.Inst.GetCoordByTileId(int.Parse(searchInput.text));
            LerpCameraTo(searchInput.text);
            RefreshTileInfo();
        }
        catch (Exception ex)
        {
            Toast.Inst.Open("Invalid number!");
        }

        // 按回车进行搜索时，输入框会带入回车造成的换行，导致再次搜索时报错，需要去掉:
        searchInput.text = searchInput.text.Replace("\n", "");
    }
    private void searchInputOnFocusIn()
    {
        isSearchInputOnFocus = true;
    }
    private void searchInputOnFocusOut()
    {
        isSearchInputOnFocus = false;
    }

    public void SetLoginOk(string wallet, string tap_rootAddr)
    {
        //btnConnect.GetController("logState").SetSelectedIndex(1);
        Game.Inst.MyWallet = wallet;
        loginCombo.GetChildByPath("n0.n115").asTextField.text = Consts.GetConnectWalletLabel(tap_rootAddr);
        ProtoUtils.SendLoadMap(wallet);
        
    }

    public void UpdateBitmapsCount(int count)
    {
        bitmapCount.text = count.ToString();
    }

    public void RefreshLanguage(int lang = 0)
    {
        UIPackage.RemoveAllPackages();

        Game.Inst.langIndex = lang;
        
        GRoot.inst.RemoveChildren();

        if (lang == 1)
        {
            FairyGUI.Utils.XML xml = new FairyGUI.Utils.XML(lang_zh_string.text);
            UIPackage.SetStringsSource(xml);
            UIConfig.defaultFont = "SimSun";
        }
        else
        {
            FairyGUI.Utils.XML xml = new FairyGUI.Utils.XML(lang_en_string.text);
            UIPackage.SetStringsSource(xml);
            UIConfig.defaultFont = "Roboto";
        }
        
        UIPackage.AddPackage("bitmapwar-web1-1920");
        UIPackage.AddPackage("bitmapwar-web2-1920");
        UIPackage.AddPackage("bitmapwar-web5-1920");
        UIPackage.AddPackage("bitmapwar-web7-1920");
        UIPackage.AddPackage("bitmapwar-error-tips-1920");
        
        makePage();
        
        Game.Inst.AutoLogin();
    }

    private bool isOpenChat = false;
    //private GTextInputEx_WebGL chatInp;

    private void SendChat()
    {
        if (chatInputCtrl.text.Length == 0)
            return;
        if (!Game.Inst.IsLogin)
        {
            Toast.Inst.Open("Please Login");
            //Debug.Log("Not Login Cannot Chat");
            return;
        }
        var colorStr = "global";

        if (currentChannel == 1)
        {
            colorStr = Game.Inst.myColor;
            var msg = new SendChatMessage()
            {
                color = "team",
                content = chatInputCtrl.text
            };


            WebSocketClient.inst.Send(JsonConvert.SerializeObject(msg));
        }
        else
        {
            var msg = new SendChatMessage()
            {
                color = "global",
                content = chatInputCtrl.text
            };

            WebSocketClient.inst.Send(JsonConvert.SerializeObject(msg));
        }
        
        chatInputCtrl.text = "";
    }

    public void OnChooseChannel(int index)
    {
        if (currentChannel == index)
        {
            return;
        }
        currentChannel = index;
        chatBubbleList.numItems = 0;
        List<Message> currentMsg;
        if (index == 0)
        {
            currentMsg = globalMessage;
        }
        else {
            currentMsg = myTeamMessage;
        }

        for (int i = 0; i < currentMsg.Count; i++)
        {
            if (currentMsg[i].from != null)
            {
                var item = chatBubbleList.AddItemFromPool().asCom;
                ComposeChatBubble(item, currentMsg[i]);
            }
        }
    }

    public Canvas canvas;
    
    public UnityEngine.Vector2 WorldToUI(RectTransform uiElement, Vector3 screenPosition)
    {
        Debug.Log("Screen pos is :" + screenPosition);
        UnityEngine.Vector2 uiPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiElement, screenPosition, Camera.main, out uiPosition);
        uiPosition.y *= -1;
        return uiPosition;
    }


    private void makePage()
    {
        view = UIPackage.CreateObject("bitmapwar-web1-1920", "bitmapwar-web1-1920").asCom;
        //view.scrollPane.mouseWheelEnabled = false;

        chatInputCtrl.gameObject.SetActive(false);

        colorPanel = view.GetChildByPath("n8.n104").asCom;

        chatPanel = view.GetChildByPath("n172").asCom;
        chatBubbleList = chatPanel.GetChildByPath("n175").asList;
        view.GetChildByPath("n178").asCom.onClick.Add(() =>
        {
            isOpenChat = !isOpenChat;
            if (isOpenChat)
            {
                chatPanel.visible = true;
                var screenPos = chatInputText.LocalToGlobal(UnityEngine.Vector2.zero);
                chatInputCtrl.gameObject.SetActive(true);
                Debug.Log("Screen Pos is :" + screenPos);
                var uiElement = chatInputCtrl.GetComponent<RectTransform>();
                screenPos.y = Screen.height - screenPos.y;
                uiElement.position= screenPos;
                //UpdatePosition(chatInputCtrl, screenPos);
                OnChooseChannel(0);
            }
            else
            {
                chatInputCtrl.gameObject.SetActive(false);
                chatPanel.visible = false;
            }
        });

        chatInputText = chatPanel.GetChildByPath("n176").asTextInput;
        chatInputText.onFocusIn.Add(() => Debug.Log("Show Input Text..."));
        //chatInp = new GTextInputEx_WebGL(chatInputText);
        chatPanel.GetChildByPath("n182").asCom.onClick.Add(() => OnChooseChannel(0));
        chatPanel.GetChildByPath("n183").asCom.onClick.Add(() => OnChooseChannel(1));
        
        view.GetChildByPath("n172.n177").asCom.onClick.Add(SendChat);

        LastPlaceList = view.GetChildByPath("n166.n169").asList;
        
        actionPanel = view.GetChild("n8").asCom;
        actionPanel.visible = false;
        UserInfoPanel = view.GetChild("n6").asCom;

        GGraph holder = view.GetChild("n165").asGraph;
        if (SparkFx != null)
        {
            DestroyImmediate(SparkFx);
            SparkFx = null;
        }

        SparkFx = Instantiate(PrefabSparkFx);
        SparkFx.transform.localPosition = Vector3.zero;
        SparkFx.transform.localScale = Vector3.one * 100;
        GoWrapper wrapper = new GoWrapper(SparkFx);
        holder.SetNativeObject(wrapper);
        
        ComboPlaceMethod = actionPanel.GetChildByPath("n122").asComboBox;
        
        /*
        btnConnect = view.GetChildByPath("n58.n158").asCom;
        btnConnect.onClick.Add(ConnectWallet);
        */
        bitmapCount = UserInfoPanel.GetChildByPath("n86").asTextField;
        
        //Wallet List
        walletList = view.GetChildByPath("n58.n161").asComboBox;
        walletList.onChanged.Add(OnChooseWallet);
        InfoWallet = view.GetChildByPath("n6.n74").asTextField;
        InfoUserProfit = view.GetChildByPath("n6.n75").asTextField;

        fightMovie = view.GetChildByPath("n161").asMovieClip;

        myLeftSoldier = view.GetChildByPath("n6.n81").asTextField;
        
        txtMapID = view.GetChildByPath("n8.n105").asTextField;
        TurnText = view.GetChildByPath("n61.n65").asTextField;
        
        TotalBonus = view.GetChild("n132").asTextField;
        JackPot = view.GetChild("n120").asTextField;
        
        DurationTimer = view.GetChildByPath("n61.n62").asTextField;
        NextRoundTimer = view.GetChildByPath("n61.n63").asTextField;
        
        
        
        var btnbmp = view.GetChildByPath("n6.n90").asCom;
        btnbmp.onClick.Add(() =>
        {
            BitmapListPage.Inst.Open();
        });

        loginCombo = view.GetChildByPath("n58.n60").asComboBox;
        loginCombo.onChanged.Add(OnChooseLoginMenu);
        
        //Share
        tweetShareBtn = view.GetChildByPath("n6.n88").asCom;
        tweetShareBtn.onClick.Add(() =>
        {
            Game.Inst.PostToTwitter("I just received a free soldier\ud83e\udd3a in #BitmapWar and am ready to engage in battles with other players on the #Bitmap. \ud83d\ude80");
        });
        

        
        
        
        //Color Button
        btnColors.Add(view.GetChildByPath("n8.n104.n100").asCom);
        btnColors.Add(view.GetChildByPath("n8.n104.n101").asCom);
        btnColors.Add(view.GetChildByPath("n8.n104.n102").asCom);
        btnColors.Add(view.GetChildByPath("n8.n104.n103").asCom);
        
        for (int i = 0; i < btnColors.Count; i++)
        {
            var b = btnColors[i];
            int t = i;
            b.onClick.Add(() =>
            {
                OnChooseColor(t);
            });
        }

        //Place Soldier
        placeSoldier = view.GetChildByPath("n8.n99.n111").asTextInput;
        placeSoldier.singleLine = true;
        btnSubmit = view.GetChildByPath("n8.n84").asCom;
        btnSubmit.onClick.Add(Submit);
        
        UserInfoPanel.GetChildByPath("n105").asCom.onClick.Add(()=> {RentPage.Inst.Open();});
        
        //HelpBtn:
        helpCombo = view.GetChildByPath("n58.n164").asComboBox;
        helpCombo.onChanged.Add(OnChooseHelp);
        
        //TopBar
        topBar = view.GetChild("n58").asCom;
        
        topBar.GetChildByPath("n167").asCom.onClick.Add((() =>
        {
            if(Game.Inst.langIndex == 0) RefreshLanguage(1);
            else RefreshLanguage(0);
        }));
        
        topBar.GetChildByPath("n166").asCom.onClick.Add((() =>
        {
            if (!Game.Inst.IsLogin)
            {
                Toast.Inst.Open("To view the profit history., please log in.");
                return;
            }
                
            ProfitHistoryPage.Inst.Open();
        }));
        
        //leaderPage:
        topBar.GetChildByPath("n162").asCom.onClick.Add(() =>
        {
            TotalRanking.Inst.Open();
        });
        
        topBar.GetChildByPath("n165").asCom.onClick.Add((() =>
        {
            LogPage.Inst.Open();
        }));
        
        //My Soldier of Tiles;
        MySoldierOfTile = view.GetChildByPath("n8.n109").asTextField;
        
        //Back to  BitmapGame
        view.GetChildByPath("n58.n59").asCom.onClick.Add(() =>
        {
            Application.OpenURL("https://bitmap.game/");
        });
        
        //Tweeter
        view.GetChildByPath("n58.n114").asCom.onClick.Add(() =>
        {
            //Application.OpenURL("https://twitter.com/BitmapWar");
            Application.OpenURL("https://twitter.com/Bitzarrd");
        });

        // Mute Btn:
        var MuteBtnContainer = view.GetChildByPath("n58.n159");
        var MuteBtnIconSound = view.GetChildByPath("n58.n159.n115");
        var MuteBtnIconMute = view.GetChildByPath("n58.n159.n116");
        MuteBtnIconMute.visible = false; // 默认只显示Sound按钮
        MuteBtnContainer.asCom.onClick.Add(() =>
        {
            GameObject audioObj = GameObject.Find("/AudioBG");

            AudioSource audioSource = audioObj.GetComponent<AudioSource>();

            audioSource.mute = !audioSource.mute;

            if (audioSource.mute)
            {
                MuteBtnIconSound.visible = false;
                MuteBtnIconMute.visible = true;
            }
            else
            {
                MuteBtnIconSound.visible = true;
                MuteBtnIconMute.visible = false;
            }
        });

        //tg
        view.GetChildByPath("n58.n156").asCom.onClick.Add(() =>
        {
            Application.OpenURL("https://t.co/3W7BOBOzeY");
        });

        lastRankingList = view.GetChildByPath("n4.n74").asList;
        lastRankingList.scrollPane.onScroll.Add(((e) => Debug.Log("Scrolling ranking")));

        InitViewSearchBar();
        
        UpdateLastRankingForLanguage();
        OnUpdateLastPlacePlayer();
        
        //Get profit
        view.GetChildByPath("n6.n80").asCom.onClick.Add(
        () =>
        {
            Application.OpenURL(Game.Inst.PCMainPageHTML);
            //ExtractProfitPage.Inst.Open();
        });
        
        view.GetChildByPath("n6.n83").asCom.onClick.Add(
        () =>
        {
            Application.OpenURL(Game.Inst.PCMainPageHTML);
            //BuySoldierPage.Inst.Open();
        });
        GRoot.inst.AddChild(view);
        view.MakeFullScreen();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        //uiCam.orthographicSize = 9f;
        UIPackage.AddPackage("bitmapwar-web1-1920");
        UIPackage.AddPackage("bitmapwar-web2-1920");
        UIPackage.AddPackage("bitmapwar-web5-1920");
        UIPackage.AddPackage("bitmapwar-error-tips-1920");
        //UIPackage.AddPackage("bitmapwar-web-panels");
        //UIPackage.AddPackage("bitmapwar-web1-1080p");
        
        
        
        //view = UIPackage.CreateObject("bitmapwar-web1-1080p", "bitmapwar-web1-1080p").asCom;
        //makePage();
        RefreshLanguage(0);
    }

    public void PlayGameStartAnim()
    {
        var m = fightMovie;
        m.frame = 0;
        m.playing = true;
        m.visible = true;
        m.SetPlaySettings(0, -1, 1, -1);
        
        m.onPlayEnd.Add(() =>
        {
            m.visible = false;
            m.playing = false;
        });
    }

    public void OnSetChooseTile(Info tileInfo)
    {
        tChoosedTile.text = tileInfo.info.bitmap_id;
    }

    private void OnChooseColor(int index)
    {
        if (Game.Inst.langIndex != 0) index -= 4;
        var cc = colorDic[index];
        if (!string.IsNullOrEmpty(Game.Inst.turnColor))
        {
            if(!cc.Equals(Game.Inst.turnColor))
            {
                return;
            }
        }
        currentColorIndex = index;
        colorPanel.GetController("c1").SetSelectedIndex(index + 1);
        Debug.Log("On Choose Color:" + index);
        Game.Inst.myColor = colorDic[index];
    }

    public void SetColor(string col)
    {
        for (int i = 0; i < colorDic.Count; i++)
        {
            if (col.Equals(colorDic[i]))
            {
                currentColorIndex = i;
                colorPanel.GetController("c1").SetSelectedIndex(i + 1);
            }
        }
    }

    public void RandomColor()
    {
        //Random Color
        int rc = Random.Range(0, 4);
        Game.Inst.myColor = colorDic[rc];
        view.GetChildByPath("n8.n104").asCom.GetController("c1").SetSelectedIndex(rc + 1);
    }

    public void ConnectWallet(int index = 0)
    {
        Debug.Log("Click Connect");
#if UNITY_WEBGL && !UNITY_EDITOR
        JSBridge.inst.ConnectToWallet(index);
#else
        ProtoUtils.SendLogin("02b13a59a27e6268117b1abc19f1b147f56bb65f89136ec574457a7466401d6652");
#endif
    }

    public void OnLogin()
    {
        Game.Inst.isLogingOut = false;
        topBar.GetController("c1").SetSelectedIndex(1);
    }

    public void UpdateStatatics(Statistics s)
    {
        var cn = Consts.colorName[s.team];
        var landStr = "n2." + cn + "1";
        var SoldierStr = "n2." + cn + "2";
        var lostStr = "n2." + cn + "3";
        view.GetChildByPath(landStr).asTextField.text = s.land.ToString();
        view.GetChildByPath(SoldierStr).asTextField.text = s.virus.ToString();
        view.GetChildByPath(lostStr).asTextField.text = s.loss.ToString();
    }

    public void RefreshTileInfo()
    {
        var id = GridUtils.Inst.gridSizeX * Game.Inst.currentSelect.y + Game.Inst.currentSelect.x;
        txtMapID.text = id.ToString();
        if (Game.Inst.IsTileMine(id.ToString()))
        {
            actionPanel.GetChildByPath("n107").asTextField.text = Consts.GetConnectWalletLabel(Game.Inst.MyTapRootWallet);
            MySoldierOfTile.text = Game.Inst.GetSoldilerOfTile(Game.Inst.MyWallet, id.ToString()).ToString();
            actionPanel.visible = true;
        }
        else
        {
            actionPanel.visible = false;
        }
    }

    public void OnClickBatchPlace()
    {
        
    }

    public void Submit()
    {
        Debug.Log("On Submit" + placeSoldier.text + " Combo: " + ComboPlaceMethod.value);
        if (string.IsNullOrEmpty(Game.Inst.MyWallet))
        {
            Toast.Inst.Open("You Must Be Logged in.");
            return;
        }
        
        var id = GridUtils.Inst.gridSizeX * Game.Inst.currentSelect.y + Game.Inst.currentSelect.x;
        if(string.IsNullOrEmpty(Game.Inst.turnColor))
        {
            //Game.Inst.turnColor = Game.Inst.myColor;
        }
        else
        {
            if(!Game.Inst.turnColor.Equals(Game.Inst.myColor))
            {
                //Toast.Inst.Open("You have selected a faction and cannot choose another faction in this round of battle");
                //return;
            }
        }
        //Game.Inst.turnColor = Game.Inst.myColor;
        if (Game.Inst.myColor.Equals("none"))
        {
            Toast.Inst.Open("Please click on the color to select the camp, otherwise soldiers cannot be placed.");
            return;
        }

        var amount = int.Parse(placeSoldier.text);
        if (amount <= 0)
        {
            Toast.Inst.Open("Invalid number!");
            return;
        }
        PlaceSoldierPage.Inst.Open(id, amount, Game.Inst.myColor, int.Parse(ComboPlaceMethod.value));
    }

    void OnChooseLoginMenu(EventContext e)
    {
        Debug.Log("Choose LoignMenu: " + loginCombo.value);
        actionPanel.visible = false;
        OnLogout();
    }
}