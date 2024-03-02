using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarHelper : MonoBehaviour
{
    public static Sprite GetAvatar(string index)
    {
        
        string avatarIndex = index == "" ? "0" : index;
        Sprite[] cardSprite = Resources.LoadAll<Sprite>("Images/avatar");

        return cardSprite[int.Parse(avatarIndex)];
    }
}
