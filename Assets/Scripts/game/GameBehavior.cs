using Newtonsoft.Json.Linq;

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class GameBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    public string roomName = "ROOM-1";
    public TMP_Text roomNameObject;

    public GameObject myCards;
    public GameObject usersParent;


    public int[] myCardsNumber = new int[] { 53, 18 };
    public Dictionary<string, string>[] usersInfo;

    void Start()
    {


    }
    // Update is called once per frame
    void Update()
    {
        GetMyCard(myCardsNumber);
        makeFakeData();
        SetUserInfo();
        SetRoomName();
    }

    private void OnRpcResponse(JToken jsonResponse)
    {
        if (jsonResponse == null)
        {
            return;
        }
    }
    

    void makeFakeData()
    {

        //For Test
        string user1AvatarIndex = "0";
        string user1Name = "CHARLIE1";
        string user1Chips = "12323";

        string user2AvatarIndex = "1";
        string user2Name = "CHARLIE2";
        string user2Chips = "12323";

        string user3AvatarIndex = "2";
        string user3Name = "CHARLIE3";
        string user3Chips = "12323";

        string user4AvatarIndex = "3";
        string user4Name = "CHARLIE4";
        string user4Chips = "12323";

        string user5AvatarIndex = "4";
        string user5Name = "CHARLIE5";
        string user5Chips = "12323";

        string user6AvatarIndex = "5";
        string user6Name = "CHARLIE6";
        string user6Chips = "12323";

        string user7AvatarIndex = "6";
        string user7Name = "CHARLIE7";
        string user7Chips = "12323";

        string user8AvatarIndex = "7";
        string user8Name = "CHARLIE8";
        string user8Chips = "12323";

        string user9AvatarIndex = "8";
        string user9Name = "CHARLIE9";
        string user9Chips = "12323";
        usersInfo = new Dictionary<string, string>[9];
        for (int i = 0; i < usersInfo.Length; i++)
        {
            usersInfo[i] = new Dictionary<string, string>();
        }

        usersInfo[0].Add("avatar", user1AvatarIndex);
        usersInfo[0].Add("name", user1Name);
        usersInfo[0].Add("chips", user1Chips);

        usersInfo[1].Add("avatar", user2AvatarIndex);
        usersInfo[1].Add("name", user2Name);
        usersInfo[1].Add("chips", user2Chips);

        usersInfo[2].Add("avatar", user3AvatarIndex);
        usersInfo[2].Add("name", user3Name);
        usersInfo[2].Add("chips", user3Chips);

        usersInfo[3].Add("avatar", user4AvatarIndex);
        usersInfo[3].Add("name", user4Name);
        usersInfo[3].Add("chips", user4Chips);

        usersInfo[4].Add("avatar", user5AvatarIndex);
        usersInfo[4].Add("name", user5Name);
        usersInfo[4].Add("chips", user5Chips);

        usersInfo[5].Add("avatar", user6AvatarIndex);
        usersInfo[5].Add("name", user6Name);
        usersInfo[5].Add("chips", user6Chips);

        usersInfo[6].Add("avatar", user7AvatarIndex);
        usersInfo[6].Add("name", user7Name);
        usersInfo[6].Add("chips", user7Chips);

        usersInfo[7].Add("avatar", user8AvatarIndex);
        usersInfo[7].Add("name", user8Name);
        usersInfo[7].Add("chips", user8Chips);

        usersInfo[8].Add("avatar", user9AvatarIndex);
        usersInfo[8].Add("name", user9Name);
        usersInfo[8].Add("chips", user9Chips);
    }

    void GetMyCard(int[] cardNumber)
    {
        myCards.SetActive(true);
        GameObject card1 = myCards.transform.GetChild(0).gameObject;
        GameObject card2 = myCards.transform.GetChild(1).gameObject;
        card1.GetComponent<SpriteRenderer>().sprite = CardHelper.GetCard(cardNumber[0], card1.GetComponent<SpriteRenderer>().sprite);
        card2.GetComponent<SpriteRenderer>().sprite = CardHelper.GetCard(cardNumber[1], card2.GetComponent<SpriteRenderer>().sprite);
    }

    void SetUserInfo()
    {

        GameObject[] usersArray = GameObjectHelper.GetActiveChildren(usersParent);
        for (int i = 0; i < usersArray.Length; i++)
        {
            GameObject avatar = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
            avatar.GetComponent<SpriteRenderer>().sprite = AvatarHelper.GetAvatar(usersInfo[i]["avatar"]);
            GameObject name = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;
            name.GetComponent<TMP_Text>().text = usersInfo[i]["name"];
            GameObject money = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject;
            money.GetComponent<TMP_Text>().text = usersInfo[i]["chips"] + " ₮";
        }
    }

    void SetRoomName()
    {
        roomNameObject.text = roomName;
    }


    


}
