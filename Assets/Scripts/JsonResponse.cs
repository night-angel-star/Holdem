using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

public class JsonResponse
{
    JToken jToken;

    public static Dictionary<string, object> ToDictionary(object token)
    {
        if (token.ConvertTo<JToken>().Type != JTokenType.Object) 
        {
            return null;
        }
        Dictionary<string, object> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, object>>(token.ToString());

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


}
