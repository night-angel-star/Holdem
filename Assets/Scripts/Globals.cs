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

public class SocketIoEventHandler
{
    public string eventName;
    public Action<SocketIOResponse> handler;

    public SocketIoEventHandler(string e, Action<SocketIOResponse> callback)
    {
        eventName = e;
        handler = callback;
    }
}
public class Globals
{
    public static SocketIOUnity socketIoUnity;
    public static object profile;
    public static object token;
    public string strUri = "http://192.168.148.182:3000";

    public Globals()
    {
        Debug.Log("global initializing");
        var uri = new Uri(strUri);
        socketIoUnity = new SocketIOUnity(
            uri,
            new SocketIOOptions
            {
                EIO = 3,
                Transport = SocketIOClient.Transport.TransportProtocol.Polling
            }
        );
        
        socketIoUnity.JsonSerializer = new NewtonsoftJsonSerializer();
        socketIoUnity.On("hello", (data) => {
            Debug.Log("Hello");
            Debug.Log(data);
            socketIoUnity.Emit("hello");
        });

        socketIoUnity.On("notify", SocketIoUnity_OnNotify);
        // socketIoUnity.On("rpc_ret", SocketIoUnity_OnRpc);
        socketIoUnity.OnConnected += SocketIoUnity_OnConnected;
        ///// reserved socketio events
        socketIoUnity.Connect();
    }

    private void SocketIoUnity_OnConnected(object sender, EventArgs e)
    {
        Debug.Log($"Connected: {socketIoUnity.Connected}");
        throw new NotImplementedException();
    }

    private void SocketIoUnity_OnRpc(SocketIOResponse data)
    {
        Debug.Log("rpc_ret");
        Debug.Log(data);
    }

    private void SocketIoUnity_OnNotify(SocketIOResponse data)
    {
        // Debug.Log("notify");
        // Debug.Log(data);
    }
}
