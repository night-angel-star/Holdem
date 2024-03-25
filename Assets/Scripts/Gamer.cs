using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gamer
{
    public string uid = "";
    public string name = "";
    public string avatar = "0";
    public int coins;
    public int score;
    public int level;
    public int exp;
    public int deposite;
    public int[] cards;
    public int prize;
    public string phoneNumber = "";

    public static Gamer FromToken(JToken baseData)
    {
        return baseData.ToObject(typeof(Gamer)) as Gamer;
    }

}
