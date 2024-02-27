using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class leaveScript : MonoBehaviour
{
    public void OnButtonClick()
    {
        Dictionary<string, object> token = (Dictionary<string, object>)Globals.token;
        string uid = token["uid"].ToString();
        int pin = Int32.Parse(token["pin"].ToString());
        var data = new
        {
            args = "0",
            f = "leave",
            pin = pin, 
            uid = uid,
        };
        Globals.socketIoConnection.SendRpc(data, OnResponse);
    }

    private void OnResponse(JToken jsonResponse)
    {
        string errorString = "";
        Dictionary<string, object> res = JsonResponse.ToDictionary(jsonResponse);

        do
        {
            if (res == null)
            {
                errorString = "Invalid response";
                break;
            }
            int err = res["err"].ConvertTo<int>();
            if (err != 0)
            {
                if (!res.ContainsKey("ret"))
                {
                    errorString = "Invalid response";
                    break;
                }
                errorString = res["ret"].ToString();
                break;
            }
            if (!res.ContainsKey("ret"))
            {
                errorString = "Invalid response";
                break;
            }
            Dictionary<string, object> ret = JsonResponse.ToDictionary(res["ret"]);
            if (ret == null)
            {
                errorString = "Invalid response";
                break;
            }
            int roomId = Int32.Parse(Globals.myRoom["id"].ToString());
            int roomIndex = Array.IndexOf(Globals.roomIdArray, roomId);
            Globals.rooms[roomIndex] = null;
            Globals.roomIdArray[roomIndex] = -1;

            Globals.myRoom =
                new Dictionary<string, object>
                {
                    { "id", -1 },
                    { "leave", null },
                    { "ready", null },
                    { "unseat", null },
                    { "fold", null },
                    { "raise", null },
                    { "call", null },
                    { "check", null },
                    { "takeseat", null },
                };

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                SceneManager.LoadScene("TexasHoldem");
            });
            return;
        } while (false);
    }
}
