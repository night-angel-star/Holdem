﻿using EasyUI.Toast;
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


public class TournamentBehaviorOld : MonoBehaviour
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

    public GameObject blindsInfo;
    public GameObject newBlindsInfo;
    public GameObject blindsUpTimeInfo;




    public int raiseAmount = 0;

    public int minRaiseAmount = 0;
    public int maxRaiseAmount = 0;

    public int actionButtonAreaIndex = -1;

    bool receiveFromGlobalResult = false;


    void Start()
    {
        InvokeRepeating("RepeatCall", 0.0f, 0.5f);
    }

    void RepeatCall()
    {
        try
        {
            //read from global
            receiveFromGlobalResult = UpdateRoomFromGlobal();
            if (receiveFromGlobalResult)
            {
                //set data from room data
                DisableUnneccessarySeats();
                SetActionButtonArea();
                //draw ui
                if (room.gameStatus == 2)
                {
                    GetMyCard();
                    GetActionButtonsInteractable();
                }
                //AutoAction();
                SetUserInfo();
                SetRoomName();
                InitializeAddChipsModal();
                SetPublicCards();
                SetTimer();
                SetActionButtonAreaIndexByGlobal();
                SetRaiseAmounts();
                SetRaiseBar();
                SetAutoButtons();
                CheckRaiseAmount();
                SetGamersActionStatus();
                SetRoomsToggler();
                SetRoomsView();
                SetTournamentInfo();
            }
        }
        catch (Exception ex)
        {
            LogHelper.AppLog("Tournament");
            LogHelper.AppLog(ex.ToString());
        }
    }

    bool UpdateRoomFromGlobal()
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

    void DisableUnneccessarySeats()
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

    void GetMyCard()
    {
        if (room.cards.Count > 0)
        {
            if (room.cards[room.GetUserSeat()].Length > 0)
            {
                myCards.SetActive(true);
                GameObject card1 = myCards.transform.GetChild(0).gameObject;
                GameObject card2 = myCards.transform.GetChild(1).gameObject;
                card1.GetComponent<SpriteRenderer>().sprite = CardHelper.GetCard(room.cards[room.GetUserSeat()][0]);
                card2.GetComponent<SpriteRenderer>().sprite = CardHelper.GetCard(room.cards[room.GetUserSeat()][1]);
            }
        }

    }

    void GetActionButtonsInteractable()
    {
        GameObject callButton = ActionButtonsArea.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject;
        GameObject checkButton = ActionButtonsArea.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;

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
    }

    void SetUserInfo()
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
                    avatar.GetComponent<SpriteRenderer>().sprite = AvatarHelper.GetAvatar(room.gamers[room.seats[i]].avatar.ToString());
                    GameObject name = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;
                    name.GetComponent<TMP_Text>().text = room.gamers[room.seats[i]].name;
                    GameObject wallet_money = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject;
                    wallet_money.GetComponent<TMP_Text>().text = room.gamers[room.seats[i]].coins + " ₮";
                }
                else
                {
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
                        if (room.gameStatus == 2||room.gameStatus==3)
                        {
                            usersArray[i].transform.GetChild(2).gameObject.SetActive(true);
                            usersArray[i].transform.GetChild(4).gameObject.SetActive(true);
                        }
                        else
                        {
                            usersArray[i].transform.GetChild(2).gameObject.SetActive(false);
                            usersArray[i].transform.GetChild(4).gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        usersArray[i].transform.GetChild(1).gameObject.SetActive(true);
                        if (room.gameStatus == 2)
                        {
                            usersArray[i].transform.GetChild(2).gameObject.SetActive(true);
                            usersArray[i].transform.GetChild(3).gameObject.SetActive(true);
                        }
                        else
                        {
                            usersArray[i].transform.GetChild(2).gameObject.SetActive(false);
                            usersArray[i].transform.GetChild(3).gameObject.SetActive(false);
                        }
                        if (room.gameStatus == 3)
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
                    GameObject avatar = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
                    avatar.GetComponent<SpriteRenderer>().sprite = AvatarHelper.GetAvatar(room.gamers[rotatedSeats[i]].avatar.ToString());
                    GameObject name = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;
                    name.GetComponent<TMP_Text>().text = room.gamers[rotatedSeats[i]].name;
                    GameObject wallet_money = usersArray[i].transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject;
                    wallet_money.GetComponent<TMP_Text>().text = room.gamers[rotatedSeats[i]].coins + " ₮";

                    if (room.gameStatus == 2)
                    {
                        if (i == 0)
                        {
                            GameObject money = usersArray[i].transform.GetChild(4).gameObject.transform.GetChild(1).gameObject;
                            try
                            {
                                int rotatedI = ArrayHelper.ReRotateNumber(i, room.GetUserSeat(), room.options.max_seats);
                                money.GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated(long.Parse(room.chips[rotatedI].ToString()), 1);

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
                            try
                            {
                                int rotatedI = ArrayHelper.ReRotateNumber(i, room.GetUserSeat(), room.options.max_seats);
                                money.GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated(long.Parse(room.chips[rotatedI].ToString()), 1);

                            }
                            catch (Exception ex)
                            {
                                Toast.Show(ex.ToString());
                                Debug.Log(ex);
                            }
                        }
                    }
                    else if (room.gameStatus == 3)
                    {
                        if (room.status[ArrayHelper.ReRotateNumber(i, room.GetUserSeat(), room.options.max_seats)] != "fold" && room.status[room.GetUserSeat()]!="fold")
                        {
                            if (i == 0)
                            {

                            }
                            else
                            {
                                GameObject userShowCards1 = usersArray[i].transform.GetChild(5).gameObject.transform.GetChild(0).gameObject;
                                GameObject userShowCards2 = usersArray[i].transform.GetChild(5).gameObject.transform.GetChild(1).gameObject;
                                userShowCards1.GetComponent<SpriteRenderer>().sprite = CardHelper.GetCard(room.cards[ArrayHelper.ReRotateNumber(i, room.GetUserSeat(), room.options.max_seats)][0]);
                                userShowCards2.GetComponent<SpriteRenderer>().sprite = CardHelper.GetCard(room.cards[ArrayHelper.ReRotateNumber(i, room.GetUserSeat(), room.options.max_seats)][1]);
                            }
                        }

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

    void SetRoomName()
    {
        roomNameObject.text = room.name;
    }

    void InitializeAddChipsModal()
    {
        chipMinBuyLimitObject.GetComponent<TMP_Text>().text = room.options.min_buy.ToString();
        chipMaxBuyLimitObject.GetComponent<TMP_Text>().text = Globals.userProfile.deposite.ToString();
        chipsSliderObject.minValue = room.options.min_buy;
        chipsSliderObject.maxValue = Globals.userProfile.deposite;

    }

    void SetPublicCards()
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
        //string uid = Globals.gameToken.uid;
        //int pin = Globals.gameToken.pin;
        //var data = new
        //{
        //    uid = uid,
        //    pin = pin,
        //    f = "takeseat",
        //    args = index.ToString(),
        //};
        //Globals.socketIoConnection.SendRpc(data, OnTakeSeatResponse);
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
    }

    public void SetTimer()
    {
        if (room.GetUserSeat() != -1)
        {
            GameObject[] usersArray = GameObjectHelper.GetActiveChildren(usersParent);
            string[] rotatedSeats = ArrayHelper.RotateArray(room.seats, room.GetUserSeat());
            int currentActiveUserRotated = ArrayHelper.RotateNumber(room.activeSeat, room.GetUserSeat(), rotatedSeats.Length);
            for (int i = 0; i < rotatedSeats.Length; i++)
            {
                GameObject progressbar = usersArray[i].transform.GetChild(1).transform.GetChild(2).transform.GetChild(0).gameObject;
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
                case 0:
                    actionButtonAreaIndex = 0;
                    break;
                case 1:
                    actionButtonAreaIndex = -1;
                    break;
                case 2:
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
                    break;
                case 3:
                    actionButtonAreaIndex = 0;
                    break;
                default:
                    actionButtonAreaIndex = -1;
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
                //switch (i)
                //{
                //    case 0:
                //        GameObject readyButton = actionButtonGroup[i].transform.GetChild(0).gameObject;
                //        if (room.operations.ready)
                //        {
                //            readyButton.GetComponent<Button>().interactable = true;
                //        }
                //        else
                //        {
                //            readyButton.GetComponent<Button>().interactable = false;
                //        }
                //        break;
                //    case 1:
                //        GameObject foldButton = actionButtonGroup[i].transform.GetChild(0).gameObject;
                //        if (room.operations.fold)
                //        {
                //            foldButton.GetComponent<Button>().interactable = true;
                //        }
                //        else
                //        {
                //            foldButton.GetComponent<Button>().interactable = false;
                //        }
                //        GameObject checkButton = actionButtonGroup[i].transform.GetChild(1).gameObject;
                //        if (room.operations.check)
                //        {
                //            checkButton.GetComponent<Button>().interactable = true;
                //        }
                //        else
                //        {
                //            checkButton.GetComponent<Button>().interactable = false;
                //        }
                //        GameObject callButton = actionButtonGroup[i].transform.GetChild(2).gameObject;
                //        if (room.operations.call)
                //        {
                //            callButton.GetComponent<Button>().interactable = true;
                //        }
                //        else
                //        {
                //            callButton.GetComponent<Button>().interactable = false;
                //        }
                //        GameObject raiseButton = actionButtonGroup[i].transform.GetChild(3).gameObject;
                //        if (room.operations.raise)
                //        {
                //            raiseButton.GetComponent<Button>().interactable = true;
                //        }
                //        else
                //        {
                //            raiseButton.GetComponent<Button>().interactable = false;
                //        }
                //        break;
                //    case 2:
                //        break;
                //    default:
                //        break;
                //}
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

    void SetAutoButtons()
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

    void SetRaiseAmounts()
    {
        if (room.gameStatus == 2)
        {
            int[] gamersCoinArray = new int[room.options.max_seats];
            for (int i = 0; i < gamersCoinArray.Length; i++)
            {
                if (room.seats[i] != null)
                {
                    gamersCoinArray[i] = room.chips[i];
                }
                else
                {
                    gamersCoinArray[i] = 0;
                }
            }


            minRaiseAmount = 0;
            maxRaiseAmount = room.gamers[Globals.userProfile.uid].coins - gamersCoinArray.Max();
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

    void SetRaiseBar()
    {
        if (room.gameStatus == 2)
        {
            GameObject[] raiseBarGrades = GameObjectHelper.GetChildren(raiseBarGradeParent);
            raiseToValue.GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated((long)raiseAmount, 1);
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

    void CheckRaiseAmount()
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

    public void SetGamersActionStatus()
    {
        if (room.gameStatus == 2)
        {
            if (room.status != null)
            {
                GameObject[] usersArray = GameObjectHelper.GetChildrenForRoomSize(usersParent,room.options.max_seats);
                string[] gamerActionStatus = room.status;
                string[] rotatedGamerActionStatus = ArrayHelper.RotateArray(gamerActionStatus, room.GetUserSeat());

                for (int i = 1; i < usersArray.Length; i++)
                {
                    if (rotatedGamerActionStatus[i] != null)
                    {
                        usersArray[i].transform.GetChild(4).gameObject.SetActive(true);
                        usersArray[i].transform.GetChild(4).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = rotatedGamerActionStatus[i];
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
        Globals.currentRoom = roomId;
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
    }

    private void OnSendChatResponse(JToken jsonResponse)
    {
    }

    void SetChatHistory()
    {
        if (Globals.chatHistory.ContainsKey(room.id))
        {
            chatHistory.GetComponent<TMP_Text>().text = Globals.chatHistory[room.id];
        }
    }

    void SetTournamentInfo()
    {
        int smallBlinds = Globals.tournamentInfo.small_bilnd;
        int nextSmallBlinds = 2 * smallBlinds;
        int bigBlinds = 2 * smallBlinds;
        int nextBigBlinds = 2 * bigBlinds;
        int blindUpTime = Globals.tournamentInfo.timeleft;
        
        string smallBlindStr = MoneyHelper.FormatNumberAbbreviated(smallBlinds, 1);
        string bigBlindStr = MoneyHelper.FormatNumberAbbreviated(bigBlinds, 1);
        string nextSmallBlindStr = MoneyHelper.FormatNumberAbbreviated(nextSmallBlinds, 1);
        string nextBigBlindStr = MoneyHelper.FormatNumberAbbreviated(nextBigBlinds, 1);
        string blindUpTimeStr = TimeSpan.FromSeconds(blindUpTime).ToString(@"hh\:mm\:ss");

        blindsInfo.GetComponent<TMP_Text>().text = smallBlindStr + "/" + bigBlindStr;
        newBlindsInfo.GetComponent<TMP_Text>().text = nextSmallBlindStr + "/" + nextBigBlindStr;
        blindsUpTimeInfo.GetComponent<TMP_Text>().text = blindUpTimeStr;

    }
}
