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
using UnityEngine.SceneManagement;

public class DealArgs
{
    public int delay;
    public List<List<object>> deals;
}

public class SocketIoConnection1
{
    public static SocketIOUnity socketIoUnity;
    public string serverUri;
    Dictionary<string, Action<JToken>> _rpcEventHandlers = new Dictionary<string, Action<JToken>>();
    Dictionary<string, bool> _rpcEventTimes = new Dictionary<string, bool>();
    private System.Random rnd = new System.Random();

    private bool SafeConnect()
    {
        // Hello Handshack
        socketIoUnity.On("hello", (data) =>
        {
            // Debug.Log("Hello");
            socketIoUnity.Emit("hello");
        });
        socketIoUnity.On("notify", OnNotify);
        socketIoUnity.On("rpc_ret", OnRpcRet);
        socketIoUnity.OnConnected += OnConnected;
        socketIoUnity.Connect();
        Debug.Log("serverUri is:"+serverUri);
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
        Globals.connected = true;
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
                JContainer argsContainer = (JContainer)jContainer.SelectToken("args");

                Debug.Log(eventType);
                Debug.Log(response);

                int roomId = -1;
                int seatId = -1;
                int roomArrayIndex = -1;
                string seatedUserid = null;
                JObject who = null;
                JObject gamerList;
                string userid;
                int pot;
                switch (eventType)
                {
                    case "prompt":
                        
                        if (jContainer.SelectToken("roomid") != null)
                        {
                            roomId = jContainer.SelectToken("roomid").Value<int>();
                            roomArrayIndex = Globals.getRoomIndex(roomId);
                            Dictionary<string, object> roomState = Globals.roomStates[roomArrayIndex];
                            Dictionary<string, object> argsDictionary = argsContainer.ToObject<Dictionary<string, object>>();
                            List<string> commonKeys = new List<string>();

                            foreach (string key in argsDictionary.Keys)
                            {
                                if (roomState.ContainsKey(key))
                                {
                                    commonKeys.Add(key);
                                }
                            }
                            foreach (string key in commonKeys)
                            {
                                Globals.roomStates[roomArrayIndex][key] = argsDictionary[key];
                            }
                        }
                        break;
                    case "relogin":
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            SceneManager.LoadScene("Room");
                        });
                        break;
                    case "look":
                        roomId = argsContainer.SelectToken("id").Value<int>();
                        roomArrayIndex = Globals.getBlankRoomIndex();
                        if(Globals.getRoomIndex(roomId) > -1)
                        {
                            roomArrayIndex = Globals.getRoomIndex(roomId);
                            Globals.currentRoomId = roomId;
                        } else if(roomArrayIndex > -1)
                        {
                            Globals.roomIdArray[roomArrayIndex] = roomId;
                            Globals.rooms[roomArrayIndex] = argsContainer.ToObject<Dictionary<string, object>>();
                            Globals.currentRoomIndex = roomArrayIndex;
                            Globals.roomStates[Globals.currentRoomIndex] =
                                new Dictionary<string, object>
                                {
                                { "id", roomId },
                                { "fold", null },
                                { "check", null },
                                { "call", null },
                                { "raise", null },
                                { "all_in", null },
                                { "ready", null },
                                };
                            Globals.gamersActionStates[roomArrayIndex] = new string[9];
                            Globals.shareCards[roomArrayIndex] = new int[5];
                            Globals.currentRoomId = roomId;
                        }
                        break;
                    case "enter":
                        roomId = argsContainer.SelectToken("where").Value<int>();
                        who = JObject.Parse(argsContainer.SelectToken("who").Value<object>().ToString());
                        roomArrayIndex = Globals.getRoomIndex(roomId);
                        JObject gamers = (JObject)Globals.rooms[roomArrayIndex]["gamers"];
                        try
                        {
                            gamers.Add(who["uid"].ToString(), (JToken)who);
                        }
                        catch (Exception)
                        {
                            break;
                        }
                        Globals.rooms[roomArrayIndex]["gamers"] = gamers;
                        break;
                    case "buychip":
                        roomId = jContainer.SelectToken("room").Value<int>();
                        userid = argsContainer.SelectToken("uid").Value<string>();
                        roomArrayIndex = Globals.getRoomIndex(roomId);
                        gamerList = JObject.Parse(Globals.rooms[roomArrayIndex]["gamers"].ToString());
                        gamerList[userid]["coins"] = Int32.Parse(argsContainer.SelectToken("coins").ToString());
                        Globals.rooms[roomArrayIndex]["gamers"] = gamerList;
                        break;
                    case "takeseat":
                        seatId = argsContainer.SelectToken("where").Value<int>();
                        roomId = argsContainer.SelectToken("roomid").Value<int>();
                        seatedUserid = argsContainer.SelectToken("uid").Value<object>().ToString();
                        roomArrayIndex = Globals.getRoomIndex(roomId);
                        JArray seats = (JArray)Globals.rooms[roomArrayIndex]["seats"];
                        seats[seatId] = seatedUserid;
                        Globals.rooms[roomArrayIndex]["seats"] = seats;
                        break;
                    case "ready":
                        seatedUserid = argsContainer.SelectToken("uid").Value<object>().ToString();
                        seatId = argsContainer.SelectToken("where").Value<int>();
                        Debug.Log(seatedUserid + " is ready in seat " + seatId);
                        break;
                    case "gamestart":
                        JObject startedRoom = argsContainer.SelectToken("room").Value<object>() as JObject;
                        Dictionary<string, object> roomObject = NewtonSoftHelper.JObjectToObject<string, object>(startedRoom);
                        int startedRoomId = Int32.Parse(roomObject["id"].ToString());
                        roomArrayIndex = Array.IndexOf(Globals.roomIdArray, startedRoomId);
                        Globals.roomGameStarted[roomArrayIndex] = true;
                        Globals.rooms[roomArrayIndex] = roomObject;
                        break;
                    case "deal":
                        roomId = jContainer.SelectToken("room").Value<int>();
                        roomArrayIndex = Globals.getRoomIndex(roomId);
                        DealArgs args = JsonConvert.DeserializeObject < DealArgs > (argsContainer.ToString());
                        if(args.delay == 1)
                        {
                            int[] shareCards = JsonConvert.DeserializeObject<int[]>(args.deals[0][1].ToString());
                            foreach (int card in shareCards)
                            {
                                int index1 = Array.IndexOf(Globals.shareCards[roomArrayIndex], 0);
                                Globals.shareCards[roomArrayIndex][index1] = card;
                            }
                        }
                        break;
                    case "drop":
                        break;
                    case "seecard":
                        roomId = jContainer.SelectToken("roomid").Value<int>();
                        JArray myCardsJArray = argsContainer.SelectToken("cards").Value<JArray>();
                        int[] myCards = NewtonSoftHelper.JArrayToArray<int>(myCardsJArray);
                        roomArrayIndex = Globals.getRoomIndex(roomId);
                        Globals.rooms[roomArrayIndex].Add("myCards", myCards);
                        break;
                    case "moveturn":
                        roomId = argsContainer.SelectToken("roomid").Value<int>();
                        roomArrayIndex = Globals.getRoomIndex(roomId);
                        Globals.rooms[roomArrayIndex]["countDownSec"] = (object)0;
                        break;
                    case "countdown":
                        roomId = argsContainer.SelectToken("roomid").Value<int>();
                        int countDownActiveUserIndex = argsContainer.SelectToken("seat").Value<int>();
                        int countDownSec = argsContainer.SelectToken("sec").Value<int>();
                        roomArrayIndex = Globals.getRoomIndex(roomId);
                        Globals.roomGameStarted[roomArrayIndex] = true;
                        if (Globals.rooms[roomArrayIndex].ContainsKey("activeUserIndex"))
                        {
                            Globals.rooms[roomArrayIndex]["activeUserIndex"] = (object)countDownActiveUserIndex;
                        }
                        else
                        {
                            Globals.rooms[roomArrayIndex].Add("activeUserIndex", (object)countDownActiveUserIndex);
                        }

