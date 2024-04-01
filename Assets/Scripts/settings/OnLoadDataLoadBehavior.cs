using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnLoadDataLoadBehavior : MonoBehaviour
{
    public GameObject scriptObj;
    // Start is called before the first frame update
    void Start()
    {
        scriptObj.GetComponent<LoadProfileInfoBehavior>().LoadProfileInfo();
    }
}
