using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TournamentBehavior : GameBehavior
{
    public GameObject blindsInfo;
    public GameObject newBlindsInfo;
    public GameObject blindsUpTimeInfo;
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
                //InitializeAddChipsModal();
                SetPublicCards();
                SetTimer();
                SetActionButtonAreaIndexByGlobal();
                SetRaiseAmounts();
                SetRaiseBar();
                SetAutoButtons();
                CheckRaiseAmount();
                SetGamersActionStatus();
                //SetRoomsToggler();
                //SetRoomsView();
                SetTournamentInfo();
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
            LogHelper.AppLog("Tournament");
            LogHelper.AppLog(ex.ToString());
        }
    }

    protected new void GetActionButtonsInteractable()
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

        readyButton.GetComponent<Button>().interactable = false;
        readyButton.SetActive(false);
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
