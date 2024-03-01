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
    public void Start()
    {
        RegisterGameEvents();
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
    private void OnRelogin(JToken baseToken)
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
                ProcessRelogin();
            }

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

    private void OnBuyChip(JToken baseToken)
    {
        string errorString = "";
        do
        {
            if (baseToken == null)
                break;
            if (baseToken.Type != JTokenType.Object)
                break;

            BuyChipNotifyEvent json = baseToken.ToObject<BuyChipNotifyEvent>();
            if (json != null)
            {
                ProcessBuyChip(json.uid, json.args.roomid.ToString(), json.args.amount);
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
                    Globals.gameRooms[roomid].gameStatus = 2;
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
    private void OnMoveTurn(JToken baseToken)
    {
        string errorString = "";
        do
        {
            if (baseToken == null)
                break;
            if (baseToken.Type != JTokenType.Object)
                break;

            MoveturnNotifyEvent json = baseToken.ToObject<MoveturnNotifyEvent>();
            if (json != null)
            {
                ProcessMoveturn(json.roomid.ToString(), json.args.seat, json.args.countdown);
            }

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
    private void OnCountDown(JToken baseToken)
    {
        string errorString = "";
        do
        {
            if (baseToken == null)
                break;
            if (baseToken.Type != JTokenType.Object)
                break;

            CountdownNotifyEvent json = baseToken.ToObject<CountdownNotifyEvent>();
            if (json != null)
            {
                ProcessCountdown(json.roomid.ToString(), json.args.sec);
            }

        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
        }
    }
    private void OnSeecard(JToken baseToken)
    {
        string errorString = "";
        do
        {
            if (baseToken == null)
                break;
            if (baseToken.Type != JTokenType.Object)
                break;

            SeeCardNotifyEvent json = baseToken.ToObject<SeeCardNotifyEvent>();
            if (json != null)
            {
                if(json.args.cards != null) ProcessSeeCard(json.args.uid, json.roomid.ToString(), json.args.seat, json.args.cards);
            }

        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
        }
    }
    public void RegisterGameEvents()
    {
        Globals.socketIoConnection.AddNotifyHandler("relogin", OnRelogin);
        Globals.socketIoConnection.AddNotifyHandler("seecard", OnSeecard);
        Globals.socketIoConnection.AddNotifyHandler("unseat", OnUnseat);
        Globals.socketIoConnection.AddNotifyHandler("leave", OnLeave);
        Globals.socketIoConnection.AddNotifyHandler("look", OnLook);
        Globals.socketIoConnection.AddNotifyHandler("enter", OnEnter);
        Globals.socketIoConnection.AddNotifyHandler("buychip", OnBuyChip);
        Globals.socketIoConnection.AddNotifyHandler("takeseat", OnTakeSeat);
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
        // Globals.socketIoConnection.AddNotifyHandler("shout", OnPrompt);
        // Globals.socketIoConnection.AddNotifyHandler("exit", OnPrompt);
        // Globals.socketIoConnection.AddNotifyHandler("pk", OnPrompt);
        // Globals.socketIoConnection.AddNotifyHandler("showcard", OnPrompt);
        // Globals.socketIoConnection.AddNotifyHandler("bue", OnPrompt);
        // Globals.socketIoConnection.AddNotifyHandler("say", OnPrompt);
        Globals.socketIoConnection.AddNotifyHandler("prompt", OnPrompt);

    }

    private void ProcessRelogin()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            SceneManager.LoadScene("Room");
        });
    }

    private void ProcessSeeCard(string uid, string roomid, int seat, int[] cards)
    {
        if (Globals.gameRooms.ContainsKey(roomid))
        {
            if (Globals.gameRooms[roomid].cards.ContainsKey(seat))
            {
                Globals.gameRooms[roomid].cards[seat] = cards;
            } else
            {
                Globals.gameRooms[roomid].cards[seat] = cards;
            }
        }
    }
    private void ProcessCountdown(string roomid, int sec)
    {
        if (Globals.gameRooms.ContainsKey(roomid))
        {
             Globals.gameRooms[roomid].countdown = sec;
        }
    }

    private void ProcessMoveturn(string roomid, int seat, int countdown)
    {
        if (Globals.gameRooms.ContainsKey(roomid))
        {
            if (Globals.gameRooms[roomid].totalCount == -1)
            {
                Globals.gameRooms[roomid].totalCount = countdown;
            }
            Globals.gameRooms[roomid].activeSeat = seat;
            Globals.gameRooms[roomid].countdown = countdown;
        }
    }

    private void ProcessTakeSeat(string uid, string roomid, int seat)
    {
        if (Globals.gameRooms.ContainsKey(roomid))
        {
            Globals.gameRooms[roomid].seats[seat] = uid;
            Globals.gameRooms[roomid].seats_count++;
            Globals.gameRooms[roomid].gameStatus = 0;
        }
    }
    private void ProcessBuyChip(string uid, string roomid, int amount)
    {
        if (Globals.gameRooms.ContainsKey(roomid))
        {
            Globals.gameRooms[roomid].gamers[uid].coins = amount;
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
            Globals.gameRooms[roomid].gameStatus = 1;
        }
    }

}