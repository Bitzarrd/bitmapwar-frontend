mergeInto(LibraryManager.library, {
    ConnectWallet: async function(message) {
        if (typeof window.unisat !== 'undefined') {
          console.log('UniSat Wallet is installed!');
          let unisat = window.unisat;
          let accounts = await window.unisat.requestAccounts();
          console.log('connect success', accounts, typeof(accounts));
          SendMessage("JSBridge", "WalletResult", JSON.stringify(accounts));
        }
        else {
            alert("UniSat Wallet is Not Installed!");
        }
    },
    
    ConnectWalletV2: async function(index)
    {
        let wallet = "unisat";
        if(index == 1) wallet = "okx";
        console.log("Connecting to Wallet Type:" + wallet);
        let result = await window.connect(wallet);
        if(result == null)
        {
            if(index == 1) 
            {
                alert("Okex Wallet is not install!");
            }
            else {
                alert("Unisat Wallet is not install!");
            }
        }
        let resstr = JSON.stringify(result);
        console.log("Connect Result: " + resstr);
        SendMessage("JSBridge", "WalletResultV2", resstr);
    },
    
    Disconnect: function()
    {
        window.disconnect();
    },
    
    BitmapExtract: async function(msg)
    {
        var strinfo = UTF8ToString(msg);
        console.log("Extract From Js:" + strinfo);
        var info = JSON.parse(strinfo);
        await window.extractProfit(info.amount, info.sig, info.nounce, info.addr);
    },
    
    JSBuySoldier: async function(count)
    {
        console.log('Buy Amount : ' + count);
        let res = await window.purchase(count);
        console.log("Buy Result:" + res);
        SendMessage("JSBridge", "OnPayResult", res);
    }
});