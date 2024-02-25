using UnityEngine;
using UI.Tables;
using TMPro;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using System;
using UnityEngine.SceneManagement;
using System.Globalization;
using UnityEngine.Windows;
using UnityEngine.UI;

public class TexasHoldem : MonoBehaviour
{
    public TableLayout roomListTable; // Reference to your TableLayout component
    public GameObject joinButtonPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Dictionary<string, object> token = (Dictionary<string, object>)Globals.token;
        string uid = token["uid"].ToString();
        int pin = Int32.Parse(token["pin"].ToString());
        var data = new
        {
            uid = uid,
            pin = pin,
            f = "rooms",
            args = "0"
        };
        Globals.socketIoConnection.SendRpc(data, OnResponse);
    }

    private void OnResponse(JToken jsonResponse)
    {
        Dictionary<string, object> res = JsonResponse.ToDictionary(jsonResponse);
        IEnumerable temp = JsonConvert.DeserializeObject(res["ret"].ToString()) as IEnumerable;
        JObject[] valueArray = temp.Cast<JObject>().ToArray();
        for (int i = 0; i < valueArray.Length; i++)
        {
            int index = i;
            Dictionary<string, object> dictionary = valueArray[i].ToObject<Dictionary<string, object>>();
            string[] rowContents = parseRow(dictionary);
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                AddRowToTable(rowContents, index);
            });
        }

    }

    private string[] parseRow(Dictionary<string, object> data)
    {
        string[] rowElements = { };
        Array.Resize(ref rowElements, 5);
        rowElements[0] = convertToTitleCase(data["type"].ToString());
        //Array.Resize(ref rowElements, rowElements.Length + 1);
        rowElements[1] = convertToTitleCase(data["room_name"].ToString());
        //Array.Resize(ref rowElements, rowElements.Length + 1);
        rowElements[2] = data["seats_taken"].ToString() + "/" + data["seat"].ToString();
        //Array.Resize(ref rowElements, rowElements.Length + 1);
        rowElements[3] = data["small_blind"].ToString() + "/" + data["big_blind"].ToString();
        //Array.Resize(ref rowElements, rowElements.Length + 1);
        rowElements[4] = data["min_buy"].ToString();
        return rowElements;
    }

    private string convertToTitleCase(string str)
    {
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        string output = textInfo.ToTitleCase(str);
        return output;
    }

    private void JoinHandler(int index)
    {
        Debug.Log(index);
    }

    public void AddRowToTable(string[] rowElements, int index)
    {
        TableRow newRow = roomListTable.AddRow();
        newRow.preferredHeight = 23;
        for (int i = 0; i < rowElements.Length; i++)
        {
            TableScript.AddStringToCell(newRow.Cells[i], convertToTitleCase(rowElements[i].ToString()));
        }
        GameObject cellObject = new GameObject("GameObject", typeof(RectTransform));
        cellObject.transform.SetParent(newRow.Cells[5].transform);
        cellObject.transform.localScale = Vector3.one;

        GameObject instantiatedButton = Instantiate(joinButtonPrefab, newRow.Cells[5].transform);
        instantiatedButton.transform.localScale = Vector3.one;

        Button button = instantiatedButton.GetComponent<Button>();
        button.onClick.AddListener(() => JoinHandler(index));
    }

    // Update is called once per frame
    void Update()
    {

    }
}