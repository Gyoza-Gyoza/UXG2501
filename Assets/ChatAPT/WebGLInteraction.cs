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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) ReceiveMessage("HideRecycleBin");
    }
    public void ReceiveMessage(string message)
    {
        Debug.Log($"[UNITY] ✅ Received message from browser: {message}");

        if (message == "HideRecycleBin")
        {
            if (PhaseManager.Instance.CurrentPhase is BuildingTrust buildingTrust)
            {
                buildingTrust.HideBin();
            }

            //if (recycleBinObj != null)
            //{
            //    Debug.Log("[UNITY] Hide Recycle Bin");
            //    recycleBinObj.SetActive(false);
            //}
        }
    }
}
