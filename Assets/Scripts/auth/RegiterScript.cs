using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;

public class RegisterScript : MonoBehaviour
{
    public TMP_InputField NameInput;
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;
    public TMP_InputField Password2Input;
    public TMP_Text NameError;
    public TMP_Text EmailError;
    public TMP_Text PasswordError;
    public TMP_Text Password2Error;
    public Button RegisterButton;

    void Start()
    {
        // Attach a listener to the login button
        RegisterButton.onClick.AddListener(Register);

        NameInput.onValueChanged.AddListener((v) => Restore(v, NameInput, NameError));
        EmailInput.onValueChanged.AddListener((v) => Restore(v, EmailInput, EmailError));
        PasswordInput.onValueChanged.AddListener((v) => Restore(v, PasswordInput, PasswordError));
        Password2Input.onValueChanged.AddListener((v) => Restore(v, Password2Input, Password2Error));
    }
    private void Register()
    {
        if (Validate())
        {
            var socket = Globals.socketIoUnity;
            string enteredName = NameInput.text;
            string enteredEmail = EmailInput.text;
            string enteredPassword = PasswordInput.text;
            string enteredPassword2 = Password2Input.text;

            System.Random rnd = new System.Random();
            int seq = rnd.Next();

            var data = new
            {
                seq = seq,
                args = new
                {
                    name = enteredName,
                    uid = enteredEmail,
                    email = enteredEmail,
                    passwd = enteredPassword
                },
                f = "signup"
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
                          if (seq != jContainer.SelectToken("seq").Value<int>())
                          {
                              JContainer retJContainer = jContainer.SelectToken("ret").Value<object>() as JContainer;
                              Globals.profile = retJContainer.SelectToken("profile").Value<object>();
                              SceneManager.LoadScene("Home");
                          }
                          else
                          {
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
                              socket.Emit("rpc", data);
                          }

                      });
                        }
                        else if (token.Value<int>() > 300)
                        {
                            JToken token1 = jContainer.SelectToken("ret");
                            if (token1.Value<string>().IndexOf("user") != -1)
                            {
                                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                            {
                            EmailInput.GetComponent<Image>().color = Color.red;
                            EmailError.text = token1.Value<string>();
                        });
                            }
                            else if (token1.Value<string>().IndexOf("password") != -1)
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
        int num = 0;
        NameInput.GetComponent<Image>().color = string.IsNullOrEmpty(NameInput.text) ? Color.red : Color.white;
        NameError.text = string.IsNullOrEmpty(NameInput.text) ? "Please enter your name" : "";
        EmailInput.GetComponent<Image>().color = string.IsNullOrEmpty(EmailInput.text) ? Color.red : Color.white;
        EmailError.text = string.IsNullOrEmpty(EmailInput.text) ? "Please enter your email" : "";
        PasswordInput.GetComponent<Image>().color = string.IsNullOrEmpty(PasswordInput.text) ? Color.red : Color.white;
        PasswordError.text = string.IsNullOrEmpty(PasswordInput.text) ? "Please enter your password" : "";
        if (PasswordInput.text != Password2Input.text)
        {
            Password2Input.GetComponent<Image>().color = Color.red;
            Password2Error.text = "Password is not match";
            num++;
        }
        if (!ValidateEmail(EmailInput.text))
        {
            EmailInput.GetComponent<Image>().color = Color.red;
            EmailError.text = "Please enter the valid email";
            num++;
        }
        if (string.IsNullOrEmpty(EmailInput.text) || string.IsNullOrEmpty(PasswordInput.text) || string.IsNullOrEmpty(NameInput.text))
        {
            num++;
        }
        if (num == 0)
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
}
