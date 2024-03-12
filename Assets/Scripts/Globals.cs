using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Sockets;
using SocketIOClient.Transport;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

using GameRoomsT =  System.Collections.Generic.Dictionary<string, Room>;
public class Token
{
    public string uid;
    public int pin;
}

public static class Globals
{
    public static SocketIoConnection socketIoConnection = new SocketIoConnection();
    public static GlobalNotification notification = new GlobalNotification();
    public static bool connected = false;
    public static Token gameToken = new Token();
    public static Gamer userProfile = new Gamer();
    public static GameRoomsT gameRooms = new GameRoomsT();
    public static string currentRoom = null;
    public static string strUri = "http://124.158.120.246/";

    
}
