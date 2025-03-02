using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
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

    [SerializeField]
    private GameObject chatAPTTextPrefab, userTextPrefab;

    [SerializeField]
    private Transform chatContent;

    [SerializeField]
    private ScrollRect scrollRect;

    [SerializeField]
    private GameObject textArea;

    [Header("Attachment Variables")]
    [SerializeField]
    private GameObject attachmentArea;
    [SerializeField]
    private GameObject attachmentImage, attachmentBackground;
    private bool inAttachmentArea; 
    public bool InAttachmentArea
    { get { return inAttachmentArea; } set { inAttachmentArea = value; } }

    private TMP_InputField inputField;
    private string userInput;
    private WaitForSeconds typingSpeed;
    private DraggableObject attachment;

    public static ChatAPTBehaviour instance;

    private enum ChatEntity
    {
        ChatAPT, 
        User
    }
    private void Awake()
    {
        inputField = userInputGO.GetComponent<TMP_InputField>();
        if (instance == null) instance = this;
    }
    private void Start()
    {
        StartCoroutine(ResponseDatabase.InitializeDatabases());
        typingSpeed = new WaitForSeconds(typeSpeed);
        AttachmentModeActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SubmitResponse();
            inputField.ActivateInputField();
        }
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.P))
        {
            foreach(KeyValuePair<string, Response> response in ResponseDatabase.ResponsesDB)
            {
                string debug = "";

                debug += response.Key + " " + response.Value.keywords.Count + "\n";
                foreach (string str in response.Value.unlocksResponse)
                {
                    debug += str + ",";
                }
                debug += "\n";
                foreach(string keyword in response.Value.keywords)
                {
                    debug += keyword + ", ";
                }
                debug += response.Value.response + " " + response.Value.isUnlocked;
                Debug.Log(debug);
            }
        }
        //if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.I)) WebGLInteraction.TriggerRefresh();
    }
    #region Response System
    private void CreateTextEntry(ChatEntity texter, string text)
    {
        GameObject textPrefab = null;
        switch (texter)
        {
            case ChatEntity.ChatAPT:
                textPrefab = Instantiate(chatAPTTextPrefab, chatContent);
                StartCoroutine(TypingEffect(textPrefab.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>(), text));
                break;

            case ChatEntity.User:
                textPrefab = Instantiate(userTextPrefab, chatContent);
                textPrefab.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = text;
                break;
        }
    }
    public void SubmitResponse()
    {
        if (inputField.text == "") return;
        userInput = inputField.text; //Store the text 
        inputField.text = ""; //Clears the text 

        CreateTextEntry(ChatEntity.User, userInput);
        SelectResponse(userInput);
        SetAttachmentPopUpActive(false);
    }
    private void SelectResponse(string input)
    {
        string[] userInput = input.ToLower().Split(' ');

        Response selectedResponse;
        int highestWeight = 0;

        //Check for exact responses first 
        if (CheckExact(input, out selectedResponse))
        {
            Respond(selectedResponse);
            return;
        }

        //If no exact responses, look for a most matched response
        foreach (KeyValuePair<string, Response> response in ResponseDatabase.ResponsesDB)
        {
            int responseWeight = 0; //Initialize the weight
            if (!response.Value.isUnlocked) continue; //Skip response if its not unlocked

            foreach (string word in userInput)
            {
                if (response.Value.keywords.Contains(word))
                {
                    responseWeight++;

                    if (highestWeight < responseWeight) highestWeight = responseWeight;
                }
            }
            if (responseWeight == 0) continue;
            if (responseWeight >= highestWeight) selectedResponse = response.Value;
        }

        //If there are no matches, respond with a random invalid response
        if (selectedResponse == null) selectedResponse = invalidInputResponse[Random.Range(0, invalidInputResponse.Length)];

        //Respond with the selected response
        Respond(selectedResponse);
    }
    private bool CheckExact(string input, out Response foundResponse)
    {
        string tempInput = input.ToLower().Replace(' ', '@');

        foreach(KeyValuePair <string, Response> response in ResponseDatabase.ResponsesDB)
        {
            if (response.Value.keywords.Contains(tempInput))
            {
                if (!response.Value.isUnlocked) continue; //Skip response if its not unlocked
                foundResponse = response.Value;
                return true;
            }
        }
        foundResponse = null;
        return false;
    }
    private void Respond(Response response)
    {
        foreach (KeyValuePair<string, Response> kvp in ResponseDatabase.ResponsesDB)
        {
            if(response == kvp.Value) Debug.Log($"Responding with {kvp.Key}");
        }

        if (response.unlocksResponse.Length > 0)
        {
            foreach (string unlock in response.unlocksResponse)
            {
                ResponseDatabase.ResponsesDB[unlock].isUnlocked = true; //Unlocks the response based on the unlocksResponse variable
            }
        }
        CreateTextEntry(ChatEntity.ChatAPT, response.response);
    }
    private IEnumerator TypingEffect(TextMeshProUGUI text, string textToType)
    {
        text.text = ""; //Initializes text at the start

        foreach (char c in textToType) //Types out text with a delay between each character
        {
            text.text += c;
            yield return typingSpeed;
            scrollRect.verticalNormalizedPosition = 0;
        }
    }
    #endregion
    #region Attachment System
    public void AttachmentModeActive(bool state)
    {
        attachmentArea.SetActive(state);
        textArea.SetActive(!state);
    }
    public void AttachObject(DraggableObject attachment)
    {
        this.attachment = attachment;
        attachmentImage.GetComponentInChildren<Image>().sprite = attachment.GetComponent<Image>().sprite;
        SetAttachmentPopUpActive(true);
    }
    public void SetAttachmentPopUpActive(bool state)
    {
        attachmentImage.gameObject.SetActive(state);
        attachmentBackground.SetActive(state);
    }
    #endregion
}