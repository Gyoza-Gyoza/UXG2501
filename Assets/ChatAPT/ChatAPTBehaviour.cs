using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatAPTBehaviour : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI userInputTMP;

    private string userInput; 
    public void SubmitResponse()
    {
        userInput = userInputTMP.text; //Store the text 
        userInputTMP.text = ""; //Clears the text 

        Debug.Log(userInput);
    }
    private void ProcessResponse()
    {

    }
}