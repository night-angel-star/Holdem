using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TabManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] Tabs;
    public GameObject[] TabsButton;
    public GameObject[] TabsButtonActive;
    public int defaultActive=0;

    void Start()
    {
        activateTab(defaultActive);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void activateTab(int index)
    {
        
        for(int i=0; i<Tabs.Length; i++)
        {
            if (i == index)
            {
                Tabs[i].SetActive(true);
                TabsButton[i].SetActive(false);
                TabsButtonActive[i].SetActive(true);
            }
            else
            {
                Tabs[i].SetActive(false);
                TabsButton[i].SetActive(true);
                TabsButtonActive[i].SetActive(false);
            }
            
        }
    }
}