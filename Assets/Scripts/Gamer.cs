using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

using JsonDictinary = System.Collections.Generic.Dictionary<string, object>;
public class Gamer
{
    public string uid = "";
    public string name = "";
    public int avatar=1;
    public int coins;
    public int score;
    public int level;
    public int exp;
    public int deposite;

    public static Gamer FromToken(JToken baseData)
    {
        return baseData.ToObject(typeof(Gamer)) as Gamer;
    }

}
