using System;
using UnityEditor;
using UnityEngine;

public class CardHelper:MonoBehaviour
{
    public static Sprite GetCard(int nCardNumber)
    {
        try
        {
            int cardColor = (int)Math.Floor((decimal)(nCardNumber / 16)) - 1;
            int cardNumber = nCardNumber % 16;
            string[] cardColorString = new string[] { "diamond", "clubs", "hearts", "spades" };
            string[] cardNumberString = new string[] { "?", "", "2", "3", "4", "5", "6", "7", "8", "9", "10", "jack", "queen", "king", "ace" };
            Sprite cardSprite = Resources.Load<Sprite>("Images/cards/" + cardNumberString[cardNumber] + "_of_" + cardColorString[cardColor]);

            return cardSprite;
        }
        catch (Exception)
        {
            Sprite blankCardSprite = Resources.Load<Sprite>("Images/cards/place");
            return blankCardSprite;
        }
        
    }
}
