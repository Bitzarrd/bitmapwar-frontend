using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Consts
{
    public static List<string> colors = new List<string>()
    {
        "red",
        "blue",
        "purple",
        "green",
    };

    public static Dictionary<string, int> colorDic = new Dictionary<string, int>()
    {
        { "red", 1 },
        { "blue", 2 },
        { "purple", 3 },
        { "green", 4 },
    };
    
    public static Dictionary<string, string> colorName = new Dictionary<string, string>()
    {
        {"red", "r"},
        {"blue", "b"},
        {"purple", "p"},
        {"green", "y"},
    };

    public static string GetConnectWalletLabel(string w)
    {
        return w.Substring(0, 4) + "..." + w.Substring(w.Length - 4, 4);
    }

    public static string StrToDouble(string str, string format = "0.######")
    {
        var myprof = (double)BigInteger.Parse(str) / (double)1e18;
        var res = myprof.ToString(format);
        return res;
    }

    private static List<string> units = new List<string>()
    {
        "K", "M", "B", "AA", "AB", "AC", "BA", 
        "BB", "BC", "CA", "CB", "CC"
    };

    public static string GetDaysByStamp(int timeout)
    {
        long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        long timeDifference = timeout - currentTimestamp;
        long days = timeDifference / (60 * 60 * 24);
        int hours = (int)((timeDifference - days * (60.0 * 60 * 24)) / (60.0f * 60));

        return days + "d " + hours + "h";
    }
    
    public static string IntToAbrevMode(int amount)
    {
        string res = amount.ToString();
        if (amount < 1000)
        {
            return res;
        }

        for (int i = 0; i < units.Count; i++)
        {
            double r = Math.Pow(1000, i + 1);
            double d = amount / r;
            Debug.Log("D is " + d + " R is : " + r);
            if (d < 1000)
            {
                res = d.ToString("0.#") + units[i];
                return res;
            }
        }

        return "NAN";
    }
    
    public static string GetYMDHMSFromTime(int time)
    {
        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(time).LocalDateTime;
        string formattedDateTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        
        return formattedDateTime;
    }
}
