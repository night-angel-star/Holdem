using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using System;
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
    public static bool connected = false;
    public static Token gameToken = new Token();
    public static Gamer userProfile = new Gamer();
    public static GameRoomsT gameRooms = new GameRoomsT();
    public static string currentRoom = null;
    public static string strUri = "http://192.168.148.182:3000";




    public static object profile;
    public static object token;
    public static int[] roomIdArray = { -1, -1, -1 };
    public static bool[] roomGameStarted = { false, false, false };
    public static Dictionary<string, object>[] rooms = { null, null, null };
    public static Dictionary<string, object>[] roomStates = { null, null, null };
    public static string[][] gamersActionStates = new string[3][];
    public static int[][] shareCards = new int[3][];
    public static int currentRoomId = -1;
    public static int currentRoomIndex = -1;

    public static int getBlankRoomIndex()
    {
        return Array.IndexOf(roomIdArray, -1);
    }

    public static int getActiveRoomIndex()
    {
        int index = -1;
        for (int i = 0; i < roomIdArray.Length; i++)
        {
            if (roomIdArray[i] != -1)
            {
                index = i;
                break;
            }
        }
        return index;
    }
    public static int getRoomIndex(int index)
    {
        return Array.IndexOf(roomIdArray, index);
    }
}
