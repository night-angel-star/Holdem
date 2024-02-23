using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleModal : MonoBehaviour
{
    
    
    
    public void OpenModal(GameObject modal)
    {
        
        modal.SetActive(true);
    }

    public void CloseModal(GameObject modal)
    {
        
        modal.SetActive(false);
    }
}
