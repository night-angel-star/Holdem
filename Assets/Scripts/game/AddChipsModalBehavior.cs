using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        currentChipValue.text=chipSlider.value.ToString();
    }
}
