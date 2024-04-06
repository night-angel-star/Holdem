using EasyUI.Toast;
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

enum ViewType { List, Detail }

public class Tournament : MonoBehaviour
{
    public TableLayout tournamentListTable;
    public GameObject tournamentListRowPrefab;
    public GameObject listContainer;
    public GameObject detailContainer;

    public GameObject tournamentDetailName;
    public GameObject tournamentDetailStatus;
    public GameObject tournamentDetailDelay;
    public GameObject tournamentDetailBuyIn;
    public GameObject registerButton;
    public GameObject watchButton;


    ViewType currentView = ViewType.List;
    int currentDetailId = -1;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("GetData", 0f, 2f);
    }

    void GetData()
    {
        if (listContainer != null && detailContainer != null)
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
                GetDetail();
            }
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
                tid = currentDetailId,
                uid = Globals.gameToken.uid
            },
        };
        Globals.socketIoConnection.SendRpc(data, OnGetDetailResponse);
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

        }
        catch (Exception e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Toast.Show("Error occured", "danger");
            });
            Debug.Log(e);
        }
    }

    void OnGetDetailResponse(JToken jsonResponse)
    {
        try
        {
            Dictionary<string, object> res = JsonResponse.ToDictionary(jsonResponse);
            TournamentDetailObject tournamentDetailObject = JsonConvert.DeserializeObject<TournamentDetailObject>(res["ret"].ToString());
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UpdateDetail(tournamentDetailObject);
            });
            
        }
        catch (Exception e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Toast.Show("Error occured", "danger");
            });
            Debug.Log(e);
        }

    }

    void UpdateList(TournamentObject[] tournamentList)
    {
        if (tournamentListTable != null)
        {
            tournamentListTable.ClearRows();
            for (int i = 0; i < tournamentList.Length; i++)
            {
                AddListRow(tournamentList[i]);
            }
        }
        
    }

    void AddListRow(TournamentObject tournamentListItem)
    {
        if (tournamentListTable != null)
        {
            GameObject newRow = Instantiate(tournamentListRowPrefab);

            newRow.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = tournamentListItem.status;
            newRow.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = tournamentListItem.name;
            newRow.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = tournamentListItem.delay.ToString();
            newRow.transform.GetChild(3).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated(tournamentListItem.buy_in, 0);
            newRow.transform.GetChild(4).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = tournamentListItem.registered_players + "/" + tournamentListItem.max_players;
            newRow.GetComponent<Button>().onClick.AddListener(() => OpenDetail(tournamentListItem.id));
            newRow.transform.SetParent(tournamentListTable.transform);
        }
        
    }

    void OpenDetail(int id)
    {
        currentDetailId = id;
        currentView = ViewType.Detail;
    }

    void UpdateDetail(TournamentDetailObject tournamentDetailObject)
    {
        if (tournamentDetailName != null)
        {
            tournamentDetailName.GetComponent<TMP_Text>().text = tournamentDetailObject.name;
        }
        if (tournamentDetailStatus != null)
        {
            tournamentDetailStatus.GetComponent<TMP_Text>().text = tournamentDetailObject.status;
        }
        if (tournamentDetailDelay != null)
        {
            tournamentDetailDelay.GetComponent<TMP_Text>().text = tournamentDetailObject.delay.ToString();
        }
        if (tournamentDetailBuyIn != null)
        {
            tournamentDetailBuyIn.GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated(tournamentDetailObject.buy_in, 0);
        }
        
    }

    public void OnRegister()
    {
        string uid = Globals.gameToken.uid;
        int pin = Globals.gameToken.pin;
        var data = new
        {
            uid = uid,
            pin = pin,
            f = "registerOne",
            args = new
            {
                tid = currentDetailId,
                uid = Globals.gameToken.uid
            },
        };
        Globals.socketIoConnection.SendRpc(data, OnRegisterResponse);
    }

    void OnRegisterResponse(JToken jsonResponse)
    {
        try
        {
            Dictionary<string, object> res = JsonResponse.ToDictionary(jsonResponse);
            Dictionary<string, object> ret = NewtonSoftHelper.JObjectToObject<string, object>(res["ret"]);
            if (ret.ContainsKey("reg"))
            {
                if (ret["reg"].ToString() == "True")
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        Toast.Show("Register success");
                    });
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        Toast.Show("Register fail", "danger");
                    });
                }
            }
            else
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Toast.Show("Register fail", "danger");
                });
            }

        }
        catch (Exception e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Toast.Show("Error occured", "danger");
            });
            Debug.Log(e);
        }
    }
}
