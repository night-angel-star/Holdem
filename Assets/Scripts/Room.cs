using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

using JsonResponseT = System.Collections.Generic.Dictionary<string, object>;
enum RoomType {
    Limit = 0,
    PotLimit = 1,
    NoLimit = 2
}
public class Room 
{
    public Dictionary<int, int[]> cards = new Dictionary<int, int[]>();
    public int casino = -1;
    public Dictionary<int, int> chips = null;
    public int dealer_seat = -1;
    public int activeSeat = -1;
    public int countdown = -1;
    public int totalCount = -1;
    public Dictionary<string, Gamer> gamers = new Dictionary<string, Gamer>();
    public int gamers_count = 0;
    public string id = null;
    public int max_chip = 0;
    public string name = null;
    public int last_raise = 0;
    public string[] status = new string[9];
    public int gameStatus = -1;
    public class Options {
        public int big_blind = 0;
        public int small_blind = 0;
        public int limit_rule = 0; // 0: limit, 1: pot limit, 2: no limit
        public int limit = 0;
        public int limit_cap = 0;
        public string roomClass = null;
        public int max_seats = 0;
        public int min_buy = 0;
        public int[] no_color = null; 
        public bool no_joker = true;
        public int[] no_number;
        public int ready_countdown = 0;
        public int turn_countdown = 0;
    } ;
    public Options options = new Options();
    public int pot = 0 ;
    public int[] pot_chips = null;
    public string[] seats = null;
    public int seats_count = 0;
    public int seats_taken = 0;
    public int[] shared_cards = new int[5];
    public string type = null;

    public class Operations {
        public bool fold = false;
        public bool check = false;
        public bool call = false;
        public bool raise = false;
        public bool takeseat = false;
        public bool ready = false;
    }
    public Operations operations = new Operations();

    public int GetUserSeat()
    {
        int user_seat = -1;
        string uid = Globals.userProfile.uid;
        for (int i = 0; i< seats.Length; i++)
        {
            if (seats[i] == uid)
            {
                user_seat = i;
                break;
            }
        }
        return user_seat;

    }
    public void UpdateCmdsFromJson(JToken baseData)
    {
        JsonResponseT cmdsDictionary = JsonResponse.ToDictionary(baseData);
        foreach (KeyValuePair<string, object> cmd in cmdsDictionary)
        {
            if (typeof(Operations).GetField(cmd.Key) == null) { continue; }

            JToken jToken = cmd.Value as JToken;
            if (jToken.Type == JTokenType.Null)
            {
                typeof(Operations).GetField(cmd.Key).SetValue(operations, false);
            } else if (jToken.Type == JTokenType.Boolean)
            {
                typeof(Operations).GetField(cmd.Key).SetValue(operations, jToken.Value<bool>());
            }
        }
    }

}
