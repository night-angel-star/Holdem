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
}
