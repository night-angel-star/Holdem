using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabSelect
{
    public static void TabKeyDown(Selectable[] inputFields)
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = null;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                next = inputFields[GetPreviousIndex(inputFields)];
            }
            else
            {
                next = inputFields[GetNextIndex(inputFields)];
            }

            if (next != null)
            {
                next.Select();
            }
        }
    }

    private static int GetNextIndex(Selectable[] inputFields)
    {
        int currentIndex = GetCurrentIndex(inputFields);
        return (currentIndex + 1) % inputFields.Length;
    }

    private static int GetPreviousIndex(Selectable[] inputFields)
    {
        int currentIndex = GetCurrentIndex(inputFields);
        return (currentIndex - 1 + inputFields.Length) % inputFields.Length;
    }

    private static int GetCurrentIndex(Selectable[] inputFields)
    {
        for (int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i].gameObject == EventSystem.current.currentSelectedGameObject)
            {
                return i;
            }
        }
        return -1;
    }
}
