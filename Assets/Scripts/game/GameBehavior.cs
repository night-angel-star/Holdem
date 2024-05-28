using EasyUI.Toast;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UI.Tables;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    // public GameEngine engine;
    public Room room;

    public TMP_Text roomNameObject;
    public TMP_Text roomMoneyObject;
    public GameObject myCards;
    public GameObject usersParent;
    public GameObject chipMinBuyLimitObject;
    public GameObject chipMaxBuyLimitObject;
    public GameObject totalChipsValueObject;
    public GameObject chipAddButton;
    public Slider chipsSliderObject;
    public GameObject chipSliderParent;
    public GameObject depositeWarningText;
    public GameObject publicCardArea;
    public GameObject sitToSeatArea;
    public GameObject ActionButtonsArea;
    public GameObject raiseModal;
    public GameObject raiseButton;
    public GameObject raiseConfirmButton;

    public GameObject addChipsModal;

    public GameObject raiseBar;
    public GameObject raiseBarGradeParent;
    public GameObject raiseToValue;

    public GameObject raiseBarSlider;

    public TableLayout roomListTable;
    public GameObject joinButtonPrefab;

    public GameObject RoomTogglerParent;
    public GameObject RoomViewParent;
    public GameObject RoomAddButton;

    public GameObject sitOutNextHandButton;
    public GameObject sitOutNextBigBlindButton;
    public GameObject[] callAnyButton;

    public GameObject foldAnyButton;
    public GameObject checkFoldButton;

    public GameObject chatInput;
    public GameObject chatHistory;

    public GameObject waitTimeInfo;

    public GameObject handStrengthParent;

    public GameObject mentionDropdown;

    public GameObject pot;

    public GameObject gameOver;




    protected int raiseAmount = 0;
    protected int minRaiseAmount = 0;
    protected int maxRaiseAmount = 0;

    protected int actionButtonAreaIndex = -1;

    protected bool receiveFromGlobalResult = false;

    protected bool waiting = false;


    void Start()
    {
        InvokeRepeating("RepeatCall", 0.0f, 0.5f);
    }

    void RepeatCall()
    {
        try
        {
            //read from global
            try
            {
                receiveFromGlobalResult = UpdateRoomFromGlobal();
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }

            if (receiveFromGlobalResult)
            {
                try
                {
                    SetWaiting();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
                //set data from room data
                try
                {
                    DisableUnneccessarySeats();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

                try
                {
                    SetActionButtonArea();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }


                //draw ui
                if (room.gameStatus == 2 && room.GetUserSeat() != -1 && !waiting)
                {
                    try
                    {
                        GetMyCard();
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex);
                    }
                }
                try
                {
                    GetActionButtonsInteractable();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
                //AutoAction();
                try
                {
                    SetUserInfo();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

                try
                {
                    SetRoomName();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

                //try
                //{
                //    InitializeAddChipsModal();
                //}
                //catch (Exception ex)
                //{
                //    Debug.Log(ex);
                //}

                try
                {
                    SetPublicCards();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

                try
                {
                    SetTimer();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

                try
                {
                    SetActionButtonAreaIndexByGlobal();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

                try
                {
                    SetRaiseAmounts();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

                try
                {
                    SetRaiseBar();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

                try
                {
                    SetAutoButtons();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

                try
                {
                    CheckRaiseAmount();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

                try
                {
                    SetGamersActionStatus();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

                try
                {
                    SetRoomsToggler();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

                try
                {
                    SetRoomsView();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

                try
                {
                    SetChatHistory();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
                try
                {
                    SetWaitTimeInfo();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

                try
                {
                    SetResult();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

                try
                {
                    SetHandStrength();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }

                try
                {
                    SetPot();
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }


            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("Multiroom");
            LogHelper.AppLog(ex.ToString());
        }
    }

    protected bool UpdateRoomFromGlobal()
    {
        if (Globals.currentRoom != null)
        {
            if (Globals.gameRooms.ContainsKey(Globals.currentRoom))
            {
                room = Globals.gameRooms[Globals.currentRoom];
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

    }

    protected void DisableUnneccessarySeats()
    {
        GameObject[] usersArray = GameObjectHelper.GetChildren(usersParent);
        GameObject[] sitButtons = GameObjectHelper.GetChildren(sitToSeatArea);
        int tempSeatIndex = 0;
        switch (room.options.max_seats)
        {
            case 3:

                for (int i = 0; i < usersArray.Length; i++)
                {
                    if (i == 0 || i == 3 || i == 6)
                    {
                        usersArray[i].SetActive(true);
                        sitButtons[i].SetActive(true);
                        int index = tempSeatIndex;
                        sitButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
                        sitButtons[i].GetComponent<Button>().onClick.AddListener(() => sitToRoom(index));
                        tempSeatIndex++;
                    }
                    else
                    {
                        usersArray[i].SetActive(false);
                        sitButtons[i].SetActive(false);
                    }
                }
                break;
            case 6:
                for (int i = 0; i < usersArray.Length; i++)
                {
                    if (i == 1 || i == 4 || i == 8)
                    {
                        usersArray[i].SetActive(false);
                        sitButtons[i].SetActive(false);
                    }
                    else
                    {
                        usersArray[i].SetActive(true);
                        sitButtons[i].SetActive(true);
                        int index = tempSeatIndex;
                        sitButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
                        sitButtons[i].GetComponent<Button>().onClick.AddListener(() => sitToRoom(index));
                        tempSeatIndex++;
                    }
                }
                break;
            case 9:
                for (int i = 0; i < usersArray.Length; i++)
                {
                    usersArray[i].SetActive(true);
                    sitButtons[i].SetActive(true);
                    sitButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
                    int index = i;
                    sitButtons[i].GetComponent<Button>().onClick.AddListener(() => sitToRoom(index));
                }
                break;
            default:
                break;
        }
        if (room.GetUserSeat() != -1)
        {

            for (int i = 0; i < sitButtons.Length; i++)
            {
                sitButtons[i].SetActive(false);
            }
        }



    }

    protected void GetMyCard()
    {
        if (room.cards.Count > 0)
        {
            if (room.cards[room.GetUserSeat()].Length > 0)
            {
                myCards.SetActive(true);
                GameObject card1 = myCards.transform.GetChild(0).gameObject;
                GameObject card2 = myCards.transform.GetChild(1).gameObject;
                if (room.cards.ContainsKey(room.GetUserSeat()))
                {
                    card1.GetComponent<SpriteRenderer>().sprite = CardHelper.GetCard(room.cards[room.GetUserSeat()][0]);
                    card2.GetComponent<SpriteRenderer>().sprite = CardHelper.GetCard(room.cards[room.GetUserSeat()][1]);
                }

            }
        }

    }

    protected void GetActionButtonsInteractable()
    {
        GameObject callButton = ActionButtonsArea.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject;
        GameObject checkButton = ActionButtonsArea.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;
        GameObject readyButton = ActionButtonsArea.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;

        if (room.operations.check != null)
        {
            checkButton.GetComponent<Button>().interactable = true;
            callButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            checkButton.GetComponent<Button>().interactable = false;
            callButton.GetComponent<Button>().interactable = true;
        }

        if (room.operations.ready != null)
        {
            readyButton.GetComponent<Button>().interactable = true;
            readyButton.SetActive(true);
        }
        else
        {
            readyButton.GetComponent<Button>().interactable = false;
            readyButton.SetActive(false);
        }
    }
    protected void SetUserInfo()
    {
        if (room.GetUserSeat() == -1)
        {
            GameObject[] usersArray = GameObjectHelper.GetChildrenForRoomSize(usersParent, room.options.max_seats);
            GameObject[] sitButtons = GameObjectHelper.GetChildrenForRoomSize(sitToSeatArea, room.options.max_seats);

            initalizeUsers(usersArray);
            initalizeSitButtons(sitButtons);
            for (int i = 0; i < usersArray.Length; i++)
            {
                if (room.seats[i] != null)
                {
                    usersArray[i].transform.GetChild(1).gameObject.SetActive(true);
                    GameObject avatar = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
                    avatar.GetComponent<SpriteRenderer>().sprite = AvatarHelper.GetAvatar(room.gamers[room.seats[i]].avatar.ToString());
                    GameObject name = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;
                    name.GetComponent<TMP_Text>().text = room.gamers[room.seats[i]].name;
                    GameObject wallet_money = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject;
                    wallet_money.GetComponent<TMP_Text>().text = room.gamers[room.seats[i]].coins + " ₮";
                }
                else
                {
                    if (i == 0)
                    {
                        usersArray[i].transform.GetChild(1).gameObject.SetActive(false);
                        usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = null;
                        usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = "";
                        usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = "";
                        usersArray[i].transform.GetChild(2).gameObject.SetActive(false);
                        usersArray[i].transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = null;
                        usersArray[i].transform.GetChild(2).gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = null;
                        usersArray[i].transform.GetChild(3).gameObject.SetActive(false);
                        usersArray[i].transform.GetChild(4).gameObject.SetActive(false);
                        usersArray[i].transform.GetChild(4).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = "";
                    }
                    else
                    {
                        usersArray[i].transform.GetChild(1).gameObject.SetActive(false);
                        usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = null;
                        usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = "";
                        usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = "";
                        usersArray[i].transform.GetChild(2).gameObject.SetActive(false);
                        usersArray[i].transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = null;
                        usersArray[i].transform.GetChild(2).gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = null;
                        usersArray[i].transform.GetChild(3).gameObject.SetActive(false);
                        usersArray[i].transform.GetChild(3).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = "";
                        usersArray[i].transform.GetChild(4).gameObject.SetActive(false);
                        usersArray[i].transform.GetChild(5).gameObject.SetActive(false);
                        usersArray[i].transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = null;
                        usersArray[i].transform.GetChild(5).gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = null;
                    }
                    sitButtons[i].SetActive(true);
                }
            }
        }
        else
        {
            GameObject[] usersArray = GameObjectHelper.GetActiveChildren(usersParent);
            GameObject[] sitButtons = GameObjectHelper.GetActiveChildren(sitToSeatArea);
            string[] rotatedSeats = ArrayHelper.RotateArray(room.seats, room.GetUserSeat());
            initalizeUsers(usersArray);
            initalizeSitButtons(sitButtons);
            for (int i = 0; i < usersArray.Length; i++)
            {
                if (rotatedSeats[i] != null)
                {

                    if (i == 0)
                    {
                        usersArray[i].transform.GetChild(1).gameObject.SetActive(true);
                        if (room.gameStatus == 2 && room.GetUserSeat() != -1 && !waiting)
                        {
                            usersArray[i].transform.GetChild(2).gameObject.SetActive(true);
                            usersArray[i].transform.GetChild(4).gameObject.SetActive(true);
                        }
                        else
                        {
                            if (room.gameStatus == 3 && room.GetUserSeat() != -1 && !waiting)
                            {
                                usersArray[i].transform.GetChild(2).gameObject.SetActive(true);
                            }
                            else
                            {
                                usersArray[i].transform.GetChild(2).gameObject.SetActive(false);
                            }
                            usersArray[i].transform.GetChild(4).gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        usersArray[i].transform.GetChild(1).gameObject.SetActive(true);
                        if (room.gameStatus == 2 && room.GetUserSeat() != -1 && !waiting)
                        {
                            usersArray[i].transform.GetChild(2).gameObject.SetActive(true);
                            usersArray[i].transform.GetChild(3).gameObject.SetActive(true);
                        }
                        else
                        {
                            usersArray[i].transform.GetChild(2).gameObject.SetActive(false);
                            usersArray[i].transform.GetChild(3).gameObject.SetActive(false);
                        }
                        if (room.gameStatus == 3 && !waiting)
                        {
                            if (room.status[ArrayHelper.ReRotateNumber(i, room.GetUserSeat(), room.options.max_seats)] != "fold" && room.status[room.GetUserSeat()] != "fold")
                            {
                                if (i == 0)
                                {
                                    usersArray[i].transform.GetChild(2).gameObject.SetActive(true);
                                }
                                else
                                {
                                    usersArray[i].transform.GetChild(5).gameObject.SetActive(true);
                                }
                            }
                            else
                            {
                                if (i == 0)
                                {
                                    usersArray[i].transform.GetChild(2).gameObject.SetActive(true);
                                }
                                else
                                {
                                    usersArray[i].transform.GetChild(5).gameObject.SetActive(false);
                                }
                            }


                        }
                        else
                        {
                            if (i == 0)
                            {

                            }
                            else
                            {
                                usersArray[i].transform.GetChild(5).gameObject.SetActive(false);
                            }

                        }

                    }
                    if (room.gamers.ContainsKey(rotatedSeats[i]))
                    {
                        GameObject avatar = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
                        avatar.GetComponent<SpriteRenderer>().sprite = AvatarHelper.GetAvatar(room.gamers[rotatedSeats[i]].avatar.ToString());
                        GameObject name = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;
                        name.GetComponent<TMP_Text>().text = room.gamers[rotatedSeats[i]].name;
                        GameObject wallet_money = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject;
                        wallet_money.GetComponent<TMP_Text>().text = room.gamers[rotatedSeats[i]].coins + " ₮";
                    }





                    if (room.gameStatus == 2 && room.GetUserSeat() != -1 && !waiting)
                    {
                        if (i == 0)
                        {
                            GameObject money = usersArray[i].transform.GetChild(4).gameObject.transform.GetChild(1).gameObject;
                            try
                            {
                                int rotatedI = ArrayHelper.ReRotateNumber(i, room.GetUserSeat(), room.options.max_seats);
                                if (room.chips.ContainsKey(rotatedI))
                                {
                                    //money.GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated(long.Parse(room.chips[rotatedI].ToString()), 1);
                                    money.GetComponent<TMP_Text>().text = room.chips[rotatedI].ToString();
                                }


                            }
                            catch (Exception ex)
                            {
                                Toast.Show(ex.ToString());
                                Debug.Log(ex);
                            }
                        }
                        else
                        {
                            GameObject money = usersArray[i].transform.GetChild(3).gameObject.transform.GetChild(1).gameObject;
                            if (room.chips.ContainsKey(ArrayHelper.ReRotateNumber(i, room.GetUserSeat(), room.options.max_seats)))
                            {
                                //money.GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated(long.Parse(room.chips[ArrayHelper.ReRotateNumber(i, room.GetUserSeat(), room.options.max_seats)].ToString()), 1);
                                money.GetComponent<TMP_Text>().text = room.chips[ArrayHelper.ReRotateNumber(i, room.GetUserSeat(), room.options.max_seats)].ToString();
                            }

                        }
                    }
                    else if (room.gameStatus == 3)
                    {

                        if (room.status[ArrayHelper.ReRotateNumber(i, room.GetUserSeat(), room.options.max_seats)] != "fold" && room.status[room.GetUserSeat()] != "fold")
                        {
                            if (i == 0)
                            {

                            }
                            else
                            {
                                GameObject userShowCards1 = usersArray[i].transform.GetChild(5).gameObject.transform.GetChild(0).gameObject;
                                GameObject userShowCards2 = usersArray[i].transform.GetChild(5).gameObject.transform.GetChild(1).gameObject;
                                if (room.cards.ContainsKey(ArrayHelper.ReRotateNumber(i, room.GetUserSeat(), room.options.max_seats)))
                                {
                                    userShowCards1.GetComponent<SpriteRenderer>().sprite = CardHelper.GetCard(room.cards[ArrayHelper.ReRotateNumber(i, room.GetUserSeat(), room.options.max_seats)][0]);
                                    userShowCards2.GetComponent<SpriteRenderer>().sprite = CardHelper.GetCard(room.cards[ArrayHelper.ReRotateNumber(i, room.GetUserSeat(), room.options.max_seats)][1]);
                                }

                            }
                        }


                    }


                }
                else
                {
                    if (i == 0)
                    {
                        usersArray[i].transform.GetChild(1).gameObject.SetActive(false);
                        usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = null;
                        usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text="";
                        usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text="";
                        usersArray[i].transform.GetChild(2).gameObject.SetActive(false);
                        usersArray[i].transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = null;
                        usersArray[i].transform.GetChild(2).gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = null;
                        usersArray[i].transform.GetChild(3).gameObject.SetActive(false);
                        usersArray[i].transform.GetChild(4).gameObject.SetActive(false);
                        usersArray[i].transform.GetChild(4).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = "";
                    }
                    else
                    {
                        usersArray[i].transform.GetChild(1).gameObject.SetActive(false);
                        usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = null;
                        usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = "";
                        usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = "";
                        usersArray[i].transform.GetChild(2).gameObject.SetActive(false);
                        usersArray[i].transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = null;
                        usersArray[i].transform.GetChild(2).gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = null;
                        usersArray[i].transform.GetChild(3).gameObject.SetActive(false);
                        usersArray[i].transform.GetChild(3).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = "";
                        usersArray[i].transform.GetChild(4).gameObject.SetActive(false);
                        usersArray[i].transform.GetChild(5).gameObject.SetActive(false);
                        usersArray[i].transform.GetChild(5).gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = null;
                        usersArray[i].transform.GetChild(5).gameObject.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = null;
                    }
                }
            }
        }

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

    protected void SetRoomName()
    {
        roomNameObject.text = room.name;
        roomMoneyObject.text = room.options.min_buy.ToString();
    }

    public void CheckUserBalance()
    {
        if (room.options.min_buy > Globals.userProfile.deposite)
        {
            Toast.Show("You have not enough balance.", "danger");
        }
    }
    public void InitializeAddChipsModal()
    {
        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;
        var data = new
        {
            uid = uid,
            pin = pin,
            f = "profile",
            args = new
            {
                roomid = room.id
            },
        };
        Globals.socketIoConnection.SendRpc(data, OnGetMoney);

    }

    protected void UpdateAddChipsModal()
    {
        chipMinBuyLimitObject.GetComponent<TMP_Text>().text = room.options.min_buy.ToString();
        chipMaxBuyLimitObject.GetComponent<TMP_Text>().text = Globals.userProfile.deposite.ToString();
        //totalChipsValueObject.GetComponent<TMP_Text>().text = Globals.userProfile.deposite.ToString();
        if (room.options.min_buy <= Globals.userProfile.deposite)
        {
            chipsSliderObject.minValue = room.options.min_buy;
            chipsSliderObject.maxValue = Globals.userProfile.deposite;
            chipAddButton.GetComponent<Button>().interactable = true;
            chipSliderParent.SetActive(true);
            depositeWarningText.SetActive(false);
        }
        else
        {
            chipsSliderObject.gameObject.SetActive(false);
            chipAddButton.GetComponent<Button>().interactable = false;
            chipSliderParent.SetActive(false);
            depositeWarningText.SetActive(true);
        }

        if (room.options.min_buy > Globals.userProfile.deposite)
        {

        }
        else
        {

        }
    }

    protected void SetPublicCards()
    {
        GameObject[] publicCards = GameObjectHelper.GetChildren(publicCardArea);
        if (room.shared_cards != null)
        {
            for (int i = 0; i < room.shared_cards.Length; i++)
            {
                GameObject publicCard = publicCards[i].transform.GetChild(0).gameObject;
                publicCard.GetComponent<SpriteRenderer>().sprite = CardHelper.GetCard(room.shared_cards[i]);
            }
        }
    }

    public void sitToRoom(int index) //api
    {
        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;
        var data = new
        {
            uid = uid,
            pin = pin,
            f = "takeseat",
            roomid = Globals.currentRoom,
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
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Toast.Show(errorString, "danger");
                });

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
            else
            {
                if (ret["cmds"] != null)
                {
                    Dictionary<string, object> cmds = JsonResponse.ToDictionary(ret["cmds"]);
                    Globals.gameRooms[Globals.currentRoom].operations.ready = cmds["ready"];
                }
            }

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
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Toast.Show(errorString, "danger");
                });

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
            else
            {
                if (ret["cmds"] != null)
                {
                    Dictionary<string, object> cmds = JsonResponse.ToDictionary(ret["cmds"]);
                    Globals.gameRooms[Globals.currentRoom].operations.ready = cmds["ready"];
                    Debug.Log("ready button disabled");
                }
            }

            actionButtonAreaIndex = 0;
            return;
        } while (false);
    }

    public void SetTimer()
    {
        if (room.GetUserSeat() != -1)
        {
            if (room.activeSeat != -1)
            {
                GameObject[] usersArray = GameObjectHelper.GetActiveChildren(usersParent);
                string[] rotatedSeats = ArrayHelper.RotateArray(room.seats, room.GetUserSeat());
                int currentActiveUserRotated = ArrayHelper.RotateNumber(room.activeSeat, room.GetUserSeat(), rotatedSeats.Length);
                for (int i = 0; i < rotatedSeats.Length; i++)
                {
                    GameObject progressbar = usersArray[i].transform.GetChild(1).transform.GetChild(2).transform.GetChild(0).gameObject;
                    progressbar.SetActive(true);
                    if (i == currentActiveUserRotated)
                    {
                        progressbar.GetComponent<Image>().fillAmount = ((float)(20 - room.countdown) / room.totalCount);
                    }
                    else
                    {
                        progressbar.GetComponent<Image>().fillAmount = 0;
                    }
                }
            }
            else
            {
                GameObject[] usersArray = GameObjectHelper.GetActiveChildren(usersParent);
                string[] rotatedSeats = ArrayHelper.RotateArray(room.seats, room.GetUserSeat());
                for (int i = 0; i < rotatedSeats.Length; i++)
                {
                    GameObject progressbar = usersArray[i].transform.GetChild(1).transform.GetChild(2).transform.GetChild(0).gameObject;
                    progressbar.SetActive(false);
                }
            }

        }
        else
        {
            if (room.activeSeat != -1)
            {
                GameObject[] usersArray = GameObjectHelper.GetActiveChildren(usersParent);
                for (int i = 0; i < room.seats.Length; i++)
                {
                    GameObject progressbar = usersArray[i].transform.GetChild(1).transform.GetChild(2).transform.GetChild(0).gameObject;
                    progressbar.SetActive(true);
                    if (i == room.activeSeat)
                    {
                        progressbar.GetComponent<Image>().fillAmount = ((float)(20 - room.countdown) / room.totalCount);
                    }
                    else
                    {
                        progressbar.GetComponent<Image>().fillAmount = 0;
                    }
                }
            }
            else
            {
                GameObject[] usersArray = GameObjectHelper.GetActiveChildren(usersParent);
                for (int i = 0; i < room.seats.Length; i++)
                {
                    GameObject progressbar = usersArray[i].transform.GetChild(1).transform.GetChild(2).transform.GetChild(0).gameObject;
                    progressbar.SetActive(false);
                }
            }
        }
    }

    public void SetActionButtonAreaIndexByGlobal()
    {
        if (room.GetUserSeat() == -1)
        {
            actionButtonAreaIndex = -1;
        }
        else
        {
            switch (room.gameStatus)
            {
                //case 0:
                //    actionButtonAreaIndex = 0;
                //    break;
                //case 1:
                //    actionButtonAreaIndex = -1;
                //    break;
                case 2:
                    if (!waiting && room.operations.ready == null)
                    {
                        if (room.status != null)
                        {
                            if (room.status[room.GetUserSeat()] == "fold")
                            {
                                actionButtonAreaIndex = 2;
                            }
                            else
                            {
                                if (room.activeSeat == room.GetUserSeat())
                                {
                                    actionButtonAreaIndex = 1;
                                }
                                else
                                {
                                    actionButtonAreaIndex = 3;
                                }
                            }

                        }
                        else
                        {
                            if (room.activeSeat == room.GetUserSeat())
                            {
                                actionButtonAreaIndex = 1;
                            }
                            else
                            {
                                actionButtonAreaIndex = 3;
                            }
                        }

                    }
                    else
                    {
                        actionButtonAreaIndex = 0;
                    }
                    break;
                //case 3:
                //    actionButtonAreaIndex = 0;
                //    break;
                default:
                    actionButtonAreaIndex = 0;
                    break;
            }
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
        raiseAmount = 0;
        raiseBarSlider.GetComponent<Slider>().value = 0;
        raiseModal.SetActive(false);
        raiseButton.SetActive(true);
        raiseConfirmButton.SetActive(false);
    }



    private void OnRaiseResponse(JToken jsonResponse)
    {
        Debug.Log("Raise");
    }

    public void RaiseModalDismiss()
    {
        raiseModal.SetActive(false);
        raiseButton.SetActive(true);
        raiseConfirmButton.SetActive(false);
    }

    public void ToggleSitOutNextHandButton()
    {
        if (room.autoOperation.sitOutNextHandButton)
        {
            Globals.gameRooms[Globals.currentRoom].autoOperation.sitOutNextHandButton = false;
        }
        else
        {
            Globals.gameRooms[Globals.currentRoom].autoOperation.sitOutNextHandButton = true;
        }
    }

    public void ToggleSitOutNextBigBlindButton()
    {
        if (room.autoOperation.sitOutNextBigBlindButton)
        {
            Globals.gameRooms[Globals.currentRoom].autoOperation.sitOutNextBigBlindButton = false;
        }
        else
        {
            Globals.gameRooms[Globals.currentRoom].autoOperation.sitOutNextBigBlindButton = true;
        }
    }



    public void ToggleFoldAnyButton()
    {
        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;

        var data = new
        {
            uid = uid,
            pin = pin,
            f = "activestatus",
            args = (Globals.gameRooms[Globals.currentRoom].gamers[Globals.gameToken.uid].activeStatus != 1 ? 1 : 0),
        };
        Globals.socketIoConnection.SendRpc(data, ToggleFoldAnyResponse);

    }

    private void ToggleFoldAnyResponse(JToken jsonResponse)
    {
        //if (room.autoOperation.foldAnyButton)
        //{
        //    Globals.gameRooms[Globals.currentRoom].autoOperation.foldAnyButton = false;

        //}
        //else
        //{
        //    Globals.gameRooms[Globals.currentRoom].autoOperation.foldAnyButton = true;
        //    Globals.gameRooms[Globals.currentRoom].autoOperation.callAnyButton = false;
        //    Globals.gameRooms[Globals.currentRoom].autoOperation.checkFoldButton = false;
        //}
        Globals.gameRooms[Globals.currentRoom].gamers[Globals.gameToken.uid].activeStatus = (Globals.gameRooms[Globals.currentRoom].gamers[Globals.gameToken.uid].activeStatus != 1 ? 1 : 0);
    }

    public void ToggleCheckFoldButton()
    {
        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;

        var data = new
        {
            uid = uid,
            pin = pin,
            f = "activestatus",
            args = (Globals.gameRooms[Globals.currentRoom].gamers[Globals.gameToken.uid].activeStatus != 2 ? 2 : 0),
        };
        Globals.socketIoConnection.SendRpc(data, ToggleCheckFoldResponse);
    }

    private void ToggleCheckFoldResponse(JToken jsonResponse)
    {
        //if (room.autoOperation.checkFoldButton)
        //{
        //    Globals.gameRooms[Globals.currentRoom].autoOperation.checkFoldButton = false;
        //}
        //else
        //{
        //    Globals.gameRooms[Globals.currentRoom].autoOperation.checkFoldButton = true;
        //    Globals.gameRooms[Globals.currentRoom].autoOperation.callAnyButton = false;
        //    Globals.gameRooms[Globals.currentRoom].autoOperation.foldAnyButton = false;
        //}
        Globals.gameRooms[Globals.currentRoom].gamers[Globals.gameToken.uid].activeStatus = (Globals.gameRooms[Globals.currentRoom].gamers[Globals.gameToken.uid].activeStatus != 2 ? 2 : 0);
    }

    public void ToggleCallAnyButton()
    {
        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;

        var data = new
        {
            uid = uid,
            pin = pin,
            f = "activestatus",
            args = (Globals.gameRooms[Globals.currentRoom].gamers[Globals.gameToken.uid].activeStatus != 3 ? 3 : 0),
        };
        Globals.socketIoConnection.SendRpc(data, ToggleCallAnyResponse);

    }

    private void ToggleCallAnyResponse(JToken jsonResponse)
    {
        //if (room.autoOperation.callAnyButton)
        //{
        //    Globals.gameRooms[Globals.currentRoom].autoOperation.callAnyButton = false;
        //}
        //else
        //{
        //    Globals.gameRooms[Globals.currentRoom].autoOperation.callAnyButton = true;
        //    Globals.gameRooms[Globals.currentRoom].autoOperation.foldAnyButton = false;
        //    Globals.gameRooms[Globals.currentRoom].autoOperation.checkFoldButton = false;
        //}
        Globals.gameRooms[Globals.currentRoom].gamers[Globals.gameToken.uid].activeStatus = (Globals.gameRooms[Globals.currentRoom].gamers[Globals.gameToken.uid].activeStatus != 3 ? 3 : 0);
    }

    protected void SetAutoButtons()
    {
        if (room.autoOperation.sitOutNextHandButton)
        {
            sitOutNextHandButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/room/btn-grey-type1-active");
        }
        else
        {
            sitOutNextHandButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/room/btn-grey-type1-inactive");
        }

        if (room.autoOperation.sitOutNextBigBlindButton)
        {
            sitOutNextBigBlindButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/room/btn-grey-type1-active");

        }
        else
        {
            sitOutNextBigBlindButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/room/btn-grey-type1-inactive");

        }



        if (Globals.gameRooms[Globals.currentRoom].gamers[Globals.gameToken.uid].activeStatus == 1)
        {
            foldAnyButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/room/btn-grey-type1-active");

        }
        else
        {
            foldAnyButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/room/btn-grey-type1-inactive");

        }

        if (Globals.gameRooms[Globals.currentRoom].gamers[Globals.gameToken.uid].activeStatus == 2)
        {
            checkFoldButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/room/btn-grey-type1-active");
        }
        else
        {
            checkFoldButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/room/btn-grey-type1-inactive");
        }

        if (Globals.gameRooms[Globals.currentRoom].gamers[Globals.gameToken.uid].activeStatus == 3)
        {
            callAnyButton[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/room/btn-grey-type1-active");
            callAnyButton[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/room/btn-grey-type1-active");

        }
        else
        {
            callAnyButton[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/room/btn-grey-type1-inactive");
            callAnyButton[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/room/btn-grey-type1-inactive");

        }
    }



    void AutoAction()
    {
        //foreach(KeyValuePair<string, Room> kvp in Globals.gameRooms)
        //{

        //}
        if (room.gameStatus == 2 && room.activeSeat == room.GetUserSeat())
        {
            if (room.autoOperation.foldAnyButton)
            {
                Fold();
            }
            if (room.autoOperation.checkFoldButton)
            {
                if (room.operations.check != null)
                {
                    Check();
                }
                else
                {
                    Fold();
                }
            }
            if (room.autoOperation.callAnyButton)
            {
                if (room.operations.check != null)
                {
                    Check();
                }
                else if (room.operations.call != null)
                {
                    Call();
                }
            }
        }
    }

    protected void SetRaiseAmounts()
    {
        if (room.gameStatus == 2 && room.GetUserSeat() != -1 && !waiting)
        {
            int[] gamersCoinArray = new int[room.options.max_seats];
            for (int i = 0; i < gamersCoinArray.Length; i++)
            {
                if (room.seats[i] != null)
                {
                    if (room.chips.ContainsKey(i))
                    {
                        gamersCoinArray[i] = room.chips[i];
                    }
                    else
                    {
                        gamersCoinArray[i] = 0;
                    }
                }
                else
                {
                    gamersCoinArray[i] = 0;
                }
            }


            minRaiseAmount = 0;
            maxRaiseAmount = room.gamers[Globals.userProfile.uid].coins - (gamersCoinArray.Max() - room.chips[room.GetUserSeat()]);
            if (maxRaiseAmount < minRaiseAmount)
            {
                raiseButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                raiseButton.GetComponent<Button>().interactable = true;
                raiseBarSlider.GetComponent<Slider>().minValue = minRaiseAmount;
                raiseBarSlider.GetComponent<Slider>().maxValue = maxRaiseAmount;
            }


        }
    }
    protected void SetRaiseBar()
    {
        if (room.gameStatus == 2 && room.GetUserSeat() != -1 && !waiting)
        {
            try
            {
                if (maxRaiseAmount > minRaiseAmount)
                {
                    GameObject[] raiseBarGrades = GameObjectHelper.GetChildren(raiseBarGradeParent);
                    //raiseToValue.GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated((long)raiseAmount, 1);
                    raiseToValue.GetComponent<TMP_InputField>().text = raiseAmount.ToString();
                    int raiseBarStep = (maxRaiseAmount - minRaiseAmount) / raiseBarGrades.Length;
                    for (int i = 0; i < raiseBarGrades.Length; i++)
                    {
                        if (i != raiseBarGrades.Length - 1)
                        {
                            raiseBarGrades[i].GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated((long)(minRaiseAmount + raiseBarStep * i), 0);
                        }
                        else
                        {
                            raiseBarGrades[i].GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated((long)maxRaiseAmount, 0);
                        }
                    }

                    float raiseBarScale = (float)(raiseAmount - minRaiseAmount) / (maxRaiseAmount - minRaiseAmount);
                    raiseBar.transform.localScale = (new Vector3(1, raiseBarScale, 1));
                }

            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }

        }

    }

    protected void CheckRaiseAmount()
    {
        if (raiseAmount > maxRaiseAmount)
        {
            raiseAmount = maxRaiseAmount;
        }
        else if (raiseAmount < minRaiseAmount)
        {
            raiseAmount = minRaiseAmount;
        }
    }

    public void ChangeRaiseAmount(int delta)
    {
        if (delta > 0)
        {
            delta = (room.options.min_buy / 100);
        }
        else
        {
            delta = (-(room.options.min_buy / 100));
        }
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
                roomid = room.id,
                amount = addChipValue
            },
        };
        Globals.socketIoConnection.SendRpc(data, OnAddChipsResponse);
    }

    private void OnAddChipsResponse(JToken jsonResponse)
    {
        string errorString = "";
        Dictionary<string, object> res = JsonResponse.ToDictionary(jsonResponse);

        do
        {
            if (res == null)
            {
                errorString = "Invalid response";
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Toast.Show(errorString, "danger");
                });

                break;
            }
            int err = res["err"].ConvertTo<int>();
            if (err != 0)
            {
                if (!res.ContainsKey("ret"))
                {
                    errorString = "Invalid response";
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        roomListTable.ClearRows();
                    });
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        Toast.Show(errorString, "danger");
                    });
                    break;
                }
                errorString = res["ret"].ToString();
                UnityMainThreadDispatcher.Instance().Enqueue(() => { Toast.Show(errorString, "danger"); });
                break;
            }
            if (!res.ContainsKey("ret"))
            {
                errorString = "Invalid response";
                UnityMainThreadDispatcher.Instance().Enqueue(() => { Toast.Show(errorString, "danger"); });
                break;
            }
            Dictionary<string, object> ret = JsonResponse.ToDictionary(res["ret"]);
            if (ret == null)
            {
                errorString = "Invalid response";
                UnityMainThreadDispatcher.Instance().Enqueue(() => { Toast.Show(errorString, "danger"); });
                break;
            }
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Toast.Show("Chips added successfully");
                addChipsModal.SetActive(false);
            });


            return;
        } while (false);
    }
    void OnGetMoney(JToken jsonResponse)
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

            if (ret.ContainsKey("name"))
            {
                Globals.userProfile.name = ret["name"].ToString();
            }
            if (ret.ContainsKey("deposite"))
            {
                Globals.userProfile.deposite = int.Parse(ret["deposite"].ToString());
            }
            if (ret.ContainsKey("avatar"))
            {
                Globals.userProfile.avatar = ret["avatar"].ToString();
            }
            if (ret.ContainsKey("coins"))
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    totalChipsValueObject.GetComponent<TMP_Text>().text = ret["coins"].ToString();
                });

            }
            else
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    totalChipsValueObject.GetComponent<TMP_Text>().text = "0";
                });
            }
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UpdateAddChipsModal();
            });


            return;
        } while (false);
    }

    public void SetGamersActionStatus()
    {
        if (room.gameStatus == 2 && room.GetUserSeat() != -1 && !waiting)
        {
            if (room.status != null)
            {
                GameObject[] usersArray = GameObjectHelper.GetChildrenForRoomSize(usersParent, room.options.max_seats);
                string[] gamerActionStatus = room.status;
                string[] rotatedGamerActionStatus = ArrayHelper.RotateArray(gamerActionStatus, room.GetUserSeat());

                for (int i = 0; i < usersArray.Length; i++)
                {
                    if (rotatedGamerActionStatus[i] != null)
                    {
                        if (i == 0)
                        {
                            usersArray[i].transform.GetChild(3).gameObject.SetActive(true);
                            usersArray[i].transform.GetChild(3).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = rotatedGamerActionStatus[i];
                        }
                        else
                        {
                            usersArray[i].transform.GetChild(4).gameObject.SetActive(true);
                            usersArray[i].transform.GetChild(4).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = rotatedGamerActionStatus[i];
                        }

                    }
                    else
                    {
                        if (i == 0)
                        {
                            usersArray[i].transform.GetChild(3).gameObject.SetActive(false);
                        }
                        else
                        {
                            usersArray[i].transform.GetChild(4).gameObject.SetActive(false);
                        }

                    }
                }
            }
        }
        else if (room.gameStatus == 3)
        {
            if (room.GetUserSeat() != -1)
            {
                GameObject[] usersArray = GameObjectHelper.GetChildrenForRoomSize(usersParent, room.options.max_seats);
                string[] rotatedSeats = ArrayHelper.RotateArray(room.seats, room.GetUserSeat());
                for (int i = 0; i < rotatedSeats.Length; i++)
                {
                    if (rotatedSeats[i] != null)
                    {
                        if (i == 0)
                        {
                            usersArray[i].transform.GetChild(3).gameObject.SetActive(true);
                            string profit = "";
                            if (room.gamers[rotatedSeats[i]].profit > 0)
                            {
                                profit = "+" + room.gamers[rotatedSeats[i]].profit.ToString();
                            }
                            else
                            {
                                profit = room.gamers[rotatedSeats[i]].profit.ToString();
                            }
                            usersArray[i].transform.GetChild(3).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = profit;
                        }
                        else
                        {
                            usersArray[i].transform.GetChild(4).gameObject.SetActive(true);
                            string profit = "";
                            if (room.gamers[rotatedSeats[i]].profit > 0)
                            {
                                profit = "+" + room.gamers[rotatedSeats[i]].profit.ToString();
                            }
                            else
                            {
                                profit = room.gamers[rotatedSeats[i]].profit.ToString();
                            }
                            usersArray[i].transform.GetChild(4).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = profit;
                        }

                    }
                    else
                    {
                        usersArray[i].transform.GetChild(4).gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                GameObject[] usersArray = GameObjectHelper.GetChildrenForRoomSize(usersParent, room.options.max_seats);
                for (int i = 0; i < room.seats.Length; i++)
                {
                    if (room.seats[i] != null)
                    {
                        usersArray[i].transform.GetChild(4).gameObject.SetActive(true);
                        string profit = room.gamers[room.seats[i]].profit.ToString();
                        usersArray[i].transform.GetChild(4).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = profit;
                    }
                    else
                    {
                        usersArray[i].transform.GetChild(4).gameObject.SetActive(false);
                    }
                }
            }
        }
    }


    public void SetRoomsToggler()
    {

        GameObject[] rooms = GameObjectHelper.GetChildren(RoomTogglerParent);
        for (int i = 0; i < 3; i++)
        {
            rooms[i].SetActive(false);
        }
        int joinedRoomCount = 0;
        string[] joinedRoomIds = Globals.gameRooms.Keys.ToArray();
        for (int i = 0; i < joinedRoomIds.Length; i++)
        {
            rooms[joinedRoomCount].SetActive(true);
            rooms[joinedRoomCount].transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = Globals.gameRooms[joinedRoomIds[i]].name;
            int tempIndex = i;
            rooms[joinedRoomCount].transform.GetChild(0).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
            rooms[joinedRoomCount].transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(() => ChangeRoom(joinedRoomIds[tempIndex]));

            GameObject avatar = rooms[joinedRoomCount].transform.GetChild(2).gameObject;
            GameObject timer = rooms[joinedRoomCount].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
            if (joinedRoomIds[i] == Globals.currentRoom)
            {

                avatar.GetComponent<SpriteRenderer>().sprite = AvatarHelper.GetAvatar(Globals.userProfile.avatar.ToString());
                avatar.SetActive(true);
            }
            else
            {
                avatar.SetActive(false);

                if (Globals.gameRooms[joinedRoomIds[i]].gameStatus == 2)
                {
                    rooms[joinedRoomCount].transform.GetChild(1).gameObject.SetActive(true);
                    int currentActiveUserIndex = ArrayHelper.RotateNumber(Globals.gameRooms[joinedRoomIds[i]].activeSeat, Globals.gameRooms[joinedRoomIds[i]].GetUserSeat(), Globals.gameRooms[joinedRoomIds[i]].options.max_seats);

                    timer.GetComponent<Image>().fillAmount = (float)(currentActiveUserIndex + 1) / Globals.gameRooms[joinedRoomIds[i]].options.max_seats;
                    if (Globals.gameRooms[joinedRoomIds[i]].activeSeat == Globals.gameRooms[joinedRoomIds[i]].GetUserSeat())
                    {
                        Toast.Show("It's your turn on " + Globals.gameRooms[joinedRoomIds[i]].name + ".");
                    }
                }
                else
                {
                    rooms[joinedRoomCount].transform.GetChild(1).gameObject.SetActive(false);
                }
            }

            joinedRoomCount++;
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

    public void ChangeRoom(string roomId)
    {

        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;
        var data = new
        {
            uid = uid,
            pin = pin,
            f = "changeroom",
            args = new
            {
                roomid = roomId,
                seat = Globals.gameRooms[roomId].GetUserSeat()
            },
        };
        Globals.socketIoConnection.SendRpc(data, OnChangeRoomResponse);
        Globals.currentRoom = roomId;
    }

    private void OnChangeRoomResponse(JToken jsonResponse)
    {

    }

    public void SetRoomsView()
    {
        GameObject[] rooms = GameObjectHelper.GetChildren(RoomViewParent);
        for (int i = 0; i < 3; i++)
        {
            rooms[i].SetActive(false);
        }
        int joinedRoomCount = 0;
        string[] joinedRoomIds = Globals.gameRooms.Keys.ToArray();
        for (int i = 0; i < joinedRoomIds.Length; i++)
        {
            rooms[joinedRoomCount].SetActive(true);
            rooms[joinedRoomCount].transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = Globals.gameRooms[joinedRoomIds[i]].name;

            joinedRoomCount++;
        }
    }


    //For Room Table
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
        catch (Exception e)
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
            if (!Globals.gameRooms.ContainsKey(dictionary["id"].ToString()))
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
        rowElements[1] = convertToTitleCase(data["name"].ToString());
        rowElements[2] = data["seats_taken"].ToString() + "/" + data["seats_count"].ToString();
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
        if (Globals.gameRooms.Count < 3)
        {
            string uid = Globals.gameToken.uid;
            int pin = Globals.gameToken.pin;
            var data = new
            {
                uid = uid,
                pin = pin,
                f = "enterroom",
                args = new
                {
                    roomid = roomIndex.ToString()
                }
            };
            Globals.socketIoConnection.SendRpc(data, OnJoinResponse);
        }
        else
        {
            Toast.Show("You can't join more than 3 rooms", "danger");
        }
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
        if (roomListTable != null)
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
    }

    //End Table

    public void SendChat(string text)
    {
        if (text.EndsWith("\n"))
        {
            chatInput.GetComponent<TMP_InputField>().text = "";
            string uid = Globals.gameToken.uid;
            int pin = Globals.gameToken.pin;
            var data = new
            {
                uid = uid,
                pin = pin,
                f = "chat",
                args = text
            };
            Globals.socketIoConnection.SendRpc(data, OnSendChatResponse);
        }
        else if (text.EndsWith("@"))
        {
            List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();
            List<string> gamersUid = new List<string>(room.gamers.Keys);
            foreach (string gamerUid in gamersUid)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
                option.text = room.gamers[gamerUid].name;
                optionDatas.Add(option);
            }

            mentionDropdown.GetComponent<TMP_Dropdown>().options = optionDatas;
            mentionDropdown.GetComponent<TMP_Dropdown>().Show();
        }
    }

    public void AddUserNameFromDropdown()
    {
        int index = mentionDropdown.GetComponent<TMP_Dropdown>().value;
        if (index != -1)
        {
            List<string> gamersUid = new List<string>(room.gamers.Keys);
            chatInput.GetComponent<TMP_InputField>().text += room.gamers[gamersUid[index]].name;
            chatInput.GetComponent<TMP_InputField>().Select();
            chatInput.GetComponent<TMP_InputField>().caretPosition = chatInput.GetComponent<TMP_InputField>().text.Length;
        }

    }

    private void OnSendChatResponse(JToken jsonResponse)
    {
    }

    protected void SetChatHistory()
    {
        if (Globals.chatHistory.ContainsKey(room.id))
        {
            chatHistory.GetComponent<TMP_Text>().text = Globals.chatHistory[room.id];
        }
    }

    protected void SetWaitTimeInfo()
    {
        if (room.activeSeat == -1)
        {
            waitTimeInfo.SetActive(true);
            GameObject waitTimeValue = waitTimeInfo.transform.GetChild(1).gameObject;
            string waitTimeStr = TimeSpan.FromSeconds(room.countdown).ToString(@"hh\:mm\:ss");
            waitTimeValue.GetComponent<TMP_Text>().text = waitTimeStr;
        }
        else
        {
            waitTimeInfo.SetActive(false);
        }
    }

    void SetWaiting()
    {
        if (room.GetUserSeat() != -1)
        {
            if (room.gameStatus == 2)
            {
                if (room.operations.ready != null)
                {
                    waiting = true;
                }
            }
            else
            {
                waiting = false;
            }
        }
    }


    protected void SetHandStrength()
    {
        if (room.gameStatus == 2 && room.GetUserSeat() != -1 && !waiting)
        {
            GameObject[] handStrengthObjects = GameObjectHelper.GetChildren(handStrengthParent);

            List<string> allCards = new List<string>
                {
                    CardHelper.GetCardStr(room.cards[room.GetUserSeat()][0]),
                    CardHelper.GetCardStr(room.cards[room.GetUserSeat()][1])
                };
            for (int i = 0; i < 5; i++)
            {
                if (room.shared_cards[i] != 0)
                {
                    allCards.Add(CardHelper.GetCardStr(room.shared_cards[i]));
                }
            }

            int handStrength = HandStrengthCalculator.CalculateHandStrength(allCards) - 1;
            for (int i = 0; i < handStrengthObjects.Length; i++)
            {
                if (i < handStrength)
                {
                    handStrengthObjects[i].SetActive(true);
                }
                else
                {
                    handStrengthObjects[i].SetActive(false);
                }
            }

        }
    }

    protected void SetPot()
    {
        if (room.gameStatus == 2)
        {
            int potValue = 0;
            List<int> chipIndexs = new List<int>(room.chips.Keys);
            for (int i = 0; i < chipIndexs.Count; i++)
            {
                potValue += room.chips[chipIndexs[i]];
            }
            pot.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = potValue.ToString();
            pot.SetActive(true);
        }
        else
        {
            pot.SetActive(false);
        }
    }

    protected void SetResult()
    {
        if (room.gameStatus == 3)
        {
            gameOver.SetActive(true);
            string resultText = "";
            for (int i = 0; i < room.seats.Length; i++)
            {
                if (room.seats != null)
                {
                    string profit = "";
                    if (room.gamers[room.seats[i]].profit > 0)
                    {
                        profit = "+" + room.gamers[room.seats[i]].profit.ToString();
                    }
                    else
                    {
                        profit = room.gamers[room.seats[i]].profit.ToString();
                    }
                    resultText += $"{room.gamers[room.seats[i]].name} ({profit})";
                }
            }
            gameOver.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = resultText;
        }
        else
        {
            gameOver.SetActive(false);
        }
    }

    public void OnRaiseInputChange(string value)
    {
        try
        {
            raiseAmount = int.Parse(value);
            raiseBarSlider.GetComponent<Slider>().value = raiseAmount;
        }
        catch (Exception)
        {
            
        }
    }

}
