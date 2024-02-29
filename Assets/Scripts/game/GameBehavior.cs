using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;


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
    public GameObject ActionButtonsArea;

    public GameObject raiseBar;
    public GameObject raiseBarGradeParent;
    public GameObject raiseToValue;

    public string roomName;
    public int chipsMinBuy;
    public int chipsMaxBuy;
    public int[] myCardsNumber;
    public Dictionary<string, object>[] usersInfo;
    public int[] openedCards = new int[] { 0, 0 };

    public int sitPosition;
    private int sitPositionTemp;

    public int currentActiveUser;
    public int currentTimeout;
    public int totalTime = 20;

    public int minRaise = 10000;
    public int maxRaise = 70000;
    public int raiseAmount = 10000;

    public int actionButtonAreaIndex = -1;
    public int currentRoomId = -1;
    public int currentRoomIndex = -1;
    public bool gameStarted = false;

    public bool sitOutNextHandButtonEnabled = false;
    public bool sitOutNextBigBlindButtonEnabled = false;
    public bool callAnyButtonEnabled = false;


    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        
        //read from global
        SetRoomData();
        SetActionButtonArea();
        //draw ui
        if (sitPosition != -1)
        {
            GetMyCard(myCardsNumber);
        }
        SetUserInfo();
        SetRoomName();
        InitializeAddChipsModal();
        SetPublicCards();
        SetTimer();
        SetActionButtonAreaIndexByGlobal();
        SetRaiseBar();
        CheckRaiseAmount();
    }

    void SetRoomData()
    {
        currentRoomId = Globals.currentRoomId;

        currentRoomIndex = NewtonSoftHelper.GetIndexFromJArray(Globals.rooms, "id", Globals.currentRoomId.ToString());
        //room info
        roomName = Globals.rooms[currentRoomIndex]["name"] as string;
        Dictionary<string, object> options = NewtonSoftHelper.JObjectToObject<string, object>(Globals.rooms[currentRoomIndex]["options"]);
        chipsMinBuy = int.Parse(options["min_buy"].ToString());
        chipsMaxBuy = int.Parse(((Dictionary<string, object>)Globals.profile)["deposite"].ToString());

        gameStarted = Globals.roomGameStarted[currentRoomIndex];

        if (gameStarted)
        {
            try
            {
                currentActiveUser = int.Parse(Globals.rooms[currentRoomIndex]["activeUserIndex"].ToString());
                currentTimeout = int.Parse(Globals.rooms[currentRoomIndex]["countDownSec"].ToString());
                myCardsNumber = (int[])Globals.rooms[currentRoomIndex]["myCards"];
                if (sitPosition == currentActiveUser)
                {
                    actionButtonAreaIndex = 1;
                }
                else
                {
                    actionButtonAreaIndex = 2;
                }
                
            }
            catch (Exception)
            {

            }

        }

        //user info
        string[] seats = NewtonSoftHelper.JArrayToArray<string>(Globals.rooms[currentRoomIndex]["seats"]);
        Dictionary<string, object> gamers = NewtonSoftHelper.JObjectToObject<string, object>(Globals.rooms[currentRoomIndex]["gamers"]);

        usersInfo = new Dictionary<string, object>[seats.Length];
        for (int i = 0; i < seats.Length; i++)
        {
            usersInfo[i] = new Dictionary<string, object>();
            if (seats[i] != null)
            {
                // var a = NewtonSoftHelper.JObjectToObject<string, string>(gamers[seats[i]]);
                // var b = gamers[seats[i]];
                // Debug.Log(a);
                // Debug.Log(b);
                Dictionary<string, object> gamer = NewtonSoftHelper.JObjectToObject<string, object>(gamers[seats[i]]);
                usersInfo[i].Add("avatar", gamer["avatar"]);
                usersInfo[i].Add("name", gamer["name"]);
                usersInfo[i].Add("walletChips", gamer["coins"]);
                usersInfo[i].Add("chips", "1200000");
                usersInfo[i].Add("uid", gamer["uid"]);
                if (gamer.ContainsKey("cards"))
                {
                    usersInfo[i].Add("cards", NewtonSoftHelper.JArrayToArray<int>(gamer["cards"]));
                }
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
            sitPosition=GetSitPosition();
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
                    avatar.GetComponent<SpriteRenderer>().sprite = AvatarHelper.GetAvatar(usersInfo[i]["avatar"].ToString());
                    GameObject name = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;
                    name.GetComponent<TMP_Text>().text = usersInfo[i]["name"].ToString();
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
            Dictionary<string, object>[] rotatedUserInfo = ArrayHelper.RotateArray(usersInfo, sitPosition);
            initalizeUsers(usersArray);
            initalizeSitButtons(sitButtons);
            for (int i = 0; i < usersArray.Length; i++)
            {
                if (rotatedUserInfo[i].Count != 0)
                {

                    if (i == 0)
                    {
                        usersArray[i].transform.GetChild(1).gameObject.SetActive(true);
                        if (gameStarted)
                        {
                            usersArray[i].transform.GetChild(2).gameObject.SetActive(true);
                        }
                        else
                        {
                            usersArray[i].transform.GetChild(2).gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        usersArray[i].transform.GetChild(1).gameObject.SetActive(true);
                        if (gameStarted)
                        {
                            usersArray[i].transform.GetChild(2).gameObject.SetActive(true);
                            usersArray[i].transform.GetChild(3).gameObject.SetActive(true);
                        }
                        else
                        {
                            usersArray[i].transform.GetChild(2).gameObject.SetActive(false);
                            usersArray[i].transform.GetChild(3).gameObject.SetActive(false);
                        }

                    }
                    GameObject avatar = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
                    avatar.GetComponent<SpriteRenderer>().sprite = AvatarHelper.GetAvatar(rotatedUserInfo[i]["avatar"].ToString());
                    GameObject name = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;
                    name.GetComponent<TMP_Text>().text = rotatedUserInfo[i]["name"].ToString();
                    GameObject wallet_money = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject;
                    wallet_money.GetComponent<TMP_Text>().text = rotatedUserInfo[i]["walletChips"] + " ₮";
                    if (i == 0)
                    {

                    }
                    else
                    {
                        GameObject money = usersArray[i].transform.GetChild(3).gameObject.transform.GetChild(1).gameObject;
                        money.GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated(long.Parse(rotatedUserInfo[i]["chips"].ToString()),1);
                    }
                }
            }
        }

    }

    int GetSitPosition()
    {
        int position = -1;
        Dictionary<string, object> token = NewtonSoftHelper.JObjectToObject<string, object>(Globals.token);

        for(int i = 0; i < usersInfo.Length; i++)
        {
            if (usersInfo[i].Count > 0)
            {
                if (usersInfo[i]["uid"].ToString() == token["uid"].ToString())
                {
                    return i;
                }
                
            }
            
        }

        return position;
    }

    void initalizeUsers(GameObject[] usersArray)
    {
        for (int i = 0; i < usersArray.Length; i++)
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
        for (int i = 0; i < sitButtons.Length; i++)
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
        chipMaxBuyLimitObject.GetComponent<TMP_Text>().text = chipsMaxBuy.ToString();
        chipsSliderObject.minValue = chipsMinBuy;
        chipsSliderObject.maxValue = chipsMaxBuy;

    }

    void SetPublicCards()
    {
        GameObject[] publicCards = GameObjectHelper.GetChildren(publicCardArea);
        for (int i = 0; i < openedCards.Length; i++)
        {
            GameObject publicCard = publicCards[i].transform.GetChild(0).gameObject;
            publicCard.GetComponent<SpriteRenderer>().sprite = CardHelper.GetCard(openedCards[i]);
        }
    }

    public void sitToRoom(int index)
    {
        sitPositionTemp = index;

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
        Globals.socketIoConnection.SendRpc(data, OnTakeSeatResponse);
    }

    private void OnTakeSeatResponse(JToken jsonResponse)
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
            Globals.roomStates[currentRoomIndex]["ready"] = true;
            sitPosition = sitPositionTemp;
            actionButtonAreaIndex = 0;
            return;
        } while (false);
    }

    public void readyForGame()
    {
        Dictionary<string, object> token = (Dictionary<string, object>)Globals.token;
        string uid = token["uid"].ToString();
        int pin = Int32.Parse(token["pin"].ToString());
        var data = new
        {
            uid = uid,
            pin = pin,
            f = "ready",
            args = "0",
        };
        Globals.socketIoConnection.SendRpc(data, OnReadyResponse);
    }

    private void OnReadyResponse(JToken jsonResponse)
    {
        Globals.roomStates[currentRoomIndex]["ready"] = null;
        actionButtonAreaIndex = -1;
    }

    public void SetTimer()
    {
        if (sitPosition != -1)
        {
            GameObject[] usersArray = GameObjectHelper.GetChildren(usersParent);
            Dictionary<string, object>[] rotatedUserInfo = ArrayHelper.RotateArray(usersInfo, sitPosition);
            int currentActiveUserRotated = ArrayHelper.RotateNumber(currentActiveUser, sitPosition, usersInfo.Length);
            for (int i = 0; i < rotatedUserInfo.Length; i++)
            {
                GameObject progressbar = usersArray[i].transform.GetChild(1).transform.GetChild(2).transform.GetChild(0).gameObject;
                if (i == currentActiveUserRotated)
                {
                    progressbar.GetComponent<Image>().fillAmount = ((float)(20 - currentTimeout) / totalTime);
                }
                else
                {
                    progressbar.GetComponent<Image>().fillAmount = 0;
                }
            }
        }
    }

    public void SetActionButtonAreaIndexByGlobal()
    {
        var readyButtonStatus = Globals.roomStates[Globals.currentRoomIndex]["ready"];
        if (readyButtonStatus!=null)
        {
            actionButtonAreaIndex = 0;
        }
    }

    public void SetActionButtonArea()
    {
        GameObject[] actionButtonGroup = GameObjectHelper.GetChildren(ActionButtonsArea);
        for (int i = 0; i < actionButtonGroup.Length; i++)
        {
            if (i == actionButtonAreaIndex)
            {
                actionButtonGroup[i].SetActive(true);
            }
            else
            {
                actionButtonGroup[i].SetActive(false);
            }
        }
    }

    public void Fold()
    {
        Dictionary<string, object> token = (Dictionary<string, object>)Globals.token;
        string uid = token["uid"].ToString();
        int pin = Int32.Parse(token["pin"].ToString());
        var data = new
        {
            uid = uid,
            pin = pin,
            f = "fold",
            args = "0",
        };
        Globals.socketIoConnection.SendRpc(data, OnFoldResponse);
    }

    private void OnFoldResponse(JToken jsonResponse)
    {
        Debug.Log("Fold");
    }

    public void Check()
    {
        Dictionary<string, object> token = (Dictionary<string, object>)Globals.token;
        string uid = token["uid"].ToString();
        int pin = Int32.Parse(token["pin"].ToString());
        var data = new
        {
            uid = uid,
            pin = pin,
            f = "check",
            args = "0",
        };
        Globals.socketIoConnection.SendRpc(data, OnCheckResponse);
    }

    private void OnCheckResponse(JToken jsonResponse)
    {
        Debug.Log("Check");
    }

    public void Call()
    {
        Dictionary<string, object> token = (Dictionary<string, object>)Globals.token;
        string uid = token["uid"].ToString();
        int pin = Int32.Parse(token["pin"].ToString());
        var data = new
        {
            uid = uid,
            pin = pin,
            f = "call",
            args = "0",
        };
        Globals.socketIoConnection.SendRpc(data, OnCallResponse);
    }

    private void OnCallResponse(JToken jsonResponse)
    {
        Debug.Log("call");
    }

    public void Raise()
    {
        Dictionary<string, object> token = (Dictionary<string, object>)Globals.token;
        string uid = token["uid"].ToString();
        int pin = Int32.Parse(token["pin"].ToString());
        var amountOfRaiseString = Globals.roomStates[Globals.currentRoomIndex]["raise"].ToString();
        string pattern = @"\d+"; // Matches one or more digits

        Match match = Regex.Match(amountOfRaiseString, pattern);
        var data = new
        {
            uid = uid,
            pin = pin,
            f = "raise",
            args = match.Value.ToString(),
        };
        Globals.socketIoConnection.SendRpc(data, OnRaiseResponse);
    }

    private void OnRaiseResponse(JToken jsonResponse)
    {
        Debug.Log("Raise");
    }

    public void ToggleSitOutNextHandButton(GameObject button)
    {
        if (sitOutNextHandButtonEnabled)
        {
            sitOutNextHandButtonEnabled = false;
            button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/room/btn-grey-type1-inactive");
        }
        else
        {
            sitOutNextHandButtonEnabled = true;
            button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/room/btn-grey-type1-active");
        }
    }

    public void ToggleSitOutNextBigBlindButton(GameObject button)
    {
        if (sitOutNextBigBlindButtonEnabled)
        {
            sitOutNextBigBlindButtonEnabled = false;
            button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/room/btn-grey-type1-inactive");

        }
        else
        {
            sitOutNextBigBlindButtonEnabled= true;
            button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/room/btn-grey-type1-active");

        }
    }

    public void CallAnyButtonEnabled(GameObject button)
    {
        if (callAnyButtonEnabled)
        {
            callAnyButtonEnabled = false;
            button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/room/btn-grey-type1-inactive");

        }
        else
        {
            callAnyButtonEnabled = true;
            button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/room/btn-grey-type1-active");

        }
    }

    void SetRaiseBar()
    {
        GameObject[] raiseBarGrades = GameObjectHelper.GetChildren(raiseBarGradeParent);
        raiseToValue.GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated((long)raiseAmount, 1);
        int raiseBarStep = (maxRaise - minRaise) / raiseBarGrades.Length;
        for(int i = 0; i < raiseBarGrades.Length; i++)
        {
            if (i != raiseBarGrades.Length - 1)
            {
                raiseBarGrades[i].GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated((long)(minRaise + raiseBarStep * i),0);
            }
            else
            {
                raiseBarGrades[i].GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated((long)maxRaise,0);
            }
        }

        float raiseBarScale=(float)(raiseAmount-minRaise)/(maxRaise - minRaise);
        raiseBar.transform.localScale = (new Vector3(1, raiseBarScale, 1));
    }

    void CheckRaiseAmount()
    {
        if (raiseAmount > maxRaise)
        {
            raiseAmount = maxRaise;
        }
        else if( raiseAmount < minRaise)
        {
            raiseAmount = minRaise;
        }
    }

    public void ChangeRaiseAmount(int delta)
    {
        raiseAmount += delta;
    }


    public void AddChips()
    {
        int addChipValue = (int)chipsSliderObject.value;
        Debug.Log(addChipValue);
    }



}
