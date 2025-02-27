using System.Runtime.InteropServices;
using UnityEngine;

public class WebGLInteraction : MonoBehaviour
{
    private static GameObject recycleBinObj;

    private void Start()
    {
        // Try to find the Recycle Bin when the script starts
        recycleBinObj = GameObject.Find("Recycle Bin");

        if (recycleBinObj != null)
        {
            Debug.Log("[UNITY] ✅ WebGLInteraction script loaded successfully.");
            Debug.Log("[UNITY] ✅ Recycle Bin object found at start.");
        }
        else
        {
            Debug.LogWarning("[UNITY] ❌ Recycle Bin object NOT found at start! Make sure it's in the scene.");
        }
    }

    [UnityEngine.Scripting.Preserve]
    public static void OnMessageReceived(string message)
    {
        Debug.Log("[UNITY] 📩 Received message from browser: " + message);

        if (message == "HideRecycleBin")
        {
            if (recycleBinObj == null)
            {
                recycleBinObj = GameObject.Find("Recycle Bin"); // Try to find it again
            }

            if (recycleBinObj != null)
            {
                Debug.Log("[UNITY] 🗑️ Recycle Bin found. Hiding now...");
                recycleBinObj.SetActive(false); // Hide the UI element
            }
            else
            {
                Debug.LogWarning("[UNITY] ❌ Recycle Bin object still not found! Check Unity Hierarchy.");
            }
        }
    }

    [DllImport("__Internal")]
    private static extern void RefreshPage();

    public static void TriggerRefresh()
    {
        // Call the JavaScript function
        RefreshPage();
    }
}
