using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewtonSoftHelper : MonoBehaviour
{
    public static T[] JArrayToArray<T>(object jArrayObj)
    {
        try
        {
            JArray jArray = jArrayObj as JArray;
            T[] array = jArray.ToObject<T[]>();
            return array;
        } catch (Exception)
        {
            return jArrayObj as T[];
        }
    }

    public static Dictionary<T, U> JObjectToObject<T,U>(object jObjectObj)
    {
        try
        {
            JObject jObject = jObjectObj as JObject;
            Dictionary<T, U> dic = jObject.ToObject<Dictionary<T, U>>();
            return dic;
        } catch (Exception)
        {
            return jObjectObj as Dictionary<T, U>;
        }
        
    }

    public static int GetIndexFromJArray(Dictionary<string,object>[] array, string key, string value)
    {
        int index = -1;
        for(int i = 0; i < array.Length; i++)
        {
            if (array[i][key].ToString() == value)
            {
                return i;
            }
        }
        return index;
    }
}
