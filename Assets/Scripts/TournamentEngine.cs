using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EasyUI.Toast;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class TournamentEngine : MonoBehaviour
{
    public bool joinedTournament = false;
    public TournamentEngine()
    {
        Globals.socketIoConnection.AddNotifyHandler("status", OnStatus);
        Globals.socketIoConnection.AddNotifyHandler("new_blind",OnNewBlind);
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
}
