using AOT;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SocketIoWrapper
{
    [DllImport("__Internal")]
    private static extern void JsSocketIo();
    [DllImport("__Internal")]
    private static extern void JsSocketIo_Connect();

    [DllImport("__Internal")]
    private static extern void JsSocketIo_SetCallback(string gameObjectName, string callBackName);

    [DllImport("__Internal")]
    private static extern void JsSocketIo_On(string _event);

    [DllImport("__Internal")]
    private static extern void JsSocketIo_Emit(string _event, string data);

    private static readonly string SOCKET_IO_GAMEOBJECT_NAME = "SocketIoRef";
    private static readonly string SOCKET_IO_CALLBACK_NAME = "callSocketEvent";

    private SocketIoInterface _socketIoInterface;

    [MonoPInvokeCallback(typeof(Func<int>))]
    public static void OnHello(string m)
    {
        JsSocketIo_Emit("hello", JsonUtility.ToJson(new { }));
    }
    [MonoPInvokeCallback(typeof(Func<int>))]
    public static void OnEvent(string m)
    {
        Debug.Log(m);
    }

    public SocketIoWrapper()
    {
        GameObject obj = new GameObject(SOCKET_IO_GAMEOBJECT_NAME);
        _socketIoInterface = obj.AddComponent<SocketIoInterface>();
        _socketIoInterface.SetSocket(this);
        GameObject.DontDestroyOnLoad(obj);
        JsSocketIo();
        JsSocketIo_SetCallback(SOCKET_IO_GAMEOBJECT_NAME, SOCKET_IO_CALLBACK_NAME);
    }
    public void Connect()
    {
        JsSocketIo_Connect();
    }
    public void On(string _event, Action<string> callback)
    {
        Debug.Log("socketIoWrapper.On();" + _event);
        _socketIoInterface.On(_event, callback);
        JsSocketIo_On(_event);
        Debug.Log("socketIoWrapper.On(); end");
    }
    public void Emit(string _event)
    {
        JsSocketIo_Emit(_event, JsonUtility.ToJson(new { }));
    }


    public void Emit(string _event, JObject data)
    {
        /*
        if (data == null)
        {
            data = new { };
        }
        */
        //string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        string json = data.ToString();
        Debug.Log("socket is sending");
        Debug.Log(json);
        JsSocketIo_Emit(_event, json);
    }
}
