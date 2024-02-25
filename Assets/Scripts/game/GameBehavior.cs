using Newtonsoft.Json.Linq;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject myCards;
    private void OnRpcResponse(JToken jsonResponse)
    {
        if (jsonResponse == null)
        {
            return;
        }
    }
    void Start()
    {
        int[] myCardsNumber = new int[] {53, 18};
        GetMyCard(myCardsNumber);
    }

    void GetMyCard(int[] cardNumber)
    {
        myCards.SetActive(true);
        GameObject card1 = myCards.transform.GetChild(0).gameObject;
        GameObject card2 = myCards.transform.GetChild(1).gameObject;
        card1.GetComponent<SpriteRenderer>().sprite = CardHelper.GetCard(cardNumber[0]);
        card2.GetComponent<SpriteRenderer>().sprite = CardHelper.GetCard(cardNumber[1]);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    
}
