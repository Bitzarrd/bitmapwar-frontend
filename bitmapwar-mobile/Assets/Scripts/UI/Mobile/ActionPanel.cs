using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionPanel : MonoBehaviour
{
    private int colorChoosed = -1;
    public List<Button> colorBtns;
    public List<Image> colorSelects;
    public Button submitBtn;
    public Button submitAllBtn;
    public TMP_InputField inpNum;

    public Text BitmapAddr;
    public Text Owner;
    public Text VirusOfTile;
    public int mapId;

    public Button AddBtn;
    public Button MinusBtn;
    public Slider slider;
    public Text numText;
    public int SoldierPlacedNum;
    public Button CloseBtn;
    
    public void RefreshNumText()
    {
        numText.text = SoldierPlacedNum.ToString();
    }

    public void OnSubmitAll()
    {
        if (colorChoosed == -1)
        {
            Debug.Log("Please Choose Color");
            return;
        }

        int num = SoldierPlacedNum;
        string playerColor = Consts.colors[colorChoosed];

        ProtoUtils.SendBatchPlace(num, playerColor);
        
        gameObject.SetActive(false);

    }

    public void OnSubmit()
    {
        if (colorChoosed == -1)
        {
            Debug.Log("Please Choose Color");
            Toast.Inst.Open("Please click on the color to select the camp, otherwise soldiers cannot be placed.");
            return;
        }

        int num = SoldierPlacedNum;
        string playerColor = Consts.colors[colorChoosed];
        ProtoUtils.SendPlaceSoldier(mapId, num, playerColor);
        
        Debug.Log("OnSubmit");
        gameObject.SetActive(false);
    }

    public void Open(int tileId)
    {
        mapId = tileId;
        gameObject.SetActive(true);
        BitmapAddr.text = tileId.ToString();
        Owner.text = Consts.GetConnectWalletLabel(Game.Inst.MyTapRootWallet);
        VirusOfTile.text = Game.Inst.GetSoldilerOfTile(Game.Inst.MyWallet, tileId.ToString()).ToString();
    }

    public void OnChooseColor(Button btn, int ci)
    {
        if (!string.IsNullOrEmpty(Game.Inst.turnColor))
        {
            Debug.Log("Turn Color is :" + Game.Inst.turnColor);
            colorChoosed = Consts.colorDic[Game.Inst.turnColor] - 1;
            if (colorChoosed >= 0)
            {
                colorSelects[colorChoosed].gameObject.SetActive(false);
            }
            colorSelects[colorChoosed].gameObject.SetActive(true);
            return;
        }

        if (colorChoosed >= 0)
        {
            colorSelects[colorChoosed].gameObject.SetActive(false);
        }
        Debug.Log("Clicked : " + ci);
        colorChoosed = ci;
        colorSelects[colorChoosed].gameObject.SetActive(true);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener(OnSlideVirus);
        AddBtn.onClick.AddListener(OnAdd);
        MinusBtn.onClick.AddListener(OnMinus);
        submitBtn.onClick.AddListener(OnSubmit);
        submitAllBtn.onClick.AddListener(OnSubmitAll);
        for (int i = 0; i < colorBtns.Count; i++)
        {
            var t = i;
            var btn = colorBtns[i];
            btn.onClick.AddListener(() =>
            {
                OnChooseColor(btn, t);
            });
        }
        
        CloseBtn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
        
    }

    private void OnMinus()
    {
        int maxVal = Game.Inst.userMySelf.virus > 1000 ? 1000 : Game.Inst.userMySelf.virus;
        if (SoldierPlacedNum > 0)
        {
            SoldierPlacedNum--;
            RefreshNumText();
            slider.value = SoldierPlacedNum / (float)maxVal;
        }
    }

    private void OnAdd()
    {
        int maxVal = Game.Inst.userMySelf.virus > 1000 ? 1000 : Game.Inst.userMySelf.virus;
        if (SoldierPlacedNum < maxVal)
        {
            SoldierPlacedNum++;
            RefreshNumText();
            slider.value = SoldierPlacedNum / (float)maxVal;
        }
    }

    private void OnSlideVirus(float val)
    {
        int maxVal = Game.Inst.userMySelf.virus > 1000 ? 1000 : Game.Inst.userMySelf.virus;
        SoldierPlacedNum = (int)(maxVal * val);
        RefreshNumText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
