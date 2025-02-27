using System.Runtime.InteropServices;
using UnityEngine;

public class WebGLInteraction : MonoBehaviour
{
    public GameObject recycleBin;

    [DllImport("__Internal")]
    private static extern void RefreshPage();

    public static void TriggerRefresh()
    {
        // Call the JavaScript function
        RefreshPage();
    }

    [UnityEngine.Scripting.Preserve]
    public static void OnMessageReceived(string message)
    {
        Debug.Log("[UNITY] Received message from browser: " + message);

        if (message == "HideRecycleBin")
        {
            GameObject recycleBinObj = GameObject.Find("Recycle Bin");
            if (recycleBinObj != null)
            {
                recycleBinObj.SetActive(false); // Hide the UI image
                Debug.Log("[UNITY] Recycle Bin hidden!");
            }
            else
            {
                Debug.LogWarning("[UNITY] Recycle Bin object not found!");
            }
        }
    }
}
