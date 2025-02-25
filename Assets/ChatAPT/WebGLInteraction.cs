using System.Runtime.InteropServices;
using UnityEngine;

public class WebGLInteraction : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void RefreshPage();

    public static void TriggerRefresh()
    {
        // Call the JavaScript function
        RefreshPage();
    }

    [DllImport("__Internal")]
    private static extern void SendToBrowser(string message);

    public static void PrintToBrowserConsole(string message)
    {
        Debug.Log("[UNITY] " + message);

        #if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            SendToBrowser("[UNITY] " + message);
        }
        catch
        {
            Debug.LogWarning("[UNITY] JavaScript call failed! Running in Editor?");
        }
        #endif
    }

    void Start()
    {
        PrintToBrowserConsole("Test log from Unity WebGL - Startup message");
    }

    public void OnBackgroundColorChanged(string newColor)
    {
        string logMessage = "Detected Background Color Change: " + newColor;
        Debug.Log(logMessage); // Unity Console Log
        PrintToBrowserConsole(logMessage); // Also send to browser console
    }
}
