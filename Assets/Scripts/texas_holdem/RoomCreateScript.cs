using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

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

    private void CreateRoom()
    {
        if (Validate())
        {
            var socket = Globals.socketIoUnity;
            System.Random rnd = new System.Random();
            int seq = rnd.Next();
            JContainer profileJContainer = Globals.profile as JContainer;
            JContainer tokenJContainer = Globals.token as JContainer;
            string uid = profileJContainer.SelectToken("uid").Value<string>();
            int pin = tokenJContainer.SelectToken("pin").Value<int>();
            var data = new
            {
                seq = seq,
                uid = uid,
                pin = pin,
                args = new
                {
                    type = "limit",
                    room_name = RoomNameInput.text,
                    seat = seatSelectManager.GetComponent<SelectManager>().currentActive,
                    room_type = roomTypeSelectManager.GetComponent<SelectManager>().currentActive
                },
                f = "mkroom"
            };

            socket.On("rpc_ret", (response) =>
            {
                object data = JsonConvert.DeserializeObject(response.ToString());

                IEnumerable e = data as IEnumerable;
                object[] arr = e.Cast<object>().ToArray();
                if (arr != null)
                {
                    var res = arr.GetValue(0);
                    JContainer jContainer = res as JContainer;
                    if (jContainer != null)
                    {
                        JToken token = jContainer.SelectToken("err");
                        if (token.Value<int>() == 0)
                        {
                            UnityMainThreadDispatcher.Instance().Enqueue(() =>
                            {
                                SceneManager.LoadScene("Room");
                            });
                        }
                    }
                }
            });

            socket.Emit("rpc", data);
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
