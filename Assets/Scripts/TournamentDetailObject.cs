using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentDetailObject
{
    public int id = 0;
    public string status = "";
    public string name = "";
    public int delay = 0;
    public int buy_in = 0;
    public int registered_players = 0;
    public int min_players = 0;
    public int max_players = 0;
    public int count_down = 0;
    //public int end_date;
    public int first_blind = 0;
    public int[] prize;
    public int rise_count = 0;
    public int rise_time = 0;
    public Dictionary<string, object> rule;
    public long start_date = 0;
    public bool is_registered;
}