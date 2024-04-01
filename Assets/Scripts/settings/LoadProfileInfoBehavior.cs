using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadProfileInfoBehavior : MonoBehaviour
{
    public TMP_Text balance;
    public GameObject avatar;
    public GameObject userName;
    public GameObject phoneNumber;
    public void LoadProfileInfo()
    {
        balance.text = Globals.userProfile.deposite.ToString();
        avatar.GetComponent<SpriteRenderer>().sprite = AvatarHelper.GetAvatar(Globals.userProfile.avatar);
        userName.GetComponent<TMP_InputField>().text = Globals.userProfile.name;
        phoneNumber.GetComponent<TMP_InputField>().text = Globals.userProfile.phoneNumber;
    }
}
