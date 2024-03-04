using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EasyUI.Toast;
using UnityEngine.SceneManagement;

public class GlobalNotification : MonoBehaviour
{
    public GlobalNotification()
    {
        Globals.socketIoConnection.AddNotifyHandler("status", OnStatus);
    }

    void OnStatus(JToken baseToken)
    {
        TournamentStatusNotifyEvent json = baseToken.ToObject<TournamentStatusNotifyEvent>();
        if (json.args.status == "delaying")
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Toast.Show("Tournament will start after " + json.args.timeleft + "s");
            });
        } else if(json.args.status == "started")
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Globals.socketIoConnection.RemoveNotifyHandler("status");
                SceneManager.LoadScene("RoomTournament");
            });
        }
    }
}
