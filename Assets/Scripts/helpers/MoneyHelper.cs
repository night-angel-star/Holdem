using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyHelper : MonoBehaviour
{
    public static string FormatNumberAbbreviated(long num,int underPoint)
    {
        string sharpAmount = "";
        for(int i = 0; i < underPoint; i++)
        {
            if (i == 0)
            {
                sharpAmount = ".";
            }
            sharpAmount += "#";
        }
        if (num >= 1000000000)
        {
            return (num / 1000000000D).ToString("0"+sharpAmount) + "B";
        }
        if (num >= 1000000)
        {
            return (num / 1000000D).ToString("0"+sharpAmount) + "M";
        }
        if (num >= 1000)
        {
            return (num / 1000D).ToString("0" + sharpAmount) + "K";
        }
        return num.ToString();
    }
}
