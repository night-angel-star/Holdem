using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    
    public TMP_Text roomNameObject;
    public GameObject myCards;
    public GameObject usersParent;
    public GameObject chipMinBuyLimitObject;
    public GameObject chipMaxBuyLimitObject;
    public Slider chipsSliderObject;
    public GameObject publicCardArea;
    public GameObject sitToSeatArea;

    public string roomName;
    public int chipsMinBuy;
    public int chipsMaxBuy;
    public int[] myCardsNumber;
    public Dictionary<string, string>[] usersInfo;
    public int[] openedCards;

    public int sitPosition;

    public int currentActiveUser;
    public int currentTimeout;
    public int totalTime;

    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        setRoomData();

        if (sitPosition != -1)
        {
            GetMyCard(myCardsNumber);
        }
        SetUserInfo();
        SetRoomName();
        InitializeAddChipsModal();
        SetPublicCards();
        SetTimer();
    }

    private void OnRpcResponse(JToken jsonResponse)
    {
        if (jsonResponse == null)
        {
            return;
        }
    }
    

    void setRoomData()
    {
        string[] seats = NewtonSoftHelper.JArrayToArray<string>(Globals.rooms[0]["seats"]);
        Dictionary<string,object> gamers=NewtonSoftHelper.JArrayToObject<string,object>(Globals.rooms[0]["gamers"]);

        usersInfo = new Dictionary<string, string>[seats.Length];
        string avatarIndex, name, walletChips,chips;
        for (int i = 0; i < seats.Length; i++)
        {
            usersInfo[i] = new Dictionary<string, string>();
            if (seats[i] != null)
            {
                Dictionary<string,string> gamer = NewtonSoftHelper.JArrayToObject<string,string>(gamers[seats[i]]);
                usersInfo[i].Add("avatar", gamer["avatar"]);
                usersInfo[i].Add("name", gamer["name"]);
                usersInfo[i].Add("walletChips", gamer["coins"]);
                usersInfo[i].Add("chips", "1200000");
            }
        }

    }

    void GetMyCard(int[] cardNumber)
    {
        myCards.SetActive(true);
        GameObject card1 = myCards.transform.GetChild(0).gameObject;
        GameObject card2 = myCards.transform.GetChild(1).gameObject;
        card1.GetComponent<SpriteRenderer>().sprite = CardHelper.GetCard(cardNumber[0]);
        card2.GetComponent<SpriteRenderer>().sprite = CardHelper.GetCard(cardNumber[1]);
    }

    void SetUserInfo()
    {
        if (sitPosition == -1)
        {
            GameObject[] usersArray = GameObjectHelper.GetChildren(usersParent);
            GameObject[] sitButtons = GameObjectHelper.GetChildren(sitToSeatArea);
            initalizeUsers(usersArray);
            initalizeSitButtons(sitButtons);
            for (int i = 0; i < usersArray.Length; i++)
            {
                if (usersInfo[i].Count != 0)
                {
                    
                    if (i == 0)
                    {
                        usersArray[i].transform.GetChild(1).gameObject.SetActive(true);
                        //usersArray[i].transform.GetChild(2).gameObject.SetActive(true);
                    }
                    else
                    {
                        usersArray[i].transform.GetChild(1).gameObject.SetActive(true);
                        //usersArray[i].transform.GetChild(2).gameObject.SetActive(true);
                        //usersArray[i].transform.GetChild(3).gameObject.SetActive(true);
                    }
                    GameObject avatar = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
                    avatar.GetComponent<SpriteRenderer>().sprite = AvatarHelper.GetAvatar(usersInfo[i]["avatar"]);
                    GameObject name = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;
                    name.GetComponent<TMP_Text>().text = usersInfo[i]["name"];
                    GameObject wallet_money = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject;
                    wallet_money.GetComponent<TMP_Text>().text = usersInfo[i]["walletChips"] + " ₮";
                    if (i == 0)
                    {

                    }
                    else
                    {
                        //GameObject money = usersArray[i].transform.GetChild(3).gameObject.transform.GetChild(1).gameObject;
                        //money.GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated(long.Parse(usersInfo[i]["chips"]));
                    }
                }
                else
                {
                    sitButtons[i].SetActive(true);
                }
            }
        }
        else
        {
            GameObject[] usersArray = GameObjectHelper.GetChildren(usersParent);
            GameObject[] sitButtons = GameObjectHelper.GetChildren(sitToSeatArea);
            Dictionary<string, string>[] rotatedUserInfo = ArrayHelper.RotateArray(usersInfo, sitPosition);
            initalizeUsers(usersArray);
            initalizeSitButtons(sitButtons);
            for (int i = 0; i < usersArray.Length; i++)
            {
                if (rotatedUserInfo[i].Count != 0)
                {
                    
                    if (i == 0)
                    {
                        usersArray[i].transform.GetChild(1).gameObject.SetActive(true);
                        usersArray[i].transform.GetChild(2).gameObject.SetActive(true);
                    }
                    else
                    {
                        usersArray[i].transform.GetChild(1).gameObject.SetActive(true);
                        usersArray[i].transform.GetChild(2).gameObject.SetActive(true);
                        usersArray[i].transform.GetChild(3).gameObject.SetActive(true);
                    }
                    GameObject avatar = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
                    avatar.GetComponent<SpriteRenderer>().sprite = AvatarHelper.GetAvatar(rotatedUserInfo[i]["avatar"]);
                    GameObject name = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;
                    name.GetComponent<TMP_Text>().text = rotatedUserInfo[i]["name"];
                    GameObject wallet_money = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject;
                    wallet_money.GetComponent<TMP_Text>().text = rotatedUserInfo[i]["walletChips"] + " ₮";
                    if (i == 0)
                    {

                    }
                    else
                    {
                        GameObject money = usersArray[i].transform.GetChild(3).gameObject.transform.GetChild(1).gameObject;
                        money.GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated(long.Parse(rotatedUserInfo[i]["chips"]));
                    }
                }
            }
        }

    }

    void initalizeUsers(GameObject[] usersArray)
    {
        for(int i = 0; i < usersArray.Length; i++)
        {
            if (i == 0)
            {
                usersArray[i].transform.GetChild(1).gameObject.SetActive(false);
                usersArray[i].transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                usersArray[i].transform.GetChild(1).gameObject.SetActive(false);
                usersArray[i].transform.GetChild(2).gameObject.SetActive(false);
                usersArray[i].transform.GetChild(3).gameObject.SetActive(false);
            }
        }
    }

    void initalizeSitButtons(GameObject[] sitButtons)
    {
        for(int i=0;i< sitButtons.Length; i++)
        {
            sitButtons[i].SetActive(false);
        }
    }

    void SetRoomName()
    {
        roomNameObject.text = roomName;
    }

    void InitializeAddChipsModal()
    {
        chipMinBuyLimitObject.GetComponent<TMP_Text>().text = chipsMinBuy.ToString();
        chipMaxBuyLimitObject.GetComponent<TMP_Text>().text=chipsMaxBuy.ToString();
        chipsSliderObject.minValue = chipsMinBuy;
        chipsSliderObject.maxValue= chipsMaxBuy;
        
    }

    void SetPublicCards()
    {
        GameObject[] publicCards= GameObjectHelper.GetChildren(publicCardArea);
        for (int i = 0; i < openedCards.Length; i++)
        {
            GameObject publicCard=publicCards[i].transform.GetChild(0).gameObject;
            publicCard.GetComponent<SpriteRenderer>().sprite = CardHelper.GetCard(openedCards[i]);
        }
    }

    public void sitToRoom(int index)
    {
        sitPosition = index;

        Dictionary<string, object> token = (Dictionary<string, object>)Globals.token;
        string uid = token["uid"].ToString();
        int pin = Int32.Parse(token["pin"].ToString());
        var data = new
        {
            uid = uid,
            pin = pin,
            f = "takeseat",
            args = index.ToString(),
        };
        Globals.socketIoConnection.SendRpc(data, OnResponse);
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
            return;
        } while (false);
    }

    public void SetTimer()
    {
        if (sitPosition != -1)
        {
            GameObject[] usersArray = GameObjectHelper.GetChildren(usersParent);
            Dictionary<string, string>[] rotatedUserInfo = ArrayHelper.RotateArray(usersInfo, sitPosition);
            int currentActiveUserRotated = ArrayHelper.RotateNumber(currentActiveUser, sitPosition, usersInfo.Length);
            for (int i = 0; i < rotatedUserInfo.Length; i++)
            {
                GameObject progressbar = usersArray[i].transform.GetChild(1).transform.GetChild(2).transform.GetChild(0).gameObject;
                if (i == currentActiveUserRotated)
                {
                    progressbar.GetComponent<Image>().fillAmount = ((float)currentTimeout / totalTime);
                }
                else
                {
                    progressbar.GetComponent<Image>().fillAmount = 0;
                }
            }
        }
    }




}
