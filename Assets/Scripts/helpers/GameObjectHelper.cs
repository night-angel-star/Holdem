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

    public static GameObject[] GetChildrenForRoomSize(GameObject parent,int max_seats)
    {
        GameObject[] allChildren = new GameObject[max_seats];
        int childrenSize = 0;
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            switch (max_seats)
            {
                case 3:
                    if (i == 0 || i == 3 || i == 6)
                    {
                        allChildren[childrenSize] = parent.transform.GetChild(i).gameObject;
                        childrenSize++;
                    }
                    break;
                case 6:
                    if (i == 1 || i == 4 || i == 8)
                    {
                    }
                    else
                    {
                        allChildren[childrenSize] = parent.transform.GetChild(i).gameObject;
                        childrenSize++;
                    }
                    break;
                case 9:
                    allChildren[childrenSize] = parent.transform.GetChild(i).gameObject;
                    childrenSize++;
                    break;
                default:
                    break;
            }
            
        }



        return allChildren;
    }
}
