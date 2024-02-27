using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using System;
using System.Net.Sockets;
using SocketIOClient.Transport;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public static class Globals
{
    public static SocketIoConnection socketIoConnection = new SocketIoConnection();
    public static object profile;
    public static object token;
    public static int[] roomIdArray = { -1, -1, -1 };
    public static Dictionary<string, object>[] rooms = { null, null, null };
    public static Dictionary<string, object> myRoom =
        new Dictionary<string, object>
        {
            { "id", -1 },
            { "leave", null },
            { "ready", null },
            { "unseat", null },
            { "fold", null },
            { "raise", null },
            { "call", null },
            { "check", null },
            { "takeseat", null },
        };
    public static string strUri = "http://192.168.148.182:3000";

}
