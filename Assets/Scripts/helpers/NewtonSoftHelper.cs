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
        } catch (Exception ex)
        {
            Debug.Log(ex);
            return (Dictionary<T, U>)jObjectObj;
        }
        
    }

    public static int GetIndexFromJArray(Dictionary<string,object>[] array, string key, string value)
    {
        int index = -1;
        for(int i = 0; i < array.Length; i++)
        {
            object idObject = array[i][key];
            if (idObject.ToString() == value)
            {
                return i;
            }
        }
        return index;
    }
}
