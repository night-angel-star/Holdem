using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System;
using Newtonsoft.Json;

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

        EmailInput.onValueChanged.AddListener((v) => Restore(v, EmailInput));
        PasswordInput.onValueChanged.AddListener((v) => Restore(v, PasswordInput));
    }

    private void Login()
    {
        if (Validate())
        {
            var socket = SocketManager.socket;
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

            string jsonData = JsonConvert.SerializeObject(data);

            socket.Emit("rpc", jsonData);

            // if (enteredEmail == correctEmail && enteredPassword == correctPassword)
            // {
            //     print("Login successful!");
            //     SceneManager.LoadScene("Home");
            // }
            // else
            // {
            //     print("Login failed!");
            // }
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
        if(!string.IsNullOrEmpty(EmailInput.text) && !ValidateEmail(EmailInput.text))
        {
            EmailError.text = "Please enter the valid email";
        }
        PasswordInput.GetComponent<Image>().color = string.IsNullOrEmpty(PasswordInput.text) ? Color.red : Color.white;
        PasswordError.text = string.IsNullOrEmpty(PasswordInput.text) ? "Please enter your password" : "";
        if(!string.IsNullOrEmpty(EmailInput.text) && !string.IsNullOrEmpty(PasswordInput.text))
        {
            return true;
        } else {
            return false;
        }
    }

    public void Restore(string value, TMP_InputField InputField)
    {
        InputField.GetComponent<Image>().color = Color.white;
        if(InputField == EmailInput) 
        {
            EmailError.text = "";
        } else
        {
            PasswordError.text = "";
        }
    }
}
