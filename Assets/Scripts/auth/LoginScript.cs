using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;
using Newtonsoft.Json;
using System;
using EasyUI.Toast;
using GameRoomsT = System.Collections.Generic.Dictionary<string, Room>;

public class LoginScript : MonoBehaviour
{
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;
    public Button LoginButton;
    public Selectable[] inputFields = new Selectable[2];

    void Start()
    {
        inputFields[0] = EmailInput;
        inputFields[1] = PasswordInput;
        // Attach a listener to the login button
        LoginButton.onClick.AddListener(Login);
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
            try
            {
                Globals.userProfile = JsonConvert.DeserializeObject<Gamer>(profile.ToString());
            } catch (Exception error)
            {
                Debug.LogError(error);
            }
            Globals.gameToken = JsonConvert.DeserializeObject<Token>(token.ToString());
            Debug.Log(Globals.gameToken);
            Globals.username = EmailInput.text;
            Globals.password = PasswordInput.text;
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Globals.notification = new TournamentEngine();
                Globals.gameRooms = new GameRoomsT();
                Globals.currentRoom = null;
                Globals.chatHistory = new Dictionary<string, string>();
                Globals.tournamentInfo = new TournamentInfo();

                SceneManager.LoadScene("Home");
            });
            return;
        } while (false);

        // Display errorString
        if (errorString != "")
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Toast.Show(errorString,"danger");
            });
        }
    }
    private void Login()
    {
        if (Validate())
        {
            string enteredEmail = EmailInput.text;
            string enteredPassword = PasswordInput.text;

            var data = new
            {
                uid=enteredEmail,
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

    private bool ValidateEmail(string email)
    {
        string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        bool isValid = Regex.IsMatch(email, pattern);
        return isValid;
    }
    private bool Validate()
    {
        string errorString = null;
        
        errorString = string.IsNullOrEmpty(errorString) && string.IsNullOrEmpty(EmailInput.text) ? "Please enter your email" : null;
        errorString = string.IsNullOrEmpty(errorString) && string.IsNullOrEmpty(PasswordInput.text) ? "Please enter your password" : null;
        if (string.IsNullOrEmpty(errorString) && !ValidateEmail(EmailInput.text))
        {
            Toast.Show("Please enter the valid email","danger");
            return false;
        }
        
        if (string.IsNullOrEmpty(errorString))
        {
            return true;
        }
        else
        {
            Toast.Show(errorString, "danger");
            return false;
        }
    }

    private void Update()
    {
        TabSelect.TabKeyDown(inputFields);
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Login();
        }
    }

    
}
