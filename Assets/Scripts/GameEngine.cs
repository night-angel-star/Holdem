using EasyUI.Toast;
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
        try
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
        catch (Exception ex)
        {
            LogHelper.AppLog("OnRelogin");
            LogHelper.AppLog(ex.ToString());
        }

    }
    private void OnPrompt(JToken baseToken)
    {
        try
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
                    if (Globals.gameRooms.ContainsKey(json.roomid.ToString()))
                    {
                        Globals.gameRooms[json.roomid.ToString()].operations = json.args;
                    }
                }
            } while (false);
            if (errorString != "")
            {
                // Debug.Log(errorString);
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("OnPrompt");
            LogHelper.AppLog(ex.ToString());
        }
    }
    private void OnTakeSeat(JToken baseToken)
    {
        try
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
                    ProcessTakeSeat(json.args.uid, json.roomid.ToString(), json.args.where, json.args.coins);
                }

            } while (false);
            if (errorString != "")
            {
                Toast.Show(errorString);
                Debug.Log(errorString);
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("OnTakeSeat");
            LogHelper.AppLog(ex.ToString());
        }
    }

    private void OnBuyChip(JToken baseToken)
    {
        try
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
        catch (Exception ex)
        {
            LogHelper.AppLog("OnBuyChip");
            LogHelper.AppLog(ex.ToString());
        }
    }
    private void OnUnseat(JToken baseToken)
    {
        try
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
        catch (Exception ex)
        {
            LogHelper.AppLog("OnUnseat");
            LogHelper.AppLog(ex.ToString());
        }
    }

    private void OnLeave(JToken baseToken)
    {
        try
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
                    string roomid = json.roomid.ToString();
                    string uid = json.args.uid;
                    if (uid != Globals.gameToken.uid)
                    {
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
                }

            } while (false);
            if (errorString != "")
            {
                Debug.Log(errorString);
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("OnLeave");
            LogHelper.AppLog(ex.ToString());
        }
    }
    private void OnEnter(JToken baseToken)
    {
        try
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
        catch (Exception ex)
        {
            LogHelper.AppLog("OnEnter");
            LogHelper.AppLog(ex.ToString());
        }
    }
    private void OnLook(JToken baseToken)
    {
        try
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
        catch (Exception ex)
        {
            LogHelper.AppLog("OnLook");
            LogHelper.AppLog(ex.ToString());
        }
    }
    private void OnReady(JToken baseToken)
    {
        try
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
        catch (Exception ex)
        {
            LogHelper.AppLog("OnReady");
            LogHelper.AppLog(ex.ToString());
        }
    }
    private void OnGameStart(JToken baseToken)
    {
        try
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
                        object readyButtonStatus = Globals.gameRooms[roomid].operations.ready;
                        Globals.gameRooms[roomid] = room;
                        Globals.gameRooms[roomid].gameStatus = 2;
                        Globals.gameRooms[roomid].operations.ready = readyButtonStatus;
                    }
                    for (int i = 0; i < room.seats.Length; i++)
                    {
                        if (room.seats[i] != null)
                        {
                            if (!Globals.gameRooms[json.roomid.ToString()].chips.ContainsKey(i))
                            {
                                Globals.gameRooms[json.roomid.ToString()].chips[i] = 0;
                            }
                        }
                    }
                    Globals.gameRooms[roomid].status = new string[room.options.max_seats];
                    Globals.gameRooms[json.roomid.ToString()].shared_cards = new int[5];
                    Globals.gameRooms[json.roomid.ToString()].cards = new Dictionary<int, int[]>();
                }

            } while (false);
            if (errorString != "")
            {
                Debug.Log(errorString);
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("OnGameStart");
            LogHelper.AppLog(ex.ToString());
        }
    }
    private void OnDeal(JToken baseToken)
    {
        try
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
                            int index = Array.IndexOf(Globals.gameRooms[json.roomid.ToString()].shared_cards, 0);
                            // Array.Resize(ref Globals.gameRooms[json.roomid.ToString()].shared_cards, Globals.gameRooms[json.roomid.ToString()].shared_cards.Length + 1);
                            // Globals.gameRooms[json.roomid.ToString()].shared_cards[Globals.gameRooms[json.roomid.ToString()].shared_cards.Length - 1] = card;
                            Globals.gameRooms[json.roomid.ToString()].shared_cards[index] = card;
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
        catch (Exception ex)
        {
            LogHelper.AppLog("OnDeal");
            LogHelper.AppLog(ex.ToString());
        }
    }
    private void OnMoveTurn(JToken baseToken)
    {
        try
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
        catch (Exception ex)
        {
            LogHelper.AppLog("OnMoveTurn");
            LogHelper.AppLog(ex.ToString());
        }
    }
    private void OnFold(JToken baseToken)
    {
        try
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
                Globals.gameRooms[json.roomid.ToString()].status[json.args.seat] = "Fold";
            } while (false);
            if (errorString != "")
            {
                Debug.Log(errorString);
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("OnFold");
            LogHelper.AppLog(ex.ToString());
        }
    }
    private void OnCheck(JToken baseToken)
    {
        try
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
                Globals.gameRooms[json.roomid.ToString()].status[json.args.seat] = "Check";
            } while (false);
            if (errorString != "")
            {
                Debug.Log(errorString);
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("OnCheck");
            LogHelper.AppLog(ex.ToString());
        }
    }
    private void OnCall(JToken baseToken)
    {
        try
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
                Globals.gameRooms[json.roomid.ToString()].status[json.args.seat] = "Call";
                Globals.gameRooms[json.roomid.ToString()].pot = json.args.pot;
                Globals.gameRooms[json.roomid.ToString()].chips[json.args.seat] = json.args.chips;
                Globals.gameRooms[json.roomid.ToString()].gamers[json.args.uid].coins = json.args.coins;
            } while (false);
            if (errorString != "")
            {
                Debug.Log(errorString);
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("OnCall");
            LogHelper.AppLog(ex.ToString());
        }
    }
    private void OnRaise(JToken baseToken)
    {
        try
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
                Globals.gameRooms[json.roomid.ToString()].status[json.args.seat] = "Raise";
                Globals.gameRooms[json.roomid.ToString()].pot = json.args.pot;
                Globals.gameRooms[json.roomid.ToString()].chips[json.args.seat] = json.args.chips;
                Globals.gameRooms[json.roomid.ToString()].gamers[json.args.uid].coins = json.args.coins;
            } while (false);
            if (errorString != "")
            {
                Debug.Log(errorString);
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("OnRaise");
            LogHelper.AppLog(ex.ToString());
        }
    }
    private void OnAllIn(JToken baseToken)
    {
        try
        {
            string errorString = "";
            do
            {
                if (baseToken == null)
                    break;
                AllInNotifyEvent json = baseToken.ToObject<AllInNotifyEvent>();

                if (json == null)
                {
                    break;
                }
                Globals.gameRooms[json.roomid.ToString()].status[json.args.seat] = "All In";
                Globals.gameRooms[json.roomid.ToString()].pot = json.args.pot;
                Globals.gameRooms[json.roomid.ToString()].chips[json.args.seat] = json.args.chips;
                Globals.gameRooms[json.roomid.ToString()].gamers[json.args.uid].coins = json.args.coins;
            } while (false);
            if (errorString != "")
            {
                Debug.Log(errorString);
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("OnAllIn");
            LogHelper.AppLog(ex.ToString());
        }
    }
    private void OnGameOver(JToken baseToken)
    {
        try
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
                foreach (Gamer g in json.args)
                {
                    Globals.gameRooms[json.roomid.ToString()].gamers[g.uid] = g;
                    int index = Array.IndexOf(Globals.gameRooms[json.roomid.ToString()].seats, g.uid);
                    Globals.gameRooms[json.roomid.ToString()].cards[index] = g.cards;
                    if (g.prize > 0)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            Toast.Show("Winner: " + g.name);
                        });
                    }
                    if (Globals.gameRooms[json.roomid.ToString()].gamers[g.uid].coins == 0)
                    {
                        SleepForGameover();
                        Globals.gameRooms[json.roomid.ToString()].seats[index] = null;
                    }
                }
                Globals.gameRooms[json.roomid.ToString()].countdown = 20;
            } while (false);
            if (errorString != "")
            {
                Debug.Log(errorString);
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("OnGameOver");
            LogHelper.AppLog(ex.ToString());
        }

    }

    IEnumerator SleepForGameover()
    {
        yield return new WaitForSeconds(3);
    }
    private void OnCountDown(JToken baseToken)
    {
        try
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
                    if (json.args.seat != -1)
                    {
                        ProcessCountdown(json.roomid.ToString(), json.args.sec);
                    }
                    
                }

            } while (false);
            if (errorString != "")
            {
                Debug.Log(errorString);
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("OnCountDown");
            LogHelper.AppLog(ex.ToString());
        }
    }
    private void OnSeecard(JToken baseToken)
    {
        try
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
                    if (json.args.cards != null) ProcessSeeCard(json.args.uid, json.roomid.ToString(), json.args.seat, json.args.cards);
                }

            } while (false);
            if (errorString != "")
            {
                Debug.Log(errorString);
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("OnSeeCard");
            LogHelper.AppLog(ex.ToString());
        }
    }

    private void OnChat(JToken baseToken)
    {
        try
        {
            string errorString = "";
            do
            {
                if (baseToken == null)
                    break;
                if (baseToken.Type != JTokenType.Object)
                    break;

                ChatNotifyEvent json = baseToken.ToObject<ChatNotifyEvent>();
                if (json != null)
                {
                    ProcessChat(json.args.uid, json.args.where.ToString(), json.args.content);
                }

            } while (false);
            if (errorString != "")
            {
                Debug.Log(errorString);
            }
        }
        catch(Exception ex)
        {
            LogHelper.AppLog("OnChat");
            LogHelper.AppLog(ex.ToString());
        }
    }
    private void OnBye(JToken baseToken)
    {
        try
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                SceneManager.LoadScene("Login");
            });
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("OnBye");
            LogHelper.AppLog(ex.ToString());
        }
    }
    public void RegisterGameEvents()
    {

        try
        {
            Debug.Log("registering game events");
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
            Globals.socketIoConnection.AddNotifyHandler("all_in", OnAllIn);
            Globals.socketIoConnection.AddNotifyHandler("countdown", OnCountDown);
            // Globals.socketIoConnection.AddNotifyHandler("shout", OnPrompt);
            // Globals.socketIoConnection.AddNotifyHandler("exit", OnPrompt);
            // Globals.socketIoConnection.AddNotifyHandler("pk", OnPrompt);
            // Globals.socketIoConnection.AddNotifyHandler("showcard", OnPrompt);
            // Globals.socketIoConnection.AddNotifyHandler("bue", OnPrompt);
            // Globals.socketIoConnection.AddNotifyHandler("say", OnPrompt);
            Globals.socketIoConnection.AddNotifyHandler("prompt", OnPrompt);
            Globals.socketIoConnection.AddNotifyHandler("chat", OnChat);
            Globals.socketIoConnection.AddNotifyHandler("bye", OnBye);
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("RegisterGameEvents");
            LogHelper.AppLog(ex.ToString());
        }

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
        try
        {
            if (Globals.gameRooms.ContainsKey(roomid))
            {
                if (Globals.gameRooms[roomid].cards.ContainsKey(seat))
                {
                    Globals.gameRooms[roomid].cards[seat] = cards;
                }
                else
                {
                    Globals.gameRooms[roomid].cards[seat] = cards;
                }
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("ProcessSeeCard");
            LogHelper.AppLog(ex.ToString());
        }

    }
    private void ProcessCountdown(string roomid, int sec)
    {
        try
        {
            if (Globals.gameRooms.ContainsKey(roomid))
            {
                Globals.gameRooms[roomid].countdown = sec;
                Globals.gameRooms[roomid].gameStatus = 2;
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("ProcessCountdown");
            LogHelper.AppLog(ex.ToString());
        }

    }

    private void ProcessMoveturn(string roomid, int seat, int countdown)
    {
        try
        {
            if (Globals.gameRooms.ContainsKey(roomid))
            {
                if (Globals.gameRooms[roomid].totalCount == -1)
                {
                    Globals.gameRooms[roomid].totalCount = countdown;
                }
                Globals.gameRooms[roomid].activeSeat = seat;
                Globals.gameRooms[roomid].countdown = countdown;
                Globals.gameRooms[roomid].gameStatus = 2;
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("ProcessMoveturn");
            LogHelper.AppLog(ex.ToString());
        }

    }

    private void ProcessTakeSeat(string uid, string roomid, int seat, int coins)
    {
        try
        {
            if (Globals.gameRooms.ContainsKey(roomid))
            {

                Globals.gameRooms[roomid].seats[seat] = uid;
                Globals.gameRooms[roomid].gamers[uid].coins = coins;
                Globals.gameRooms[roomid].seats_taken++;
                Globals.gameRooms[roomid].gameStatus = 0;
                Debug.Log(Globals.gameRooms);
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("ProcessTakeSeat");
            LogHelper.AppLog(ex.ToString());
        }

    }
    private void ProcessBuyChip(string uid, string roomid, int amount)
    {
        try
        {
            if (Globals.gameRooms.ContainsKey(roomid))
            {
                Globals.gameRooms[roomid].gamers[uid].coins += amount;
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("ProcessBuyChip");
            LogHelper.AppLog(ex.ToString());
        }

    }
    private void ProcessUnseat(string uid, string roomid, int seat)
    {
        try
        {
            if (Globals.gameRooms.ContainsKey(roomid))
            {
                Globals.gameRooms[roomid].seats[seat] = null;
                Globals.gameRooms[roomid].seats_count--;
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("ProcessUnseat");
            LogHelper.AppLog(ex.ToString());
        }

    }
    private void ProcessReady(string uid, string roomid, int seat)
    {
        try
        {
            if (Globals.gameRooms.ContainsKey(roomid))
            {
                Globals.gameRooms[roomid].status[seat] = "ready";
                if (uid == Globals.gameToken.uid)
                {
                    Globals.gameRooms[roomid].gameStatus = 1;
                }
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("ProcessReady");
            LogHelper.AppLog(ex.ToString());
        }
    }

    private void ProcessChat(string uid, string roomid,string chat)
    {
        string name = "";
        if (Globals.gameRooms.ContainsKey(roomid))
        {
            if (Globals.gameRooms[roomid].gamers.ContainsKey(uid))
            {
                name = Globals.gameRooms[roomid].gamers[uid].name;
            }
        }
        
        if (!Globals.chatHistory.ContainsKey(roomid))
        {
            Globals.chatHistory.Add(roomid, name+" : "+chat);
        }
        else
        {
            Globals.chatHistory[roomid] = (name + " : " + chat)+ Globals.chatHistory[roomid];
        }
    }

}