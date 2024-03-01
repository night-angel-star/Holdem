using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using static RpcLoginCommand;

public class GameCommands
{
    public bool? fastsignup;
    public bool? signup;
    public bool? login;
    public bool? logout;
}
public class RpcCommand
{
    public int seq;
    public string uid;
    public int pin;
    public string f;
    public object args;
}

public class RpcLoginCommand : RpcCommand
{
    public string f = "login";
    public class LoginArgs{
        public string uid;
        public string passwd;
    };
    public new LoginArgs args = new LoginArgs();
    public void SetArgs(string uid, string passwd)
    {
        args.uid = uid;
        args.passwd = passwd;
    }
}

public class RpcLogoutCommand : RpcCommand
{
    public string f = "logout";
    public new int args = 0;
    public void Set(string uid, int pin)
    {
        this.uid = uid;
        this.pin = pin;
    }
}
public class RpcRetResponse
{
    public int seq;
    /*
     * 0 : success
     * 400, bad request, invalid argument, invalid action, etc.
     * 403, denied, invalid password, etc.
     * 404, not found.
     * 500, server side error, for example, database error, etc.
     */
    public int err; 
    public JToken ret;
}

public class RpcRetLogin : RpcRetResponse
{
    public class RpcLoginRet
    {
        public Token token;
        public Gamer profile;
        public object cmds;
    }
}
public class NotifyEvent
{
    public string uid;
    public string e;
    public JToken args;
}

public class RoomEvent : NotifyEvent
{
    public int roomid;
}
public class LookNotifyEvent : RoomEvent
{
    public new Room args;
}
public class EnterNotifyEvent : RoomEvent
{
    public class EnterEventArgs {
        public Gamer who;
        public int where = 0;
    }
    public new EnterEventArgs args;
    public int room;
}
public class TakeSeatNotifyEvent : RoomEvent
{
    public class TakeSeatEventArgs
    {
        public string uid;
        public int where;
        public int roomid;
    }
    public new TakeSeatEventArgs args;
}
public class UnSeatNotifyEvent : RoomEvent
{
    public class UnSeatEventArgs
    {
        public string uid;
        public int where;
    }
    public new UnSeatEventArgs args;
}
public class LeaveNotifyEvent : RoomEvent
{
    public class LeaveEventArgs
    {
        public string uid;
        public int where;
    }
    public new LeaveEventArgs args;
}

public class ReadyNotifyEvent : RoomEvent
{
    public class ReadyEventArgs
    {
        public string uid;
        public int roomid;
        public int where;
    }
    public new ReadyEventArgs args;
}
public class GameStartNotifyEvent : RoomEvent
{
    public class GameStartEventArgs
    {
        public Room room;
        public int[] seats;
    }
    public new GameStartEventArgs args;
}

public class DealNotifyEvent : RoomEvent
{
    public class DealEventArgs
    {
        public JToken[][] deals;
        public int delay;
    }
    public new DealEventArgs args;
}
