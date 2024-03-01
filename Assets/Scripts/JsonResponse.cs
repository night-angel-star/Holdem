using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

using JsonResponseT = System.Collections.Generic.Dictionary<string, object>;
using System.Reflection;
using System;

public class JsonResponse
{
    JToken jToken;

    public static JsonResponseT ToDictionary(object token)
    {
        if (token.ConvertTo<JToken>().Type != JTokenType.Object) 
        {
            return null;
        }
        JsonResponseT keyValuePairs = JsonConvert.DeserializeObject<JsonResponseT>(token.ToString());

        return keyValuePairs;
    }

    public static object[] ToArray(object token)
    {
        JToken jToken = token.ConvertTo<JToken>();
        if (jToken.Type != JTokenType.Array)
        {
            return null;
        }
        object[] arr = jToken.ToArray<object>();

        return arr;
    }

    public static T ConvertTo<T>(object token) 
    {
        return token.ConvertTo<JToken>().Value<T>();
    }

    public static void SetObjectValue(JToken token, object obj, string[] props)
    {
        for (int i = 0; i < props.Length; i++)
        {
            JToken t = token.SelectToken(props[i]);
            if (t == null)
                continue;
            
            FieldInfo property = obj.GetType().GetField(props[i]);
            if (property == null)
                continue;
            if (property.FieldType == typeof(string) && (t.Type == JTokenType.String || t.Type == JTokenType.Integer || t.Type == JTokenType.Float))
            {
                property.SetValue(obj, t.Value<string>());
            }
            if (property.FieldType == typeof(int) && t.Type == JTokenType.Integer)
            {
                property.SetValue(obj, t.Value<int>());
            }
            if (property.FieldType == typeof(bool) && t.Type == JTokenType.Boolean)
            {
                property.SetValue(obj, t.Value<bool>());
            }
        }
    }
}
