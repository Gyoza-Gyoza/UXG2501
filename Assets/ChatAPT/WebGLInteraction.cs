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
        SendToBrowser("[UNITY] " + message);
        #endif
    }

    public void OnBackgroundColorChanged(string newColor)
    {
        string logMessage = "Detected Background Color Change: " + newColor;
        Debug.Log(logMessage); // Print in Unity console
        PrintToBrowserConsole(logMessage); // Also send to browser console
    }
}
