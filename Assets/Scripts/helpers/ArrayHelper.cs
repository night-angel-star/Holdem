using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayHelper
{
    public static T[] RotateArray<T>(T[] array, int index)
    {
        if (index < 0 || index >= array.Length)
        {
            throw new ArgumentException("Invalid index");
        }

        T[] rotatedArray = new T[array.Length];

        for (int i = 0; i < array.Length; i++)
        {
            int rotatedIndex=RotateNumber(i, index, array.Length);
            rotatedArray[rotatedIndex] = array[i];
        }

        return rotatedArray;
    }

    public static int RotateNumber(int number,int index, int size)
    {
        int rotatedIndex = (number + size - index) % size;
        return rotatedIndex;
    }
}
