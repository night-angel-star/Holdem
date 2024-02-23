using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;

public class LoginScript : MonoBehaviour
{
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;
    public TMP_Text EmailError;
    public TMP_Text PasswordError;
    public Button LoginButton;

    void Start()
    {
        // Attach a listener to the login button
        LoginButton.onClick.AddListener(Login);

        EmailInput.onValueChanged.AddListener((v) => Restore(v, EmailInput, EmailError));
        PasswordInput.onValueChanged.AddListener((v) => Restore(v, PasswordInput, PasswordError));
    }
     private void Login()
    {
        if (Validate())
        {
            var socket = Globals.socketIoUnity;
            string enteredEmail = EmailInput.text;
            string enteredPassword = PasswordInput.text;

            System.Random rnd = new System.Random();
            int seq = rnd.Next();

            var data = new
            {
                seq = seq,
                args = new
                {
                    uid = enteredEmail,
                    passwd = enteredPassword
                },
                f = "login"
            };

            //string jsonData = JsonConvert.SerializeObject(data);

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
                                JContainer retJContainer = jContainer.SelectToken("ret").Value<object>() as JContainer;
                                Globals.profile = retJContainer.SelectToken("profile").Value<object>();
                                Globals.token = retJContainer.SelectToken("token").Value<object>();
                                SceneManager.LoadScene("Home");
                            });
                        } else if(token.Value<int>() > 300)
                        {
                            JToken token1 = jContainer.SelectToken("ret");
                            if(token1.Value<string>().IndexOf("user") != -1)
                            {
                                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                                {
                                    EmailInput.GetComponent<Image>().color = Color.red;
                                    EmailError.text = token1.Value<string>();
                                });
                            } else if(token1.Value<string>().IndexOf("password") != -1)
                            {
                                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                                {
                                    PasswordInput.GetComponent<Image>().color = Color.red;
                                    PasswordError.text = token1.Value<string>();
                                });
                            }
                        }
                    }
                }
            });

            socket.Emit("rpc", data);
        }
    }

    public bool ValidateEmail(string email)
    {
        string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        bool isValid = Regex.IsMatch(email, pattern);
        return isValid;
    }
    public bool Validate()
    {
        EmailInput.GetComponent<Image>().color = string.IsNullOrEmpty(EmailInput.text) ? Color.red : Color.white;
        EmailError.text = string.IsNullOrEmpty(EmailInput.text) ? "Please enter your email" : "";
        PasswordInput.GetComponent<Image>().color = string.IsNullOrEmpty(PasswordInput.text) ? Color.red : Color.white;
        PasswordError.text = string.IsNullOrEmpty(PasswordInput.text) ? "Please enter your password" : "";
        if (!ValidateEmail(EmailInput.text))
        {
            EmailInput.GetComponent<Image>().color = Color.red;
            EmailError.text = "Please enter the valid email";
            return false;
        }
        if (!string.IsNullOrEmpty(EmailInput.text) && !string.IsNullOrEmpty(PasswordInput.text))
        {
            return true;
        } else {
            return false;
        }
    }

    public void Restore(string value, TMP_InputField InputField, TMP_Text ErrorText)
    {
        InputField.GetComponent<Image>().color = Color.white;
        ErrorText.text = "";
    }
}
