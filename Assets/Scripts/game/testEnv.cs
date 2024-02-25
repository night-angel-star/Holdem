using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class testEnv : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Globals.socketIoConnection.serverUri = Globals.strUri;
        Globals.socketIoConnection.Connect();
        Login();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            var profile = ret["profile"];
            var token = ret["token"];
            Globals.profile = JsonResponse.ToDictionary(profile);
            Globals.token = JsonResponse.ToDictionary(token);

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                SceneManager.LoadScene("Home");
            });
            return;
        } while (false);

        // Display errorString
        if (errorString != "")
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                //ErrorText.text = errorString;
            });
        }
    }

    private void Login()
    {
        string enteredEmail = "qwe@qwe.qwe";
        string enteredPassword = "qweqweqwe";

        var data = new
        {
            args = new
            {
                uid = enteredEmail,
                passwd = enteredPassword
            },
            f = "login"
        };
        Globals.socketIoConnection.SendRpc(data, OnResponse);
    }
}
