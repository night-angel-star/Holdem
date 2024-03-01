using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileHearderScript : MonoBehaviour
{
    public TMP_Text UsernameText;
    public TMP_Text CoinText;

    public GameObject Avatar;

    // Start is called before the first frame update
    void Start()
    {
        UsernameText.text = Globals.userProfile.name;
        CoinText.text = Globals.userProfile.deposite.ToString();
        Avatar.GetComponent<SpriteRenderer>().sprite = AvatarHelper.GetAvatar(Globals.userProfile.avatar.ToString());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
