using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleBin : MonoBehaviour
{
    public void ReceiveMessage(string message)
    {
        Debug.Log($"[UNITY] ✅ Received message from browser: {message}");

        if (message == "HideRecycleBin")
        {
            gameObject.SetActive(false);
        }
    }
}
