using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogHelper : MonoBehaviour
{
    public static string appDir = Directory.GetCurrentDirectory();
    public static string networkLogFileName = "network.log";
    public static string appLogFileName = "app.log";

    public static void NetworkLog(string text)
    {
#if UNITY_WEBGL||UNITY_ANDROID
        string timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        Debug.Log("[" + timestamp + "]          " + text);
#else
        using (StreamWriter writer = File.AppendText(appDir + "/" + networkLogFileName))
        {
            // Get the current timestamp
            string timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            // Write the timestamp and text to the file
            writer.WriteLine("[" + timestamp + "]          " + text);
        }
#endif
    }

    public static void AppLog(string text)
    {
#if UNITY_WEBGL||UNITY_ANDROID
        string timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        Debug.Log("[" + timestamp + "]          " + text);
#else
        using (StreamWriter writer = File.AppendText(appDir + "/" + appLogFileName))
        {
            // Get the current timestamp
            string timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            // Write the timestamp and text to the file
            writer.WriteLine("[" + timestamp + "]          " + text);
        }
#endif
    }
}
