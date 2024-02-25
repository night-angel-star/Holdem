using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileHearderScript : MonoBehaviour
{
    public TMP_Text UsernameText;
    public TMP_Text CoinText;

    // Start is called before the first frame update
    void Start()
    {
        Dictionary<string, object> profile = (Dictionary<string, object>)Globals.profile;
        UsernameText.text = profile["name"].ToString();
        CoinText.text = profile["coins"].ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
