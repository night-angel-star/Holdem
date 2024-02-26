using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewtonSoftHelper : MonoBehaviour
{
    public static T[] JArrayToArray<T>(object jArrayObj)
    {
        JArray jArray = jArrayObj as JArray;
        T[] array = jArray.ToObject<T[]>();
        return array;
    }

    public static Dictionary<T, U> JArrayToObject<T,U>(object jObjectObj)
    {
        JObject jObject = jObjectObj as JObject;
        Dictionary<T,U> dic = jObject.ToObject<Dictionary<T,U>>();
        return dic;
    }
}
