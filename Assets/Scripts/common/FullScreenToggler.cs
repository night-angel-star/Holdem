using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class FullScreenToggler : MonoBehaviour
{
#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void FullScreenToggle();
#endif
    // Start is called before the first frame update
    public void ToggleScreen()
    {
#if UNITY_WEBGL
        Debug.Log("Toggling full screen status");
        FullScreenToggle();
#endif
    }
}
