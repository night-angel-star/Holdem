using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class SocketManager : MonoBehaviour
{

    public static SocketIOUnity socket;
    public Uri uri; 
    // Start is called before the first frame update
    void Start()
    {
        uri = new Uri("http://192.168.145.195:3000");
        //uri = new Uri("http://192.168.148.181:3000");
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    {"token", "UNITY" }
                }
            ,
            EIO = 3
            ,
            Transport = SocketIOClient.Transport.TransportProtocol.Polling
        });
        socket.JsonSerializer = new NewtonsoftJsonSerializer();

        socket.On("hello", (data) => {
            print("data");
            socket.Emit("hello");
        });

        socket.On("notify", (data) => {
            print(data);
        });

        ///// reserved socketio events
        socket.OnConnected += (sender, e) =>
        {
            //print("socket.OnConnected");
            Debug.Log("socket connected");
        };

        Debug.Log("connecting");
        socket.Connect();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        if (socket != null)
        {
            socket.Disconnect(); // Clean up the socket when the GameObject is destroyed
        }
    }
}
