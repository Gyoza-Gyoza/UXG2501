using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChatAPTBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject userInputGO;

    [SerializeField]
    private TextMeshProUGUI chatAPTTextBox;

    [SerializeField]
    private float typeSpeed;

    [SerializeField]
    private Response[] invalidInputResponse;

    private TMP_InputField inputField; 
    private string userInput;
    private Dictionary<string, Response> responsesDB = new Dictionary<string, Response>();
    private StreamReader sr;
    private WaitForSeconds typingSpeed;
    private string csvData;

    private void Awake()
    {
        inputField = userInputGO.GetComponent<TMP_InputField>();
    }
    private void Start()
    {
        StartCoroutine(DownloadCSV());
        //InitializeResponses();
        typingSpeed = new WaitForSeconds(typeSpeed);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)) SubmitResponse();
        if(Input.GetKeyDown(KeyCode.P))
        {
            foreach(KeyValuePair<string, Response> response in responsesDB)
            {
                string debug = "";

                debug += response.Key + " " + response.Value.keywords.Count + " " + response.Value.unlocksResponse + "\n";
                debug += response.Value.response + " " + response.Value.isUnlocked;
                Debug.Log(debug);
            }
        }
        if (Input.GetKeyDown(KeyCode.O)) Debug.Log(csvData);
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

        Response selectedResponse = invalidInputResponse[Random.Range(0, invalidInputResponse.Length)];
        int highestWeight = 0;

        foreach (KeyValuePair<string, Response> response in responsesDB)
        {
            int responseWeight = 0; //Initialize the weight
            if (response.Value.isUnlocked == false) continue; //Skip response if its not unlocked

            foreach (string word in userInput)
            {
                if (response.Value.keywords.Contains(word))
                {
                    responseWeight++;

                    if(highestWeight < responseWeight) highestWeight = responseWeight; 
                }
            }
            if (responseWeight == 0) continue;
            if (responseWeight >= highestWeight) selectedResponse = response.Value;
        }
        Respond(selectedResponse);
    }
    private void Respond(Response response)
    {
        if (response.unlocksResponse != "") responsesDB[response.unlocksResponse].isUnlocked = true; //Unlocks the response based on the unlocksResponse variable
        StartCoroutine(TypingEffect(chatAPTTextBox, response.response));
    }
    private void InitializeResponses()
    {
        List<string> tempResponses = ParseCSV("ResponsesDatabase.csv");

        foreach (string response in tempResponses)
        {
            string[] values = response.Split(',');

            responsesDB.Add(values[0], new Response(values[1].Split(' '), values[2], values[3], values[0][0] == 'U'));
        }
    }
    private List<string> ParseCSV(string filePath)
    {
        List<string> result = new List<string>();

        sr = File.OpenText("Assets/ChatAPT/" + filePath);

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
    public IEnumerator DownloadCSV()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/1OncuKhA95jmtVfitsLe6EavoJfGq_eReZTcBSfEcnNs/export?format=csv");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            csvData = webRequest.downloadHandler.text;
            Debug.Log("CSV Downloaded Successfully!");

            string[] data = csvData.Split("\r\n"); 

            for(int i = 1; i < data.Length; i++)
            {
                string[] values = data[i].Split(',');

                responsesDB.Add(values[0], new Response(values[1].Split(' '), values[2], values[3], values[0][0] == 'U'));
            }
        }
        else
        {
            Debug.LogError($"Failed to download CSV: {webRequest.error}");
        }
    }
}
[System.Serializable]
public class Response
{
    public HashSet<string> keywords = new HashSet<string>();
    public string response;
    public string unlocksResponse;
    public bool isUnlocked; 

    public Response(string[] keywords, string response, string unlocksResponse, bool isUnlocked)
    {
        string[] tempKeywords = new string[keywords.Length]; 
        for(int i = 0; i < tempKeywords.Length; i++)
        {
            this.keywords.Add(keywords[i].ToLower());
        }

        this.response = response;
        this.unlocksResponse = unlocksResponse;
        this.isUnlocked = isUnlocked;
    }
}