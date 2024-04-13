using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    //public int end_date;
    public int first_blind;
    public int prize;
    public int rise_count;
    public int rise_time;
    public Dictionary<string, object> rule;
    public int start_date;
}