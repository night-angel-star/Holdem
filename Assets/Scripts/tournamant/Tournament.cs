using EasyUI.Toast;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public long start_date;
    public int buy_in;
    public int registered_players;
    public int max_players;
    public int[] prize;
    public bool is_registered;
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
    public GameObject tournamentDetailPrize;
    public GameObject tournamentDetailEntry;
    public GameObject tournamentDetailStartTime;

    public GameObject tournamentDetailInfoTable;
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
            if (tournamentListItem.status == "draft")
            {
                tournamentListItem.status = "registering";
            }
            newRow.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = tournamentListItem.status;
            newRow.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = tournamentListItem.name;
            DateTime startDateTime=new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc).AddMilliseconds(tournamentListItem.start_date);
            DateTime now = DateTime.Now;
            TimeSpan delta = startDateTime - now;
            string formattedDelta = "";
            if (delta.Days > 0)
            {
                formattedDelta += $"{delta.Days} day ";
            }
            formattedDelta += $"{delta.Hours:D2}:{delta.Minutes:D2}:{delta.Seconds:D2}";
            if (DateTime.Compare(startDateTime,now)>0)
            {
                newRow.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = formattedDelta;
            }
            else
            {
                newRow.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = "Started";
            }
            
            newRow.transform.GetChild(3).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated(tournamentListItem.buy_in, 0);
            newRow.transform.GetChild(3).gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = "Prize "+MoneyHelper.FormatNumberAbbreviated(tournamentListItem.prize.Sum(), 0);
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
            if (tournamentDetailObject.status == "draft")
            {
                tournamentDetailObject.status = "registering";
            }
            tournamentDetailStatus.GetComponent<TMP_Text>().text = tournamentDetailObject.status;
        }
        if (tournamentDetailDelay != null)
        {
            DateTime startDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(tournamentDetailObject.start_date);
            DateTime now = DateTime.Now;
            TimeSpan delta = startDateTime - now;
            string formattedDelta = "";
            if (delta.Days > 0)
            {
                formattedDelta += $"{delta.Days} day ";
            }
            formattedDelta += $"{delta.Hours:D2}:{delta.Minutes:D2}:{delta.Seconds:D2}";
            if (DateTime.Compare(startDateTime, now) > 0)
            {
                tournamentDetailDelay.GetComponent<TMP_Text>().text = formattedDelta;
            }
            else
            {
                tournamentDetailDelay.GetComponent<TMP_Text>().text = "Started";
            }
        }
        if (tournamentDetailBuyIn != null)
        {
            tournamentDetailBuyIn.GetComponent<TMP_Text>().text = MoneyHelper.FormatNumberAbbreviated(tournamentDetailObject.buy_in, 0);
        }
        if (tournamentDetailPrize != null)
        {
            string prize = MoneyHelper.FormatNumberAbbreviated(tournamentDetailObject.prize.Sum(), 0);
            tournamentDetailPrize.GetComponent<TMP_Text>().text=prize;
        }
        if(tournamentDetailStartTime != null)
        {
            string startDate = "";
            DateTime startDateTime1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(tournamentDetailObject.start_date);
            startDate = startDateTime1.ToString("yyyy-MM-dd HH:mm:ss");
            tournamentDetailStartTime.GetComponent<TMP_Text>().text = startDate;
        }

        

        if (tournamentDetailInfoTable != null)
        {
            GameObject startingChips = tournamentDetailInfoTable.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
            GameObject gameType = tournamentDetailInfoTable.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
            GameObject minMaxPlayers = tournamentDetailInfoTable.transform.GetChild(2).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
            GameObject currentBlinds = tournamentDetailInfoTable.transform.GetChild(3).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
            GameObject blindLvlUp = tournamentDetailInfoTable.transform.GetChild(4).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
            GameObject breakTime = tournamentDetailInfoTable.transform.GetChild(5).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
            GameObject reEntry = tournamentDetailInfoTable.transform.GetChild(6).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
            GameObject gameSpeed = tournamentDetailInfoTable.transform.GetChild(7).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
            GameObject earlyBird = tournamentDetailInfoTable.transform.GetChild(8).gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;

            startingChips.GetComponent<TMP_Text>().text = tournamentDetailObject.buy_in.ToString();
            minMaxPlayers.GetComponent<TMP_Text>().text = $"<color=#FCB823>{tournamentDetailObject.min_players}</color>/{tournamentDetailObject.max_players}";
            if (Globals.tournamentInfo.started)
            {
                currentBlinds.GetComponent<TMP_Text>().text = $"lvl:<color=#FCB823>{Globals.tournamentInfo.small_bilnd}/{Globals.tournamentInfo.small_bilnd * 2}</color>({Globals.tournamentInfo.timeleft}s)";
            }
            else
            {
                currentBlinds.GetComponent<TMP_Text>().text = $"lvl:<color=#FCB823>{tournamentDetailObject.first_blind}/{tournamentDetailObject.first_blind * 2}</color>";
            }
            tournamentDetailEntry.GetComponent<TMP_Text>().text = tournamentDetailObject.registered_players.ToString();
            
            blindLvlUp.GetComponent<TMP_Text>().text = $"Lvl up every <color=#FC2323>{(int)(tournamentDetailObject.rise_time/60)} : {tournamentDetailObject.rise_time%60}</color>";


            if (tournamentDetailObject.is_registered)
            {
                registerButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                registerButton.GetComponent<Button>().interactable = true;
            }

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
                if (ret.ContainsKey("reg"))
                {
                    if (ret["reg"].ToString() == "True")
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            Toast.Show("Register success");
                            registerButton.GetComponent<Button>().interactable = false;
                        });
                        return;
                    }
                    else
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            Toast.Show("Invalid response", "danger");
                        });
                        return;
                    }
                }
                else
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        Toast.Show("Invalid response", "danger");
                    });
                }
            }
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Toast.Show(errorString, "danger");
            });
            return;
        } while (false);
        
    }
}
