using UnityEngine;
using UnityEngine.UI; // Required for UI components

public class WebGLInteraction : MonoBehaviour
{
    public GameObject recycleBinObj; // ✅ Manually assign in Inspector

    void Start()
    {
        Debug.Log("[UNITY] ✅ WebGL Script Loaded");

        if (recycleBinObj == null)
        {
            recycleBinObj = GameObject.Find("Recycle Bin"); // Fallback if not assigned
        }

        if (recycleBinObj != null)
        {
            Debug.Log("[UNITY] ✅ Recycle Bin UI found.");
        }
        else
        {
            Debug.LogWarning("[UNITY] ❌ Recycle Bin UI NOT found! Assign it manually.");
        }
    }

    public void ReceiveMessage(string message)
    {
        Debug.Log($"[UNITY] 📩 Received message from browser: {message}");

        if (message == "HideRecycleBin")
        {
            if (recycleBinObj != null)
            {
                Debug.Log("[UNITY] 🗑️ Hiding Recycle Bin...");
                recycleBinObj.SetActive(false); // ✅ Hide the UI
            }
            else
            {
                Debug.LogWarning("[UNITY] ❌ Recycle Bin object still not found!");
            }
        }
    }
}
