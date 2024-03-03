using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Tables;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TournamentObject
{
    public int id;
    public string status;
    public string name;
    public int delay;
    public int buy_in;
    public int registered_players;
    public int max_players;
}

public class TournamentDetailObject
{
    public int id;
    public string status;
    public string name;
    public int delay;
    public int buy_in;
    public int registered_players;
    public int max_players;
    public int count_down;
    public int end_date;
    public int first_blind;
    public int prize;
    public int rise_count;
    public int rise_time;
    public Dictionary<string, object> rule;
    public int start_date;
}

enum ViewType { List, Detail }

public class Tournament : MonoBehaviour
{
    public TableLayout tournamentListTable;
    public GameObject tournamentListRowPrefab;
    public GameObject listContainer;
    public GameObject detailContainer;

    ViewType currentView = ViewType.List;
    int currentDetailId = -1;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("GetData", 0f, 1f);
    }

    void GetData()
    {
        if (currentView == ViewType.List)
        {
            currentDetailId = -1;
            listContainer.SetActive(true);
            detailContainer.SetActive(false);
            GetList();
        }
        else if (currentView == ViewType.Detail)
        {
            listContainer.SetActive(false);
            detailContainer.SetActive(true);
        }

    }

    void GetList()
    {
        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;
        var data = new
        {
            uid = uid,
            pin = pin,
            f = "viewTournaments",
            args = "0",
        };
        Globals.socketIoConnection.SendRpc(data, OnGetListResponse);
        
    }

    void GetDetail()
    {
        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;
        var data = new
        {
            uid = uid,
            pin = pin,
            f = "tournamentInfo",
            args = new
            {
                id = currentDetailId,
                uid = Globals.gameToken.uid
            },
        };
        Globals.socketIoConnection.SendRpc(data, OnGetListResponse);

    }

    void OnGetListResponse(JToken jsonResponse)
    {
        try
        {
            Dictionary<string, object> res = JsonResponse.ToDictionary(jsonResponse);
            TournamentObject[] tournamentList = JsonConvert.DeserializeObject<TournamentObject[]>(res["ret"].ToString());
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UpdateList(tournamentList);
            });
            
        } catch(Exception e)
        {
            Debug.Log(e);
        }
    }

    void OnGetDetailResponse(JToken jsonResponse)
    {
        try
        {
            Dictionary<string, object> res = JsonResponse.ToDictionary(jsonResponse);
            TournamentObject tournamentList = JsonConvert.DeserializeObject<TournamentObject>(res["ret"].ToString());
            

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    void UpdateList(TournamentObject[] tournamentList)
    {
        tournamentListTable.ClearRows();
        for (int i = 0; i < tournamentList.Length; i++)
        {
            AddListRow(tournamentList[i]);
        }
    }

    void AddListRow(TournamentObject tournamentListItem)
    {
        GameObject newRow = Instantiate(tournamentListRowPrefab);

        newRow.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = tournamentListItem.status;
        newRow.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = tournamentListItem.name;
        newRow.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = tournamentListItem.delay.ToString();
        newRow.transform.GetChild(3).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated(tournamentListItem.buy_in,0);
        newRow.transform.GetChild(4).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = tournamentListItem.registered_players+"/"+tournamentListItem.max_players;
        newRow.GetComponent<Button>().onClick.AddListener(() => OpenDetail(tournamentListItem.id));
        newRow.transform.SetParent(tournamentListTable.transform);
    }

    void OpenDetail(int id)
    {
        Debug.Log(id);

        currentDetailId = id;
        currentView = ViewType.Detail;
    }
}
