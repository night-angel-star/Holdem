using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ProfileHearderScript : MonoBehaviour
{
    public TMP_Text UsernameText;
    public TMP_Text CoinText;

    public GameObject Avatar;

    // Start is called before the first frame update
    void Start()
    {
        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;
        var data = new
        {
            uid = uid,
            pin = pin,
            f = "profile",
            args = 0,
        };
        Globals.socketIoConnection.SendRpc(data, OnProfile);
        
    }

    void OnProfile(JToken jsonResponse)
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

            if (ret.ContainsKey("name"))
            {
                Globals.userProfile.name = ret["name"].ToString();

                Debug.Log("set name");
                Debug.Log(ret["name"].ToString());
            }
            if (ret.ContainsKey("deposite"))
            {
                Globals.userProfile.deposite = int.Parse(ret["deposite"].ToString());

                Debug.Log("set deposite");
                Debug.Log(int.Parse(ret["deposite"].ToString()));
            }
            if (ret.ContainsKey("avatar"))
            {
                Globals.userProfile.avatar = ret["avatar"].ToString();

                Debug.Log("set avatar");
                Debug.Log(ret["avatar"].ToString());
            }

            return;
        } while (false);

        Debug.Log(errorString);
        
    }

    // Update is called once per frame
    void Update()
    {
        UsernameText.text = Globals.userProfile.name;
        CoinText.text = Globals.userProfile.deposite.ToString();
        Avatar.GetComponent<SpriteRenderer>().sprite = AvatarHelper.GetAvatar(Globals.userProfile.avatar.ToString());
    }
}
