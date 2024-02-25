using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Tables;
using UnityEngine;

public class TableScript
{
    public static void AddStringToCell(TableCell cell, string value)
    {
        GameObject text = new GameObject("Text");
        TextMeshProUGUI textMeshPro = text.AddComponent<TextMeshProUGUI>();
        textMeshPro.transform.SetParent(cell.transform);
        textMeshPro.fontSize = 10;
        textMeshPro.alignment = TextAlignmentOptions.MidlineLeft;
        textMeshPro.transform.localScale = Vector3.one;
        textMeshPro.SetText(value);
    }
}
