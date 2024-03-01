using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

using JsonResponseT = System.Collections.Generic.Dictionary<string, object>;
public class GameEngine
{
    // private GameBehavior parentBehavior;

    // public GameEngine(GameBehavior behavior)
    // {
        // parentBehavior = behavior;
    // }
    public void Start()
    {
        RegisterGameEvents();
    }
    public void ShowRoom(Room room)
    {
        // parentBehavior.room = room;
        // parentBehavior.ShowRoom();
    }

    private void OnRpcResponse(JToken jsonResponse)
    {
        if (jsonResponse == null)
        {
            return;
        }
    }

    private void OnRpcEnter(JToken baseToken)
    {
    }
    private void OnRelogin(JToken jsonResponse)
    {
        string errorString = "";
        if (jsonResponse == null)
        {
            return;
        }
        do
        {
            if (jsonResponse.Type != JTokenType.Object)
            {
                errorString = "Invalid notify event (message is not object) : " + jsonResponse.ToString();
                break;
            }
            JsonResponseT response = JsonResponse.ToDictionary(jsonResponse);
            if (!response.ContainsKey("args"))
            {
                errorString = "Invalid notify event (no args) : " + jsonResponse.ToString();
                break;
            }
            if (response["args"].ConvertTo<JToken>().Type != JTokenType.Object)
            {
                errorString = "Invalid notify event (args is not object) : " + jsonResponse.ToString();
                break;
            }
            JsonResponseT args = JsonResponse.ToDictionary(response["args"]);
            ProcessRelogin(args["uid"].ToString(), args["where"].ToString());
        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
        }
    }
    private void OnPrompt(JToken jsonResponse)
    {
        if (jsonResponse == null)
        {
            return;
        }
        JToken uidToken = jsonResponse.SelectToken("uid");
        if (uidToken == null || uidToken.Type == JTokenType.Null)
        {
            return;
        }
        string uidString = uidToken.ToString();
        JToken roomIdToken = jsonResponse.SelectToken("roomid");
        if (roomIdToken != null)
        {
            if (roomIdToken.Type == JTokenType.Integer || roomIdToken.Type == JTokenType.String)
            {
                string roomId = roomIdToken.ToString();

                JToken argsToken = jsonResponse.SelectToken("args");
                if (argsToken != null)
                {
                    if (Globals.gameRooms.ContainsKey(roomId))
                    {
                        Globals.gameRooms[roomId].UpdateCmdsFromJson(argsToken);
                        /* if (parentBehavior.room.id == roomId)
                        {
                            parentBehavior.room.UpdateCmdsFromJson(argsToken);
                        } */
                    }
                }
            }
        }

    }
    private void OnTakeSeat(JToken baseToken)
    {
        string errorString = "";
        do
        {
            if (baseToken == null)
                break;
            if (baseToken.Type != JTokenType.Object)
                break;

            TakeSeatNotifyEvent json = baseToken.ToObject<TakeSeatNotifyEvent>();
            if (json != null)
            {
                ProcessTakeSeat(json.args.uid, json.roomid.ToString(), json.args.where);
            }

        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
        }
    }
    private void OnUnseat(JToken baseToken)
    {
        string errorString = "";
        do
        {
            if (baseToken == null)
                break;
            if (baseToken.Type != JTokenType.Object)
                break;

            UnSeatNotifyEvent json = baseToken.ToObject<UnSeatNotifyEvent>();
            if (json != null)
            {
                ProcessUnseat(json.args.uid, json.roomid.ToString(), json.args.where);
            }

        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
        }
    }