                        if (Globals.rooms[roomArrayIndex].ContainsKey("countDownSec"))
                        {
                            Globals.rooms[roomArrayIndex]["countDownSec"] = (object)countDownSec;
                        }
                        else
                        {
                            Globals.rooms[roomArrayIndex].Add("countDownSec", (object)countDownSec);
                        }
                        break;
                    case "check":
                        roomId = jContainer.SelectToken("roomid").Value<int>();
                        roomArrayIndex = Globals.getRoomIndex(roomId);
                        roomId = jContainer.SelectToken("room").Value<int>();
                        seatId = argsContainer.SelectToken("seat").Value<int>();
                        Globals.gamersActionStates[roomArrayIndex][seatId] = "check";
                        break;
                    case "call":
                        roomId = jContainer.SelectToken("room").Value<int>();
                        roomArrayIndex = Globals.getRoomIndex(roomId);
                        userid = argsContainer.SelectToken("uid").Value<string>();
                        seatId = argsContainer.SelectToken("seat").Value<int>();
                        pot = Int32.Parse(Globals.rooms[roomArrayIndex]["pot"].ToString());
                        pot += argsContainer.SelectToken("call").Value<int>();
                        Globals.rooms[roomArrayIndex]["pot"] = (object)pot;
                        gamerList = JObject.Parse(Globals.rooms[roomArrayIndex]["gamers"].ToString());
                        gamerList[userid]["coins"] = Int32.Parse(gamerList[userid]["coins"].ToString()) - argsContainer.SelectToken("call").Value<int>();
                        gamerList[userid]["chips"] = Int32.Parse(gamerList[userid]["chips"].ToString()) + argsContainer.SelectToken("call").Value<int>();
                        Globals.rooms[roomArrayIndex]["gamers"] = gamerList;
                        Globals.gamersActionStates[roomArrayIndex][seatId] = "call";
                        break;
                    case "raise":
                        roomId = jContainer.SelectToken("room").Value<int>();
                        roomArrayIndex = Globals.getRoomIndex(roomId);
                        userid = argsContainer.SelectToken("uid").Value<string>();
                        seatId = argsContainer.SelectToken("seat").Value<int>();
                        int amount_sum = argsContainer.SelectToken("call").Value<int>() + argsContainer.SelectToken("raise").Value<int>();
                        pot = Int32.Parse(Globals.rooms[roomArrayIndex]["pot"].ToString());
                        pot += amount_sum;
                        Globals.rooms[roomArrayIndex]["pot"] = (object)pot;
                        gamerList = JObject.Parse(Globals.rooms[roomArrayIndex]["gamers"].ToString());
                        gamerList[userid]["coins"] = Int32.Parse(gamerList[userid]["coins"].ToString()) - amount_sum;
                        gamerList[userid]["chips"] = Int32.Parse(gamerList[userid]["chips"].ToString()) + amount_sum;
                        Globals.rooms[roomArrayIndex]["gamers"] = gamerList;
                        Globals.gamersActionStates[roomArrayIndex][seatId] = "call";
                        break;
                    case "fold":
                        roomId = jContainer.SelectToken("roomid").Value<int>();
                        roomArrayIndex = Globals.getRoomIndex(roomId);
                        roomId = jContainer.SelectToken("room").Value<int>();
                        seatId = argsContainer.SelectToken("seat").Value<int>();
                        Globals.gamersActionStates[roomArrayIndex][seatId] = "fold";
                        break;
                    case "gameover":
                        roomId = jContainer.SelectToken("room").Value<int>();
                        roomArrayIndex = Globals.getRoomIndex(roomId);
                        JObject gamersList = JObject.Parse(Globals.rooms[roomArrayIndex]["gamers"].ToString());
                        foreach (object player in argsContainer)
                        {
                            JContainer playerJContainer = (JContainer)player;
                            string playerUid = playerJContainer.SelectToken("uid").ToString();
                            gamersList[playerUid] = JObject.Parse(player.ToString());
                        }
                        Globals.roomGameStarted[roomArrayIndex] = false;
                        Globals.rooms[roomArrayIndex]["gamers"] = gamersList;
                        Globals.shareCards[roomArrayIndex] = new int[5];
                        break;
                    case "leave":
                        roomId = argsContainer.SelectToken("where").Value<int>();
                        string leaveUserid = argsContainer.SelectToken("uid").Value<object>().ToString();
                        roomArrayIndex = Globals.getRoomIndex(roomId);
                        string[] seatArray = NewtonSoftHelper.JArrayToArray<string>(Globals.rooms[roomArrayIndex]["seats"]);
                        if ((seatId = Array.IndexOf(seatArray, leaveUserid)) != -1)
                        {
                            seatArray[seatId] = null;
                        }
                        Dictionary<string, object> gamersObject = NewtonSoftHelper.JObjectToObject<string, object>(Globals.rooms[roomArrayIndex]["gamers"]);
                        gamersObject.Remove(leaveUserid);
                        Globals.rooms[roomArrayIndex]["seats"] = seatArray;
                        Globals.rooms[roomArrayIndex]["gamers"] = gamersObject;
                        Globals.shareCards[roomArrayIndex] = new int[5];
                        break;
                    default:
                        break;
                }
            }
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
        socketIoUnity.Emit("rpc", req);
    }

}
