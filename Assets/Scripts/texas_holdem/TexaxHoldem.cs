using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TexasHoldem : MonoBehaviour
{
    public TMP_Text UsernameText;
    public TMP_Text CoinText;

    private int seats = 9;
    // Start is called before the first frame update
    void Start()
    {
        JContainer jContainer = Globals.profile as JContainer;
        UsernameText.text = jContainer.SelectToken("name").Value<string>();
        CoinText.text = jContainer.SelectToken("coins").Value<string>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
