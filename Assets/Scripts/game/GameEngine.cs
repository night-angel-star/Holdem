using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
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

            ReloginNotifyEvent json = baseToken.ToObject<ReloginNotifyEvent>();
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
    private void OnPrompt(JToken baseToken)
    {
        string errorString = "";
        do
        {
            if (baseToken == null)
                break;
            if (baseToken.Type != JTokenType.Object)
                break;

            PromptNotifyEvent json = baseToken.ToObject<PromptNotifyEvent>();
            if (json != null)
            {
                foreach (var field in typeof(PromptNotifyEvent).GetFields())
                {
                    if (field.FieldType == typeof(bool))
                    {
                        bool temp = (bool)field.GetValue(json.args);
                        if(temp == true)
                        {
                            field.SetValue(Globals.gameRooms[json.roomid.ToString()].operations, field.GetValue(json.args)); 
                        }
                    }
                }
            }

        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
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
                ProcessBuyChip(json.args.uid, json.roomid.ToString(), json.args.amount);
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

                            Globals.gameRooms[roomid].cards.Remove(i);

                            Globals.gameRooms[roomid].status[i] = "";
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
            Globals.gameRooms[r.id].status = new string[r.options.max_seats];
            Globals.gameRooms[r.id].shared_cards = new int[5];
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
                List<List<object>> deals = json.args.deals;
                if (deals == null)
                {
                    return;
                }
                if (json.args.delay == 1)
                {
                    int[] shareCards = JsonConvert.DeserializeObject<int[]>(deals[0][1].ToString());
                    foreach (int card in shareCards)
                    {
                        Array.Resize(ref Globals.gameRooms[json.roomid.ToString()].shared_cards, Globals.gameRooms[json.roomid.ToString()].shared_cards.Length + 1);
                        Globals.gameRooms[json.roomid.ToString()].shared_cards[Globals.gameRooms[json.roomid.ToString()].shared_cards.Length - 1] = card;
                    }
                    Debug.Log(Globals.gameRooms[json.roomid.ToString()].shared_cards);
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
    private void OnFold(JToken baseToken)
    {
        string errorString = "";
        do
        {
            if (baseToken == null)
                break;
            FoldNotifyEvent json = baseToken.ToObject<FoldNotifyEvent>();

            if (json == null)
            {
                break;
            }
            Globals.gameRooms[json.roomid.ToString()].status[json.args.seat] = "fold";
        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
        }
    }
    private void OnCheck(JToken baseToken)
    {
        string errorString = "";
        do
        {
            if (baseToken == null)
                break;
            CheckNotifyEvent json = baseToken.ToObject<CheckNotifyEvent>();

            if (json == null)
            {
                break;
            }
            Globals.gameRooms[json.roomid.ToString()].status[json.args.seat] = "check";
        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
        }
    }
    private void OnCall(JToken baseToken)
    {
        string errorString = "";
        do
        {
            if (baseToken == null)
                break;
            CallNotifyEvent json = baseToken.ToObject<CallNotifyEvent>();

            if (json == null)
            {
                break;
            }
            Globals.gameRooms[json.roomid.ToString()].status[json.args.seat] = "call";
            Globals.gameRooms[json.roomid.ToString()].pot += json.args.call;
            Globals.gameRooms[json.roomid.ToString()].chips[json.args.seat] += json.args.call;
            Globals.gameRooms[json.roomid.ToString()].gamers[json.args.uid].coins -= json.args.call;
        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
        }
    }
    private void OnRaise(JToken baseToken)
    {
        string errorString = "";
        do
        {
            if (baseToken == null)
                break;
            RaiseNotifyEvent json = baseToken.ToObject<RaiseNotifyEvent>();

            if (json == null)
            {
                break;
            }
            Globals.gameRooms[json.roomid.ToString()].status[json.args.seat] = "raise";
            Globals.gameRooms[json.roomid.ToString()].pot += json.args.call + json.args.raise;
            Globals.gameRooms[json.roomid.ToString()].chips[json.args.seat] += json.args.call + json.args.raise;
            Globals.gameRooms[json.roomid.ToString()].gamers[json.args.uid].coins -= json.args.call + json.args.raise;
        } while (false);
        if (errorString != "")
        {
            Debug.Log(errorString);
        }
    }
    private void OnGameOver(JToken baseToken)
    {
        string errorString = "";
        do
        {
            if (baseToken == null)
                break;
            GameoverNotifyEvent json = baseToken.ToObject<GameoverNotifyEvent>();

            if (json == null)
            {
                break;
            }
            Globals.gameRooms[json.roomid.ToString()].gameStatus = 3;
            foreach(Gamer g in json.args)
            {
                Globals.gameRooms[json.roomid.ToString()].gamers[g.uid] = g;
                int index = Array.IndexOf(Globals.gameRooms[json.roomid.ToString()].seats, g.uid);
                Globals.gameRooms[json.roomid.ToString()].cards[index] = g.cards;
            }
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
        // Globals.socketIoConnection.AddNotifyHandler("unseat", OnUnseat);
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
        Globals.socketIoConnection.AddNotifyHandler("fold", OnFold);
        Globals.socketIoConnection.AddNotifyHandler("check", OnCheck);
        Globals.socketIoConnection.AddNotifyHandler("call", OnCall);
        Globals.socketIoConnection.AddNotifyHandler("raise", OnRaise);
        // Globals.socketIoConnection.AddNotifyHandler("all_in", OnMoveTurn);
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
            Globals.gameRooms[roomid].seats_taken++;
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
            if(uid == Globals.gameToken.uid)
            {
                Globals.gameRooms[roomid].gameStatus = 1;
            }
        }
    }

}