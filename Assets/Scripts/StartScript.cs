using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Globals.socketIoConnection.serverUri = Globals.strUri;
        Globals.socketIoConnection.Connect();
        SceneManager.LoadScene("Login");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
