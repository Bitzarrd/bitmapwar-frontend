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
        let txid = await window.extractProfit(info.amount, info.sig, info.nounce, info.addr);
        let infoStr = {};
        infoStr.txid = txid;
        infoStr.id = info.nounce;
        SendMessage("JSBridge", "OnExtractResponse", JSON.stringify(infoStr));
    },
    
    RetryExtract: async function(msg)
    {
        var strinfo = UTF8ToString(msg);
        console.log("ReExtract From Js:" + strinfo);
        var info = JSON.parse(strinfo);
        let txid = await window.extractProfit(info.amount, info.sig, info.nounce, info.addr);
        let infoStr = {};
        infoStr.txid = txid;
        infoStr.id = info.nounce;
        SendMessage("JSBridge", "OnRetryExtract", JSON.stringify(infoStr));
    },
    
    Rent : async function(id, days)
    {
        console.log("Rent: " + id + " days: " + days)
        let res = await window.rentMap(id ,days);
        let rentInfo = {}
        rentInfo.txid = res;
        rentInfo.map_id = id;
        SendMessage("JSBridge", "OnRent", JSON.stringify(rentInfo));
    },
    
    GetBalance : async function()
    {
        let res = await window.getBalance();
        SendMessage("JSBridge", "OnBalance", res);
        return res;
    },
    
    JSBuySoldier: async function(count)
    {
        console.log('Buy Amount : ' + count);
        try {
        let res = await window.purchase(count);
        console.log("Buy Result:" + res);
        SendMessage("JSBridge", "OnPayResult", res);
        }
        catch(e)
        {
            SendMessage("JSBridge", "OnToast", e.data.extraMessage.message);
        }
        
    }
});