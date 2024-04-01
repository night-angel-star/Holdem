using EasyUI.Toast;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeUserProfileBehavior : MonoBehaviour
{
    public TMP_InputField newUserNameInput;
    public TMP_InputField currentPasswordInput;
    public TMP_InputField newPasswordInput;
    public TMP_InputField confirmPasswordInput;
    public TMP_InputField newPhoneNumberInput;
    public GameObject AvatarsParent;
    public static string tempAvatarIndex=Globals.userProfile.avatar;

    private void Update()
    {
        UpdateAvatars();
    }

    void UpdateAvatars()
    {
        if (AvatarsParent != null)
        {
            GameObject[] avatarsObject = GameObjectHelper.GetChildren(AvatarsParent);
            for(int i=0;i<avatarsObject.Length;i++)
            {
                //avatarsObject[i].
                if (i == int.Parse(tempAvatarIndex))
                {
                    avatarsObject[i].transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    avatarsObject[i].transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }

    public void ChangeUserName()
    {
        if (newUserNameInput.text != "")
        {
            var data = new
            {
                uid = Globals.gameToken.uid,
                pin = Globals.gameToken.pin,
                args = new
                {
                    name = newUserNameInput.text
                },
                f = "changeprofile"
            };
            Globals.socketIoConnection.SendRpc(data, OnChangeUserNameResponse);
        }
        else
        {
            Toast.Show("Please fill out every input.", "danger");
        }
    }

    private void OnChangeUserNameResponse(JToken jsonResponse)
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
            //Todo:For future process
            Globals.userProfile.name = newUserNameInput.text;
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    public void ChangePassword()
    {
        if (currentPasswordInput.text != "" && newPasswordInput.text != "" && confirmPasswordInput.text != "")
        {
            var data = new
            {
                uid = Globals.gameToken.uid,
                pin = Globals.gameToken.pin,
                args = new
                {
                    passwd1 = currentPasswordInput.text,
                    passwd = newPasswordInput.text
                },
                f = "changeprofile"
            };
            Globals.socketIoConnection.SendRpc(data, OnChangePasswordResponse);
        }
        else
        {
            Toast.Show("Please fill out every input.", "danger");
        }
    }

    private void OnChangePasswordResponse(JToken jsonResponse)
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
            //Todo:For future process
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    public void ChangePhoneNumber()
    {
        if(newPhoneNumberInput.text != "")
        {
            var data = new
            {
                uid = Globals.gameToken.uid,
                pin = Globals.gameToken.pin,
                args = new
                {
                    phone = newPhoneNumberInput.text
                },
                f = "changeprofile"
            };
            Globals.socketIoConnection.SendRpc(data, OnChangePhoneNumberResponse);
        }
        else
        {
            Toast.Show("Please fill out every input.", "danger");
        }
    }

    private void OnChangePhoneNumberResponse(JToken jsonResponse)
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
            //Todo:For future process
            Globals.userProfile.phoneNumber = newPhoneNumberInput.text;
            UnityMainThreadDispatcher.Instance().Enqueue(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().name); });
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

    public void ChangeAvatar()
    {
        var data = new
        {
            uid = Globals.gameToken.uid,
            pin = Globals.gameToken.pin,
            args = new
            {
                avatar = tempAvatarIndex

            },
            f = "changeprofile"
        };
        Globals.socketIoConnection.SendRpc(data, OnChangeAvatarResponse);
    }

    private void OnChangeAvatarResponse(JToken jsonResponse)
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
            //Todo:For future process
            Globals.userProfile.avatar = tempAvatarIndex;
            UnityMainThreadDispatcher.Instance().Enqueue(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().name); });
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

    public void ChangeTempAvaterIndex(string index)
    {
        tempAvatarIndex = index;
    }
}
