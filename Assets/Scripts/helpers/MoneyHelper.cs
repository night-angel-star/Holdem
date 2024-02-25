using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyHelper : MonoBehaviour
{
    public static string FormatNumberAbbreviated(long num)
    {
        if (num >= 1000000000)
        {
            return (num / 1000000000D).ToString("0.#") + "B";
        }
        if (num >= 1000000)
        {
            return (num / 1000000D).ToString("0.#") + "M";
        }
        if (num >= 1000)
        {
            return (num / 1000D).ToString("0.#") + "K";
        }
        return num.ToString();
    }
}
