using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectHelper : MonoBehaviour
{
    public static GameObject[] GetActiveChildren(GameObject parent)
    {
        Transform[] allChildren = new Transform[parent.transform.childCount];
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            allChildren[i] = parent.transform.GetChild(i);
        }

        // Filter out the inactive child GameObjects
        List<GameObject> activeChildrenList = new List<GameObject>();
        foreach (Transform child in allChildren)
        {
            if (child.gameObject != parent && child.gameObject.activeInHierarchy)
            {
                activeChildrenList.Add(child.gameObject);
            }
        }

        // Convert the list of active child GameObjects to an array
        GameObject[] activeChildrenArray = activeChildrenList.ToArray();

        return activeChildrenArray;
    }

    public static GameObject[] GetChildren(GameObject parent)
    {
        GameObject[] allChildren = new GameObject[parent.transform.childCount];
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            allChildren[i] = parent.transform.GetChild(i).gameObject;
        }



        return allChildren;
    }
}
