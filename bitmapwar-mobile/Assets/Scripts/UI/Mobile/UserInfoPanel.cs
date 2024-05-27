using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoPanel : MonoBehaviour
{
    public Text MyAddress;

    public Button BitmapListBtn;

    public Button PurchaseBtn;

    public Text MyProfitText;
    public Text CurrentBitmapTileText;
    public Text MySoldierText;

    public BitmapList bitmapListPage;

    public Button CloseBtn;

    void OnOpenBitmapPage()
    {
        bitmapListPage.gameObject.SetActive(true);
    }

    void OnPurchaseBtn()
    {
        
    }

    public void RefreshUserInfo()
    {
        MyAddress.text = Consts.GetConnectWalletLabel(Game.Inst.userMySelf.taproot_address);
        MyProfitText.text = Consts.StrToDouble(Game.Inst.userMySelf.profit);
        CurrentBitmapTileText.text = Game.Inst.myLandList.Length.ToString();
        MySoldierText.text = Game.Inst.userMySelf.virus.ToString();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        ProtoUtils.SendLoadMap(Game.Inst.MyWallet);
        BitmapListBtn.onClick.AddListener(OnOpenBitmapPage);
        PurchaseBtn.onClick.AddListener(OnPurchaseBtn);
        
        CloseBtn.onClick.AddListener(Close);

        MyAddress.text = Consts.GetConnectWalletLabel(Game.Inst.userMySelf.taproot_address);
        MyProfitText.text = Consts.StrToDouble(Game.Inst.userMySelf.profit);
        if (Game.Inst.myLandList != null)
        {
            CurrentBitmapTileText.text = Game.Inst.myLandList.Length.ToString();
            MySoldierText.text = Game.Inst.userMySelf.virus.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.Inst.myLandList != null)
        {
            CurrentBitmapTileText.text = Game.Inst.myLandList.Length.ToString();
        }
    }
}
