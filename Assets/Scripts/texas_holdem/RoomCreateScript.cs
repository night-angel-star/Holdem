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

            if (ret.ContainsKey("roomid"))
            {
                //UnityMainThreadDispatcher.Instance().Enqueue(() =>
                //{
                //    StartCoroutine(DelayJoin(int.Parse(ret["roomid"].ToString())));
                //});
                
                JoinHandler(int.Parse(ret["roomid"].ToString()));
            }

            //UnityMainThreadDispatcher.Instance().Enqueue(() =>
            //{
            //    SceneManager.LoadScene("TexasHoldem");
            //});
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

    private IEnumerator DelayJoin(int roomid)
    {
        yield return new WaitForSeconds(1);
        JoinHandler(roomid);
    }


    private void CreateRoom()
    {
        if (Validate())
        {
            string uid = Globals.gameToken.uid;
            int pin = Globals.gameToken.pin;
            var data = new
            {
                uid = uid,
                pin = pin,
                args = new
                {
                    room_name = RoomNameInput.text,
                    seat = (seatSelectManager.GetComponent<SelectManager>().currentActive + 1) * 3,
                    room_type = roomTypeSelectManager.GetComponent<SelectManager>().currentActive
                },
                f = "mkroom"
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

    private void JoinHandler(int roomIndex)
    {
        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;
        var data = new
        {
            uid = uid,
            pin = pin,
            f = "enterroom",
            args = new
            {
                roomid = roomIndex
            }
        };
        Globals.socketIoConnection.SendRpc(data, OnJoinResponse);
    }

    private void OnJoinResponse(JToken jsonResponse)
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
    }

    void Update()
    {
        
    }
}
