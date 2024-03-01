using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScript : MonoBehaviour
{
    bool loadedLoginScene = false;
    // Start is called before the first frame update
    void Start()
    {
        Globals.socketIoConnection.serverUri = Globals.strUri;
        
        Globals.socketIoConnection.Connect();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!loadedLoginScene)
        {
            if (Globals.connected)
            {
                loadedLoginScene = true;
                SceneManager.LoadScene("Login");
            }
        }
    }
}
