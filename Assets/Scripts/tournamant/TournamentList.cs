using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class TournamentList : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
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

    private void OnGetListResponse(JToken jsonResponse)
    {
        try
        {
            Dictionary<string, object> res = JsonResponse.ToDictionary(jsonResponse);
            TournamentObject[] tournamentList = JsonConvert.DeserializeObject<TournamentObject[]>(res["ret"].ToString());
            Debug.Log(tournamentList);
        } catch(Exception e)
        {
            Debug.Log(e);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
