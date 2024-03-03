using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Utils
{
    public static GameObject GetChildGameObject(GameObject fromGameObject, string withName)
    {
        Transform[] ts = fromGameObject.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts)
        {
            if (t.name == withName)
            {
                return t.gameObject;
            }
        }
        return null;
    }

    public static string GetFullTimeFormatFromSeconds(int seconds)
    {
        if(seconds > 3600 * 24)
        {
            int d = seconds / (60 * 60 * 24);
            string ret = d > 1 ? d.ToString() + " days left" : d.ToString() + " day left";
            return ret;
        }
        int h = seconds / 3600;
        int m = (seconds - h * 3600) / 60;
        int s = seconds - m * 60;
        string h_string = h < 10 ? "0" + h.ToString() : h.ToString();
        string m_string = m < 10 ? "0" + m.ToString() : m.ToString();
        string s_string = s < 10 ? "0" + s.ToString() : s.ToString();

        return h_string + ":" + m_string + ":" + s_string;
    }
}
