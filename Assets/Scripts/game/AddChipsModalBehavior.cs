using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AddChipsModalBehavior : MonoBehaviour
{
    public Slider chipSlider;
    public TMP_Text currentChipValue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        chipSlider.value=(int) Math.Floor(chipSlider.value); 
        currentChipValue.text=chipSlider.value.ToString();
    }
}
