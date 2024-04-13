using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EasyUI.Toast;
using UnityEngine.SceneManagement;
using System;
using static TournamentWinnerNotifyEvent;

public class RankingComparer : IComparer<TournamentWinners>
{
    public int Compare(TournamentWinners x, TournamentWinners y)
    {
        return x.ranking.CompareTo(y.ranking);
    }
}

public class TournamentEngine : MonoBehaviour
{
    public bool joinedTournament = false;
    public TournamentEngine()
    {
        Globals.socketIoConnection.AddNotifyHandler("status", OnStatus);
        Globals.socketIoConnection.AddNotifyHandler("new_blind",OnNewBlind);
        Globals.socketIoConnection.AddNotifyHandler("tournament_winner", OnTournamentWinner);
    }
    

    void OnStatus(JToken baseToken)
    {
        TournamentStatusNotifyEvent json = baseToken.ToObject<TournamentStatusNotifyEvent>();
        if (json != null)
        {
            if (json.args.status == "delaying")
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Toast.Show("Tournament will start after " + json.args.timeleft + "s");
                });
            }
            else if (json.args.status == "started")
            {
                if (!joinedTournament)
                {
                    joinedTournament = true;
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        //Globals.socketIoConnection.RemoveNotifyHandler("status");
                        SceneManager.LoadScene("RoomTournament");
                    });
                }
                else
                {
                    Globals.tournamentInfo.timeleft = json.args.timeleft;
                }

            }
        }
        
    }

    void OnNewBlind(JToken baseToken)
    {
        TournamentNewBlindNotifyEvent json=baseToken.ToObject<TournamentNewBlindNotifyEvent>();
        if (json != null)
        {
            Globals.tournamentInfo.small_bilnd = json.args.new_small_blind;
        }
    }

    void OnTournamentWinner(JToken baseToken)
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

                TournamentWinnerNotifyEvent json = baseToken.ToObject<TournamentWinnerNotifyEvent>();
                if (json != null)
                {
                    ProcessTournamentWinner(json.args.tournament_winners);
                }

            } while (false);
            if (errorString != "")
            {
                Debug.Log(errorString);
            }

        }
        catch (Exception ex)
        {
            LogHelper.AppLog("OnTournamentWinner");
            LogHelper.AppLog(ex.ToString());
        }
    }

    private void ProcessTournamentWinner(TournamentWinnerNotifyEvent.TournamentWinners[] args)
    {
        try
        {
            string resultStr = "";
            Array.Sort(args, new RankingComparer());
            for (int i = 0; i <= args.Length; i++)
            {
                resultStr += ("#" + args[i].ranking+":" + args[i].name+"(prize:" + args[i].tournament_prize+")");
                if (i != 6)
                {
                    resultStr = resultStr + ", ";
                }
            }
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Toast.Show(resultStr);
            });
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("ProcessTournamentWinner");
            LogHelper.AppLog(ex.ToString());
        }
    }
}
