using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Tables;
using UnityEngine;

public class TableScript : MonoBehaviour
{
    public void AddRowToTable(TableLayout table, string[] rowElements)
    {
        TableRow newRow = table.AddRow();
        newRow.preferredHeight = 23;
        for (int i = 0; i < rowElements.Length; i++)
        {
            GameObject text = new GameObject("Text");
            TextMeshProUGUI textMeshPro = text.AddComponent<TextMeshProUGUI>();
            textMeshPro.transform.SetParent(newRow.Cells[i].transform);
            textMeshPro.fontSize = 10;
            textMeshPro.alignment = TextAlignmentOptions.MidlineLeft;
            textMeshPro.transform.localScale = Vector3.one;
            textMeshPro.SetText(rowElements[i]);
        }
    }

    public void setCellStyle(TableRow row, string text, int index)
    {
            GameObject textObject = new GameObject("Text");
            TextMeshProUGUI textMeshPro = textObject.AddComponent<TextMeshProUGUI>();
            textMeshPro.transform.SetParent(row.Cells[index].transform);
            textMeshPro.fontSize = 10;
            textMeshPro.alignment = TextAlignmentOptions.MidlineLeft;
            textMeshPro.transform.localScale = Vector3.one;
            textMeshPro.SetText(text);
    }
}
