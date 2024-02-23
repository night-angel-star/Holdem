using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartPage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("startup");
        Globals globals = new Globals();

        //SceneManager.LoadScene("Login");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
