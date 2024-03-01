using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UI.Tables;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;


public class GameBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    // public GameEngine engine;
    public Room room;

    public TMP_Text roomNameObject;
    public GameObject myCards;
    public GameObject usersParent;
    public GameObject chipMinBuyLimitObject;
    public GameObject chipMaxBuyLimitObject;
    public Slider chipsSliderObject;
    public GameObject publicCardArea;
    public GameObject sitToSeatArea;
    public GameObject ActionButtonsArea;
    public GameObject raiseModal;
    public GameObject raiseButton;
    public GameObject raiseConfirmButton;

    public GameObject raiseBar;
    public GameObject raiseBarGradeParent;
    public GameObject raiseToValue;

    public GameObject raiseBarSlider;

    public TableLayout roomListTable;
    public GameObject joinButtonPrefab;

    public GameObject RoomTogglerParent;
    public GameObject RoomViewParent;
    public GameObject RoomAddButton;

    public string roomName;
    public int chipsMinBuy;
    public int chipsMaxBuy;


    public int minRaise = 0;
    public int maxRaise = 70000;
    public int raiseAmount = 0;

    public int actionButtonAreaIndex = -1;

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
        UpdateRoomFromGlobal();
        //set data from room data
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
        SetRaiseAmounts();
        SetRaiseBar();
        CheckRaiseAmount();
        SetGamersActionStatus();
        SetRoomsToggler();
        SetRoomsView();
        SetSharedCards();
    }

    void UpdateRoomFromGlobal()
    {
        room = Globals.gameRooms[Globals.currentRoom];
    }

    void SetRoomData()
    {
        roomName = room.name;
        chipsMinBuy = room.options.min_buy;
        chipsMaxBuy = Globals.userProfile.deposite;
        
        

        gameStarted = Globals.roomGameStarted[currentRoomIndex];

        if (gameStarted)
        {
            try
            {
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
                Dictionary<string, object> gamer = NewtonSoftHelper.JObjectToObject<string, object>(gamers[seats[i]]);
                usersInfo[i].Add("avatar", gamer["avatar"]);
                usersInfo[i].Add("name", gamer["name"]);
                usersInfo[i].Add("walletChips", gamer["coins"]);
                if (gamer.ContainsKey("chips"))
                {
                    usersInfo[i].Add("chips", gamer["chips"]);
                }
                else
                {
                    usersInfo[i].Add("chips", "0");
                }
                
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

        for(int i = 0; i < usersInfo.Length; i++)
        {
            if (usersInfo[i].Count > 0)
            {
                if (usersInfo[i]["uid"].ToString() == Globals.gameToken.uid)
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

        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;
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
        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;
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
        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;
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
        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;
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
        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;
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
        raiseModal.SetActive(true);
        raiseButton.SetActive(false);
        raiseConfirmButton.SetActive(true);
    }

    public void RaiseConfirm()
    {
        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;
        
        var data = new
        {
            uid = uid,
            pin = pin,
            f = "raise",
            args = raiseAmount.ToString(),
        };
        Globals.socketIoConnection.SendRpc(data, OnRaiseResponse);
        raiseModal.SetActive(false);
        raiseButton.SetActive(true);
        raiseConfirmButton.SetActive(false);
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

    void SetRaiseAmounts()
    {
        if (gameStarted)
        {
            Dictionary<string, object> options = NewtonSoftHelper.JObjectToObject<string, object>(Globals.rooms[currentRoomIndex]["options"]);
            minRaise = int.Parse(options["small_blind"].ToString());
            maxRaise = int.Parse(usersInfo[sitPosition]["walletChips"].ToString());
            raiseBarSlider.GetComponent<Slider>().minValue = minRaise;
            raiseBarSlider.GetComponent<Slider>().maxValue = maxRaise;
        }
    }

    void SetRaiseBar()
    {
        if(gameStarted)
        {
            GameObject[] raiseBarGrades = GameObjectHelper.GetChildren(raiseBarGradeParent);
            raiseToValue.GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated((long)raiseAmount, 1);
            int raiseBarStep = (maxRaise - minRaise) / raiseBarGrades.Length;
            for (int i = 0; i < raiseBarGrades.Length; i++)
            {
                if (i != raiseBarGrades.Length - 1)
                {
                    raiseBarGrades[i].GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated((long)(minRaise + raiseBarStep * i), 0);
                }
                else
                {
                    raiseBarGrades[i].GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated((long)maxRaise, 0);
                }
            }

            float raiseBarScale = (float)(raiseAmount - minRaise) / (maxRaise - minRaise);
            raiseBar.transform.localScale = (new Vector3(1, raiseBarScale, 1));
        }
        
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
        raiseBarSlider.GetComponent<Slider>().value = raiseAmount;
    }

    public void OnRaiseBarSliderChanged()
    {
        raiseAmount = (int)raiseBarSlider.GetComponent<Slider>().value;
    }


    public void AddChips()
    {
        int addChipValue = (int)chipsSliderObject.value;
        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;
        var data = new
        {
            uid = uid,
            pin = pin,
            f = "buychip",
            args = new
            {
                roomid = Globals.currentRoomId,
                amount = addChipValue
            },
        };
        Globals.socketIoConnection.SendRpc(data, OnAddChipsResponse);
    }

    private void OnAddChipsResponse(JToken jsonResponse)
    {

    }

    public void SetGamersActionStatus()
    {
        if (gameStarted)
        {
            GameObject[] usersArray = GameObjectHelper.GetChildren(usersParent);
            string[] gamerActionStatus = Globals.gamersActionStates[Globals.currentRoomIndex];
            string[] rotatedGamerActionStatus = ArrayHelper.RotateArray(gamerActionStatus, sitPosition);

            for (int i = 1; i < usersArray.Length; i++)
            {
                if (rotatedGamerActionStatus[i] != null)
                {
                    usersArray[i].transform.GetChild(4).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = rotatedGamerActionStatus[i];
                }
            }
        }
    }


    public void getRoomsListData()
    {
        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;
        var data = new
        {
            uid = uid,
            pin = pin,
            f = "rooms",
            args = "0"
        };
        Globals.socketIoConnection.SendRpc(data, OnRoomsResponse);
    }

    private void OnRoomsResponse(JToken jsonResponse)
    {
        try
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                roomListTable.ClearRows();
            });
            
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
        
        Dictionary<string, object> res = JsonResponse.ToDictionary(jsonResponse);
        IEnumerable temp = JsonConvert.DeserializeObject(res["ret"].ToString()) as IEnumerable;
        JObject[] valueArray = temp.Cast<JObject>().ToArray();
        for (int i = 0; i < valueArray.Length; i++)
        {
            int index = i;
            Dictionary<string, object> dictionary = valueArray[i].ToObject<Dictionary<string, object>>();
            if (!Globals.roomIdArray.Contains(int.Parse(dictionary["id"].ToString())))
            {
                string[] rowContents = parseRow(dictionary);
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    AddRowToTable(rowContents, Int32.Parse(dictionary["id"].ToString()));
                });
            }
            
        }

    }

    private string[] parseRow(Dictionary<string, object> data)
    {
        string[] rowElements = { };
        Array.Resize(ref rowElements, 5);
        rowElements[0] = convertToTitleCase(data["type"].ToString());
        rowElements[1] = convertToTitleCase(data["room_name"].ToString());
        rowElements[2] = data["seats_taken"].ToString() + "/" + data["seat"].ToString();
        rowElements[3] = data["small_blind"].ToString() + "/" + data["big_blind"].ToString();
        rowElements[4] = data["min_buy"].ToString();
        return rowElements;
    }

    private string convertToTitleCase(string str)
    {
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        string output = textInfo.ToTitleCase(str);
        return output;
    }

    private void JoinHandler(int roomIndex)
    {
        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;
        var data = new
        {
            uid = uid,
            pin = pin,
            f = "entergame",
            args = new
            {
                is_new = "old",
                id = roomIndex
            }
        };
        Globals.socketIoConnection.SendRpc(data, OnJoinResponse);
    }

    private void OnJoinResponse(JToken jsonResponse)
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

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                SceneManager.LoadScene("Room");
            });
            return;
        } while (false);
    }

    public void AddRowToTable(string[] rowElements, int index)
    {
        TableRow newRow = roomListTable.AddRow();
        newRow.preferredHeight = 23;
        for (int i = 0; i < rowElements.Length; i++)
        {
            TableScript.AddStringToCell(newRow.Cells[i], convertToTitleCase(rowElements[i].ToString()));
        }
        GameObject cellObject = new GameObject("GameObject", typeof(RectTransform));
        cellObject.transform.SetParent(newRow.Cells[5].transform);
        cellObject.transform.localScale = Vector3.one;

        GameObject instantiatedButton = Instantiate(joinButtonPrefab, cellObject.transform);
        instantiatedButton.transform.localScale = Vector3.one;

        Button button = instantiatedButton.GetComponent<Button>();
        button.onClick.AddListener(() => JoinHandler(index));
    }

    public void SetRoomsToggler()
    {
        
        GameObject[] rooms = GameObjectHelper.GetChildren(RoomTogglerParent);
        for(int i = 0; i < 3; i++)
        {
            rooms[i].SetActive(false);
        }
        int joinedRoomCount = 0;
        for(int i = 0; i < Globals.rooms.Length; i++)
        {
            if (Globals.rooms[i] != null)
            {
                rooms[joinedRoomCount].SetActive(true);
                rooms[joinedRoomCount].transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = Globals.rooms[i]["name"].ToString();
                Dictionary<string, object> profile = (Dictionary<string, object>)Globals.profile;
                
                int roomId = int.Parse(Globals.rooms[i]["id"].ToString());
                int roomIndex = i;
                rooms[joinedRoomCount].transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(() => ChangeRoom(roomId, roomIndex));

                GameObject avatar = rooms[joinedRoomCount].transform.GetChild(2).gameObject;
                if (roomId == Globals.currentRoomId)
                {

                    avatar.GetComponent<SpriteRenderer>().sprite = AvatarHelper.GetAvatar(profile["avatar"].ToString());
                    avatar.SetActive(true);
                }
                else
                {
                    avatar.SetActive(false);
                }

                joinedRoomCount++;
                
            }
        }
        if (joinedRoomCount > 2)
        {
            RoomTogglerParent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 245);
            RoomAddButton.SetActive(false);
        }
        else
        {
            RoomTogglerParent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 150);
            RoomAddButton.SetActive(true);
        }
    }

    public void SetRoomsView()
    {
        GameObject[] rooms = GameObjectHelper.GetChildren(RoomViewParent);
        for (int i = 0; i < 3; i++)
        {
            rooms[i].SetActive(false);
        }
        int joinedRoomCount = 0;
        for (int i = 0; i < Globals.rooms.Length; i++)
        {
            if (Globals.rooms[i] != null)
            {
                rooms[joinedRoomCount].SetActive(true);
                rooms[joinedRoomCount].transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = Globals.rooms[i]["name"].ToString();

                joinedRoomCount++;
            }
        }
    }

    public void ChangeRoom(int roomId, int roomIndex)
    {
        Globals.currentRoomId = roomId;
        Globals.currentRoomIndex = roomIndex;
    }

    void SetSharedCards()
    {
        if (gameStarted)
        {
            openedCards = Globals.shareCards[Globals.currentRoomIndex];
        }
    }
}
