using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatAPTBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject userInputGO;

    [SerializeField]
    private TextMeshProUGUI chatAPTTextBox;

    [SerializeField]
    private float typeSpeed;

    private TMP_InputField inputField; 
    private string userInput;
    private List<Response> responsesDB = new List<Response>();
    private StreamReader sr;
    private WaitForSeconds typingSpeed;

    private void Awake()
    {
        inputField = userInputGO.GetComponent<TMP_InputField>();
    }
    private void Start()
    {
        InitializeResponses();
        typingSpeed = new WaitForSeconds(typeSpeed);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)) SubmitResponse();
    }
    public void SubmitResponse()
    {
        userInput = inputField.text; //Store the text 
        inputField.text = ""; //Clears the text 

        SelectResponse(userInput);
    }
    private void SelectResponse(string input)
    {
        string[] userInput = input.ToLower().Split(' ');

        string selectedResponse = "Sorry I don't understand that";
        int highestWeight = 0;

        foreach (Response response in responsesDB)
        {
            int responseWeight = 0;

            foreach (string word in userInput)
            {
                if (response.keywords.Contains(word))
                {
                    responseWeight++;

                    if(highestWeight < responseWeight) highestWeight = responseWeight; 
                }
            }
            if (responseWeight >= highestWeight) selectedResponse = response.response;
        }
        Debug.Log("Response weight: " + highestWeight);
        Respond(selectedResponse);
    }
    private void Respond(string response)
    {
        StartCoroutine(TypingEffect(chatAPTTextBox, response));
    }
    private void InitializeResponses()
    {
        //List<string> tempResponses = ParseCSV("ResponsesDatabase.csv");

        //foreach (string response in tempResponses)
        //{
        //    string[] values = response.Split(',');

        //    responsesDB.Add(new Response(values[0].Split('@'), values[1], values[2], false));
        //}

        responsesDB.Add(new Response(new string[] { "Hello" }, "Hi! I'm LeBron", ""));
        responsesDB.Add(new Response(new string[] { "Hi" }, "Hi! I'm LeBron", ""));
        responsesDB.Add(new Response(new string[] { "Basketball" }, "I love Kobe", ""));
        responsesDB.Add(new Response(new string[] { "Love" }, "I love Kobe", ""));
        responsesDB.Add(new Response(new string[] { "Scream", "if", "you" }, "AAAAAAAAAAAAAAA", ""));
        responsesDB.Add(new Response(new string[] { "You", "are", "my", "sunshine" }, "My only sunshine", ""));
        responsesDB.Add(new Response(new string[] { "Sunshine" }, "Huh?", ""));
    }
    private List<string> ParseCSV(string filePath)
    {
        List<string> result = new List<string>();

        sr = File.OpenText("/ChatAPT/" + filePath);

        sr.ReadLine();
        while (!sr.EndOfStream)
        {
            result.Add(sr.ReadLine());
        }

        return result;
    }
    private IEnumerator TypingEffect(TextMeshProUGUI text, string textToType)
    {
        text.text = ""; //Initializes text at the start

        foreach (char c in textToType) //Types out text with a delay between each character
        {
            text.text += c;
            yield return typingSpeed;
        }
    }
}
public class Response
{
    public string[] keywords;
    public string response;
    public string unlockCondition;
    public bool isUnlocked; 

    public Response(string[] keywords, string response, string unlockCondition)
    {
        string[] tempKeywords = new string[keywords.Length]; 
        for(int i = 0; i < tempKeywords.Length; i++)
        {
            tempKeywords[i] = keywords[i].ToLower();
        }
        this.keywords = tempKeywords;

        this.response = response;
        this.unlockCondition = unlockCondition;
        isUnlocked = false;
    }
}