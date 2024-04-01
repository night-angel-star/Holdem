using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketIoInterface : MonoBehaviour
{
    private SocketIoWrapper socket;
    public class EventData{
        public string _event;
        public string _msg;
    };

    private Dictionary<string, Action<string>> _eventHandlers = new Dictionary<string, Action<string>>();

    public void SetSocket(SocketIoWrapper _socket)
    {
        socket = _socket;
    }
    public void callSocketEvent(string data)
    {
        try
        {
            Debug.Log(data);
            EventData eventData = JsonUtility.FromJson<EventData>(data);
            Debug.Log(eventData._msg);
            if (_eventHandlers.ContainsKey(eventData._event))
            {
                _eventHandlers[eventData._event].Invoke(eventData._msg);
            }
        }
        catch(Exception ex)
        {
            LogHelper.AppLog("callSocketEvent");
            LogHelper.AppLog(ex.ToString());
        }
        
    }

    public void On(string _event, Action<string> callback)
    {
        _eventHandlers.Add(_event, callback);
    }

}