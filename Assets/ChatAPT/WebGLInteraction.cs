using UnityEngine;
using UnityEngine.UI; // Required for UI components

public class WebGLInteraction : MonoBehaviour
{
    public GameObject recycleBinObj;

    void Start()
    {
        Debug.Log("[UNITY] ✅ WebGL Script Loaded");

        if (recycleBinObj != null)
        {
            Debug.Log("[UNITY] ✅ Recycle Bin UI found.");
        }
    }

    public void ReceiveMessage(string message)
    {
        Debug.Log($"[UNITY] ✅ Received message from browser: {message}");

        if (message == "HideRecycleBin")
        {
            if (recycleBinObj != null)
            {
                Debug.Log("[UNITY] Hide Recycle Bin");
                recycleBinObj.SetActive(false);
            }
        }
    }
}
