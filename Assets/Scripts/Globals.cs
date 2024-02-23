using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using System;
using System.Net.Sockets;
using SocketIOClient.Transport;
using Newtonsoft.Json;

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
    public string strUri = "http://192.168.145.195:3000";

    public Globals()
    {
        Debug.Log("Globals");
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
            Debug.Log("Hello" + data);
            socketIoUnity.Emit("hello");

            System.Random rnd = new System.Random();
            int seq = rnd.Next();
            var req = new
            {
                seq = seq,
                
                f = "login",
                args = new
                {
                    uid = "asd",
                    passwd = "123"
                }                
                /*
                f = "signup",
                args = new
                {
                    uid = "ccc",
                    passwd = "ccc",
                    email = "ccc@ccc.ccc"
                }
                */
            };
            
            string jsonData = JsonConvert.SerializeObject(req);
            socketIoUnity.Emit("rpc", req);
        });

        socketIoUnity.On("notify", SocketIoUnity_OnNotify);
        socketIoUnity.On("rpc_ret", SocketIoUnity_OnRpcRet);
        socketIoUnity.OnConnected += SocketIoUnity_OnConnected;
        ///// reserved socketio events
        socketIoUnity.Connect();
    }

    private void SocketIoUnity_OnConnected(object sender, EventArgs e)
    {
        Debug.Log($"Connected: {socketIoUnity.Connected}");
        throw new NotImplementedException();
    }

    private void SocketIoUnity_OnRpcRet(SocketIOResponse data)
    {
        Debug.Log(data);
    }

    private void SocketIoUnity_OnNotify(SocketIOResponse data)
    {

    }
}
