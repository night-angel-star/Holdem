using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    public GameObject[] buttons;
    public GameObject[] buttonsActive;
    public int defaultActive = 0;
    public int currentActive = 0;
    // Start is called before the first frame update
    void Start()
    {
        activateButton(defaultActive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activateButton(int index)
    {
        currentActive = index;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == index)
            {

                buttons[i].SetActive(false);
                buttonsActive[i].SetActive(true);
            }
            else
            {
                buttons[i].SetActive(true);
                buttonsActive[i].SetActive(false);
            }

        }
    }
}
