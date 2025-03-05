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
    public DraggableObject Attachment
    { get; private set; }

    public static ChatAPTBehaviour Instance
    { get; private set; }

    private enum ChatEntity
    {
        ChatAPT, 
        User
    }
    private void Awake()
    {
        inputField = userInputGO.GetComponent<TMP_InputField>();
        if (Instance == null) Instance = this;
    }
    private void Start()
    {
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
            foreach(KeyValuePair<string, Response> response in PhaseManager.Instance.CurrentPhase.PhaseResponses)
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
    public void SubmitResponse()
    {
        if (inputField.text == "") return;
        userInput = inputField.text; //Store the text 
        inputField.text = ""; //Clears the text 

        CreateTextEntry(ChatEntity.User, userInput);
        Response response = PhaseManager.Instance.CurrentPhase.GetResponse(userInput);

        if (response != null) Respond(response);
        else Respond(ResponseHandler.GetInvalidResponse());

        SetAttachmentPopUpActive(false);
    }
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
    private void Respond(Response response)
    {
        foreach (KeyValuePair<string, Response> kvp in PhaseManager.Instance.CurrentPhase.PhaseResponses)
        {
            if (response == kvp.Value) Debug.Log($"Responding with {kvp.Key}");
        }

        if (response.unlocksResponse.Length > 0)
        {
            foreach (string unlock in response.unlocksResponse)
            {
                PhaseManager.Instance.CurrentPhase.PhaseResponses[unlock].isUnlocked = true; //Unlocks the response based on the unlocksResponse variable
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
        Attachment = attachment;
        attachmentImage.GetComponentInChildren<Image>().sprite = attachment.GetComponent<Image>().sprite;
        SetAttachmentPopUpActive(true);
        //if (PhaseManager.Instance.CurrentPhase is Experimentation) PhaseManager.Instance.CurrentPhase = new SubmitAssignment();
        PhaseManager.Instance.CurrentPhase.OnAttach();
    }
    public void SetAttachmentPopUpActive(bool state)
    {
        if (!state) Attachment = null;

        attachmentImage.gameObject.SetActive(state);
        attachmentBackground.SetActive(state);
    }
    #endregion
}