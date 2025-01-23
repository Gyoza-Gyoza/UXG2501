using System.Runtime.InteropServices;
using UnityEngine;

public static class WebGLInteraction
{
    [DllImport("__Internal")]
    private static extern void RefreshPage();

    public static void TriggerRefresh()
    {
        // Call the JavaScript function
        RefreshPage();
    }
}