    private void OnLeave(JToken baseToken)
    {
        string errorString = "";
        do
        {
            if (baseToken == null)
                break;
            if (baseToken.Type != JTokenType.Object)
                break;

            LeaveNotifyEvent json = baseToken.ToObject<LeaveNotifyEvent>();
            if (json != null)
            {
                string roomid = json.args.where.ToString();
                string uid = json.args.uid;

                if (Globals.gameRooms.ContainsKey(roomid))
                {
                    Room room = Globals.gameRooms[roomid];
                    for (int i = 0; i < room.seats.Length; i++)
                    {
                        if (room.seats[i] == uid)
                        {
                            Globals.gameRooms[roomid].seats[i] = null;
                            Globals.gameRooms[roomid].seats_count--;
                        }
                    }
                    Globals.gameRooms[roomid].gamers.Remove(uid);
                    Globals.gameRooms[roomid].gamers_count--;
                }
            }

        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
        }
    }
    private void OnEnter(JToken baseToken)
    {
        string errorString = "";
        do
        {
            if (baseToken == null)
                break;
            if (baseToken.Type != JTokenType.Object)
                break;

            EnterNotifyEvent json = baseToken.ToObject<EnterNotifyEvent>();
            if (json != null)
            {
                string roomid = json.args.where.ToString();
                Gamer g = json.args.who;
                if (Globals.gameRooms.ContainsKey(roomid))
                {
                    if (Globals.gameRooms[roomid].gamers.ContainsKey(g.uid))
                    {
                        Globals.gameRooms[roomid].gamers[g.uid] = g;
                    }
                    else
                    {
                        Globals.gameRooms[roomid].gamers.Add(g.uid, g);
                        Globals.gameRooms[roomid].gamers_count++;
                    }
                }
            }

        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
        }
    }
    private void OnLook(JToken baseToken)
    {
        string errorString = "";
        do
        {
            if (baseToken == null)
                break;
            LookNotifyEvent lookNotifyEvent = baseToken.ToObject<LookNotifyEvent>();

            if (lookNotifyEvent == null)
            {
                break;
            }
            Room r = lookNotifyEvent.args;
            if (Globals.gameRooms.ContainsKey(r.id))
            {
                Globals.gameRooms[r.id] = r;

            }
            else
            {
                Globals.gameRooms.Add(r.id, r);
            }
            Globals.currentRoom = r.id;
            ShowRoom(r);
        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
        }
    }
    private void OnReady(JToken baseToken)
    {
        string errorString = "";
        do
        {
            if (baseToken == null)
                break;
            if (baseToken.Type != JTokenType.Object)
                break;

            ReadyNotifyEvent json = baseToken.ToObject<ReadyNotifyEvent>();
            if (json != null)
            {
                ProcessReady(json.args.uid, json.roomid.ToString(), json.args.where);
            }

        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
        }
    }
    private void OnGameStart(JToken baseToken)
    {
        string errorString = "";
        do
        {
            if (baseToken == null)
                break;
            if (baseToken.Type != JTokenType.Object)
                break;

            GameStartNotifyEvent json = baseToken.ToObject<GameStartNotifyEvent>();
            if (json != null)
            {
                Room room = json.args.room;
                string roomid = room.id;
                if (Globals.gameRooms.ContainsKey(roomid))
                {
                    Globals.gameRooms[roomid] = room;
                    // Process seats
                }
            }

        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
        }
    }
    private void OnDeal(JToken baseToken)
    {
        string errorString = "";
        do
        {
            if (baseToken == null)
                break;
            if (baseToken.Type != JTokenType.Object)
                break;

            DealNotifyEvent json = baseToken.ToObject<DealNotifyEvent>();
            if (json != null)
            {
                JToken[][] deals= json.args.deals;
                if (deals != null)
                {
                    return;
                }
                for (int i = 0; i < deals.Length; i++)
                {
                    if (deals[i].Length != 2)
                        continue;
                    int seat = deals[i][0].Value<int>();
                    int[] cards = deals[i][1].ToObject<int[]>();
                    if (seat >= 0) // private card
                    {
                        
                    }
                }
            }

        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
        }
    }
    private void OnMoveTurn(JToken jsonResponse)
    {
        string errorString = "";
        do
        {
            if (jsonResponse == null)
                break;

        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
        }
    }
    private void OnFold(JToken jsonResponse)
    {
        string errorString = "";
        do
        {
            if (jsonResponse == null)
                break;

        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
        }
    }
    private void OnGameOver(JToken jsonResponse)
    {
        string errorString = "";
        do
        {
            if (jsonResponse == null)
                break;

        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
        }
    }
    private void OnCountDown(JToken jsonResponse)
    {
        string errorString = "";
        do
        {
            if (jsonResponse == null)
                break;

        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
        }
    }
    private void OnSeecard(JToken jsonResponse)
    {
        string errorString = "";
        if (jsonResponse == null)
        {
            return;
        }
        do
        {
            if (jsonResponse.Type != JTokenType.Object)
            {
                errorString = "Invalid notify event (message is not object) : " + jsonResponse.ToString();
                break;
            }
            JsonResponseT response = JsonResponse.ToDictionary(jsonResponse);
            if (!response.ContainsKey("args"))
            {
                errorString = "Invalid notify event (no args) : " + jsonResponse.ToString();
                break;
            }
            if (response["args"].ConvertTo<JToken>().Type != JTokenType.Object)
            {
                errorString = "Invalid notify event (args is not object) : " + jsonResponse.ToString();
                break;
            }
            JsonResponseT args = JsonResponse.ToDictionary(response["args"]);
            if (!args.ContainsKey("seat"))
            {
                errorString = "Invalid notify event (no seat) : " + jsonResponse.ToString();
                break;
            }
            if (!args.ContainsKey("cards"))
            {
                errorString = "Invalid notify event (no cards) : " + jsonResponse.ToString();
                break;
            }
            if (!args.ContainsKey("uid"))
            {
                errorString = "Invalid notify event (no uid) : " + jsonResponse.ToString();
                break;
            }
            if (args["cards"].ConvertTo<JToken>().Type != JTokenType.Array)
            {
                errorString = "Invalid notify event (cards is not array) : " + jsonResponse.ToString();
                break;
            }
            object[] oCards = JsonResponse.ToArray(args["cards"]);
            int[] iCards = new int[oCards.Length];

            for (int i = 0; i < oCards.Length; i++)
            {
                iCards[i] = oCards[i].ConvertTo<JToken>().Value<int>();
            }
            ProcessSeeCard(JsonResponse.ConvertTo<string>(args["uid"]), JsonResponse.ConvertTo<int>(args["seat"]), iCards);

        } while (false);
    }
    public void RegisterGameEvents()
    {
        Globals.socketIoConnection.AddNotifyHandler("relogin", OnRelogin);
        Globals.socketIoConnection.AddNotifyHandler("seecard", OnSeecard);
        Globals.socketIoConnection.AddNotifyHandler("takeseat", OnTakeSeat);
        Globals.socketIoConnection.AddNotifyHandler("unseat", OnUnseat);
        Globals.socketIoConnection.AddNotifyHandler("leave", OnLeave);
        Globals.socketIoConnection.AddNotifyHandler("enter", OnEnter);
        Globals.socketIoConnection.AddNotifyHandler("look", OnLook);
        Globals.socketIoConnection.AddNotifyHandler("ready", OnReady);
        Globals.socketIoConnection.AddNotifyHandler("gamestart", OnGameStart);
        Globals.socketIoConnection.AddNotifyHandler("gameover", OnGameOver);
        Globals.socketIoConnection.AddNotifyHandler("deal", OnDeal);
        Globals.socketIoConnection.AddNotifyHandler("moveturn", OnMoveTurn);
        Globals.socketIoConnection.AddNotifyHandler("fold", OnMoveTurn);
        Globals.socketIoConnection.AddNotifyHandler("check", OnMoveTurn);
        Globals.socketIoConnection.AddNotifyHandler("call", OnMoveTurn);
        Globals.socketIoConnection.AddNotifyHandler("raise", OnMoveTurn);
        Globals.socketIoConnection.AddNotifyHandler("all_in", OnMoveTurn);
        Globals.socketIoConnection.AddNotifyHandler("countdown", OnCountDown);
        Globals.socketIoConnection.AddNotifyHandler("shout", OnPrompt);
        Globals.socketIoConnection.AddNotifyHandler("exit", OnPrompt);
        Globals.socketIoConnection.AddNotifyHandler("pk", OnPrompt);
        Globals.socketIoConnection.AddNotifyHandler("showcard", OnPrompt);
        Globals.socketIoConnection.AddNotifyHandler("bue", OnPrompt);
        Globals.socketIoConnection.AddNotifyHandler("say", OnPrompt);
        Globals.socketIoConnection.AddNotifyHandler("prompt", OnPrompt);

    }

    private void ProcessRelogin(string uId, string roomId)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            SceneManager.LoadScene("Room");
        });
    }

    private void ProcessSeeCard(string uid, int seat, int[] cards)
    {

    }
    private void ProcessTakeSeat(string uid, string roomid, int seat)
    {
        if (Globals.gameRooms.ContainsKey(roomid))
        {
            Globals.gameRooms[roomid].seats[seat] = uid;
            Globals.gameRooms[roomid].seats_count++;
        }
    }
    private void ProcessUnseat(string uid, string roomid, int seat)
    {
        if (Globals.gameRooms.ContainsKey(roomid))
        {
            Globals.gameRooms[roomid].seats[seat] = null;
            Globals.gameRooms[roomid].seats_count--;
        }
    }
    private void ProcessReady(string uid, string roomid, int seat)
    {
        if (Globals.gameRooms.ContainsKey(roomid))
        {
            Globals.gameRooms[roomid].status[seat] = "ready";
        }
    }

}