using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GlobalNotification : MonoBehaviour
{
    public GlobalNotification()
    {
        Globals.socketIoConnection.AddNotifyHandler("status", OnStatus);
    }

    void OnStatus(JToken baseToken)
    {
        Debug.Log(baseToken);
    }
}
