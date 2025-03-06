//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class DoubleClickBehaviour : MonoBehaviour
//{
//    [SerializeField]
//    private GameObject targetWindow;
//    [SerializeField]
//    private Button closeButton;

//    private Button button;

//    //Double Click 
//    private float lastClickTime = 0f;
//    private float doubleClickThreshold = 0.3f; // Adjust based on preference

//    private void Awake()
//    {
//        button = GetComponent<Button>();
//    }
//    private void Start()
//    {
//        targetWindow.SetActive(false);
//        button.onClick.AddListener(OnIconClick);
//        closeButton.onClick.AddListener(CloseMoodle);
//    }
//    public void OnIconClick()
//    {
//        float timeSinceLastClick = Time.time - lastClickTime;
//        lastClickTime = Time.time;

//        if (timeSinceLastClick <= doubleClickThreshold)
//        {
//            Debug.Log("Double Clicked");
//            targetWindow.SetActive(true);
//        }
//        else
//        {
//            Debug.Log("Single Clicked");
//        }
//    }
//    public void CloseMoodle()
//    {
//        targetWindow.SetActive(false);
//    }
//}
