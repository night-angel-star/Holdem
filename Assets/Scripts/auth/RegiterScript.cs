using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using EasyUI.Toast;

public class RegisterScript : MonoBehaviour
{
    public TMP_InputField NameInput;
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;
    public TMP_InputField Password2Input;
    public Button RegisterButton;
    public Selectable[] inputFields = new Selectable[4];

    void Start()
    {
        inputFields[0] = NameInput;
        inputFields[1] = EmailInput;
        inputFields[2] = PasswordInput;
        inputFields[3] = Password2Input;
        // Attach a listener to the login button
        RegisterButton.onClick.AddListener(Register);
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
                Toast.Show("Account created successfully.");
                SceneManager.LoadScene("Login");
            });
            return;
        } while (false);

        // Display errorString
        if (errorString != "")
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Toast.Show(errorString, "danger");
            });
        }
    }
    private void Register()
    {
        if (Validate())
        {
            string enteredName = NameInput.text;
            string enteredEmail = EmailInput.text;
            string enteredPassword = PasswordInput.text;
            string enteredPassword2 = Password2Input.text;

            var data = new
            {
                args = new
                {
                    name = enteredName,
                    uid = enteredEmail,
                    email = enteredEmail,
                    passwd = enteredPassword
                },
                f = "signup"
            };

            Globals.socketIoConnection.SendRpc(data, OnResponse);
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
        string errorString = null;
        if (string.IsNullOrEmpty(errorString) && string.IsNullOrEmpty(EmailInput.text))
        {
            errorString = string.IsNullOrEmpty(NameInput.text) ? "Please enter your name" : null;
        }
        if (string.IsNullOrEmpty(errorString) && string.IsNullOrEmpty(EmailInput.text))
        {
            errorString = string.IsNullOrEmpty(EmailInput.text) ? "Please enter your email" : null;
        }
        if (string.IsNullOrEmpty(errorString) && string.IsNullOrEmpty(PasswordInput.text))
        {
            errorString = string.IsNullOrEmpty(PasswordInput.text) ? "Please enter your password" : null;
        }
        if (string.IsNullOrEmpty(errorString) && !ValidateEmail(EmailInput.text))
        {
            errorString = "Please enter the valid email";
        }
        if (string.IsNullOrEmpty(errorString) && PasswordInput.text != Password2Input.text)
        {
            errorString = "Password is not match";
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
    }
}

