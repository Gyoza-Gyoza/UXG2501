using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI; // Required for UI components

public class WebGLInteraction : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void ChangeWebBg();

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
        if (Input.GetKeyDown(KeyCode.Alpha9)) ReceiveMessage("RemoveButton");
    }
    public void ReceiveMessage(string message) //Takes in messages from javascript and performs behaviour based on it
    {
        Debug.Log($"[UNITY] ✅ Received message from browser: {message}");

        switch(message)
        {
            case "HideRecycleBin":
                if (PhaseManager.Instance.CurrentPhase is ChangingPermission changingBackground)
                {
                    changingBackground.HideBin();
                }
                break;

            case "RemoveButton": 
                if (PhaseManager.Instance.CurrentPhase is ChangeRootWriteAccess removeButton)
                {
                    removeButton.BlackScreen();
                }
                break;

            case "BlackScreen":
                if (PhaseManager.Instance.CurrentPhase is ChangeRootWriteAccess changeRootWriteAccess)
                {
                    changeRootWriteAccess.BlackScreen();
                }
                break;
        }
    }
}
