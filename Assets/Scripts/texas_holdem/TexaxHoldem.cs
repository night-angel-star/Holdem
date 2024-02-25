using UnityEngine;
using UI.Tables;
using TMPro;
using System;
using UI;
using Unity.VisualScripting;

public class TexasHoldem : MonoBehaviour
{
    public TableLayout roomListTable; // Reference to your TableLayout component

    // Start is called before the first frame update
    void Start()
    {
        string[] rowElements = { "Element 1", "Element 2", "Element 3", "Element 4", "Element 5", "Element 6" };
        AddRowToTable(rowElements);
    }

    public void AddRowToTable(string[] rowElements)
    {
        TableRow newRow = roomListTable.AddRow();
        newRow.preferredHeight = 23;
        for(int i = 0; i < rowElements.Length; i++)
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

    // Update is called once per frame
    void Update()
    {

    }
}