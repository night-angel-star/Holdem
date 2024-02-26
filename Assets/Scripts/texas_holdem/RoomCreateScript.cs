using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class RoomCreateScript : MonoBehaviour
{
    public TMP_Text RoomNameError;
    public TMP_InputField RoomNameInput;
    public Button CreateButton;
    public GameObject roomTypeSelectManager;
    public GameObject seatSelectManager;

    // Start is called before the first frame update
    void Start()
    {
        RoomNameInput.onValueChanged.AddListener((v) => Restore(v, RoomNameInput, RoomNameError));
        CreateButton.onClick.AddListener(CreateRoom);
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

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                SceneManager.LoadScene("Room");
            });
            return;
        } while (false);

        // Display errorString
        if (errorString != "")
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                RoomNameError.text = errorString;
            });
        }
    }

    private void CreateRoom()
    {
        if (Validate())
        {
            Dictionary<string, object> token = (Dictionary<string, object>)Globals.token;
            string uid = token["uid"].ToString();
            int pin = Int32.Parse(token["pin"].ToString());
            var data = new
            {
                uid = uid,
                pin = pin,
                args = new
                {
                    is_new = "new",
                    type = "limit",
                    room_name = RoomNameInput.text,
                    seat = seatSelectManager.GetComponent<SelectManager>().currentActive,
                    room_type = roomTypeSelectManager.GetComponent<SelectManager>().currentActive
                },
                f = "entergame"
            };

            Globals.socketIoConnection.SendRpc(data, OnResponse);
        }
    }

    private bool Validate()
    {
        if (string.IsNullOrEmpty(RoomNameInput.text))
        {
            RoomNameInput.GetComponent<Image>().color = Color.red;
            RoomNameError.text = "Please enter the room name";
            return false;
        }
        if (!string.IsNullOrEmpty(RoomNameInput.text))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Restore(string value, TMP_InputField InputField, TMP_Text ErrorText)
    {
        InputField.GetComponent<Image>().color = Color.white;
        ErrorText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
