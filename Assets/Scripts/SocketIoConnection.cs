using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Dynamic;
using static UnityEditor.Progress;

public class SocketIoConnection
{
    public static SocketIOUnity socketIoUnity;
    public string serverUri;
    Dictionary<string, Action<JToken>> _rpcEventHandlers = new Dictionary<string, Action<JToken>>();
    Dictionary<string, bool> _rpcEventTimes = new Dictionary<string, bool>();
    private System.Random rnd = new System.Random();

    private bool SafeConnect()
    {
        // Hello Handshack
        socketIoUnity.On("hello", (data) => {
            // Debug.Log("Hello");
            socketIoUnity.Emit("hello");
        });
        socketIoUnity.On("notify", OnNotify);
        socketIoUnity.On("rpc_ret", OnRpcRet);
        socketIoUnity.OnConnected += OnConnected;
        socketIoUnity.Connect();
        return true;
    }
    public bool Connect()
    {
        if (socketIoUnity == null)
        {
            var uri = new Uri(serverUri);
            SocketIOOptions options = new SocketIOOptions
            {
                EIO = 3,
                Transport = SocketIOClient.Transport.TransportProtocol.Polling
            };
            socketIoUnity = new SocketIOUnity(uri, options);
            socketIoUnity.JsonSerializer = new NewtonsoftJsonSerializer();
            SafeConnect();
        }
        else
        {
            if (!socketIoUnity.Connected)
            {
                SafeConnect();
            } else
            {
                return true;
            }
        }

        return true;
    }

    private void OnConnected(object sender, EventArgs e)
    {
        Debug.Log($"Connected: {socketIoUnity.Connected}");
        throw new NotImplementedException();
    }

    private void ProcessRpcRet(SocketIOResponse response)
    {
        IEnumerable e = JsonConvert.DeserializeObject(response.ToString()) as IEnumerable;
        object[] responseArray = e.Cast<object>().ToArray();
        if (responseArray != null)
        {
            for (int i = 0; i < responseArray.Length; i++)
            {
                JContainer jContainer = (JContainer)responseArray[i];
                if (jContainer == null)
                {
                    Debug.Log("Invalid rpc response (JContainer): " + responseArray[i].ToString());
                    continue;
                }
                    
                JToken jToken = jContainer.SelectToken("seq");
                if (jToken == null)
                {
                    Debug.Log("Invalid rpc response (jToken): " + jContainer.ToString());
                    continue;
                }
                string seq = jToken.Value<string>();
                if (_rpcEventHandlers.ContainsKey(seq))
                {

                    _rpcEventHandlers[seq](jContainer);
                    if (_rpcEventTimes.ContainsKey(seq) && _rpcEventTimes[seq] == true)
                    {
                        _rpcEventHandlers.Remove(seq);
                        _rpcEventTimes.Remove(seq);
                    }
                } 
            }
        }
    }
    private void OnRpcRet(SocketIOResponse response)
    {
        Debug.Log("rpc_ret");
        Debug.Log(response);
        ProcessRpcRet(response);

    }
    private void OnNotify(SocketIOResponse response)
    {
        IEnumerable e = JsonConvert.DeserializeObject(response.ToString()) as IEnumerable;
        object[] responseArray = e.Cast<object>().ToArray();
        if (responseArray != null)
        {
            for (int i = 0; i < responseArray.Length; i++)
            {
                JContainer jContainer = (JContainer)responseArray[i];
                if (jContainer == null)
                {
                    Debug.Log("Invalid rpc response (JContainer): " + responseArray[i].ToString());
                    continue;
                }
                JToken eventJToken = jContainer.SelectToken("e");
                if (eventJToken == null)
                {
                    Debug.Log("Invalid rpc response (jToken): " + jContainer.ToString());
                    continue;
                }
                string eventType = eventJToken.Value<string>();
                Debug.Log(eventType);
                Debug.Log(response);
                JContainer argsContainer = null;
                int roomId = -1;
                int seatId = -1;
                int roomArrayIndex = -1;
                switch (eventType)
                {
                    case "look":
                        argsContainer = (JContainer)jContainer.SelectToken("args");
                        roomId = argsContainer.SelectToken("id").Value<int>();
                        roomArrayIndex = Array.IndexOf(Globals.roomIdArray, -1);
                        Globals.roomIdArray[roomArrayIndex] = roomId;
                        Globals.rooms[roomArrayIndex] = argsContainer.ToObject<Dictionary<string, object>>();
                        break;
                    case "enter":
                        argsContainer = (JContainer)jContainer.SelectToken("args");
                        roomId = argsContainer.SelectToken("where").Value<int>();
                        roomArrayIndex = Array.IndexOf(Globals.roomIdArray, roomId);
                        break;
                    case "takeseat":
                        argsContainer = (JContainer)jContainer.SelectToken("args");
                        seatId = argsContainer.SelectToken("where").Value<int>();
                        string seatedUserid = argsContainer.SelectToken("uid").Value<string>();
                        break;
                    case "ready":
                        break;
                    case "drop":
                        break;
                    case "seecard":
                        break;
                    case "moveturn":
                        break;
                    case "countdown":
                        break;
                    case "fold":
                        break;
                    case "gameover":
                        break;
                    case "leave":
                        break;

                    default:
                        break;
                }
            }
        }
    }

    public void SendRpc(object data, Action<JToken> callback, bool oneTime=true)
    {
        var req = new ExpandoObject() as IDictionary<string, System.Object>;
        var props = data.GetType().GetProperties();
        foreach (var property in props)
        {
            if (property.CanRead)
            {
                req[property.Name] = property.GetValue(data);
            }
        }
        string seq = rnd.Next().ToString();
        req["seq"] = seq;

        _rpcEventHandlers.Add(seq, callback);
        _rpcEventTimes.Add(seq, oneTime);
        socketIoUnity.Emit("rpc", req);
    }

}
