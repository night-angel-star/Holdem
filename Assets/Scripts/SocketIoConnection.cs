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


public class SocketIoConnection
{
    public static SocketIOUnity socketIoUnity;
    public string serverUri;
    Dictionary<string, Action<JToken>> _rpcEventHandlers = new Dictionary<string, Action<JToken>>();
    Dictionary<string, Action<JToken>> _notifyEventHandlers = new Dictionary<string, Action<JToken>>();
    Dictionary<string, bool> _rpcEventTimes = new Dictionary<string, bool>();
    Dictionary<string, bool> _notifyEventTimes = new Dictionary<string, bool>();
    private System.Random rnd = new System.Random();
    public GameEngine engine;

    private bool SafeConnect()
    {
        // Hello Handshack
        socketIoUnity.On("hello", (data) => {
            Debug.Log("Hello");
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
            }
            else
            {
                return true;
            }
        }

        return true;
    }

    private void OnConnected(object sender, EventArgs e)
    {
        Debug.Log($"Connected: {socketIoUnity.Connected}");
        LogHelper.AppLog("App is connected to server");
        Globals.connected = true;
        engine = new GameEngine();
        engine.Start();
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
    private void ProcessNotify(SocketIOResponse response)
    {
        JToken[] jToken = JsonConvert.DeserializeObject<JToken[]>(response.ToString());
        if (jToken != null)
        {
            for (int i = 0; i < jToken.Length; i++)
            {
                NotifyEvent notifyEvent = jToken[i].ToObject<NotifyEvent>();
                if (notifyEvent != null)
                {
                    if (notifyEvent.e != null)
                    {
                        string eventName = notifyEvent.e;
                        if (_notifyEventHandlers.ContainsKey(eventName))
                        {
                            _notifyEventHandlers[eventName](jToken[i]);
                            if (_notifyEventTimes.ContainsKey(eventName) && _notifyEventTimes[eventName] == true)
                            {
                                _notifyEventHandlers.Remove(eventName);
                                _notifyEventTimes.Remove(eventName);
                            }
                        }
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
    private void OnNotify(SocketIOResponse data)
    {
        Debug.Log("notify : " + data);
        ProcessNotify(data);
    }

    public void AddNotifyHandler(string e, Action<JToken> callback, bool oneTime = false)
    {
        if (_notifyEventHandlers.ContainsKey(e))
        {
            _notifyEventHandlers.Remove(e);
        }
        _notifyEventHandlers.Add(e, callback);
        if (_notifyEventTimes.ContainsKey(e))
        {
            _notifyEventTimes.Remove(e);
        }
        _notifyEventTimes.Add(e, oneTime);
    }
    public void RemoveNotifyHandler(string e)
    {
        if (_notifyEventHandlers.ContainsKey(e))
        {
            _notifyEventHandlers.Remove(e);
        }
    }

    public void SendRpc(object data, Action<JToken> callback, bool oneTime = true)
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
        Debug.Log("rpc : " + req.ToString());
        socketIoUnity.Emit("rpc", req);
    }

}
