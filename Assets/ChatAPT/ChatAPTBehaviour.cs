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
    private List<Response> currentResponses = new List<Response>();
    public DraggableObject Attachment
    { get; private set; }

    public static ChatAPTBehaviour Instance
    { get; private set; }

    private enum ChatEntity
    {
        ChatAPT, 
        User
    }

    //Hide Window (Dylan)
    public GameObject chatScreen;
    public Button chatIcon;
    public Button closeButton;
    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.3f; // Adjust based on preference

    private void Awake()
    {
        inputField = userInputGO.GetComponent<TMP_InputField>();
        if (Instance == null) Instance = this;
    }
    private void Start()
    {
        typingSpeed = new WaitForSeconds(typeSpeed);
        AttachmentModeActive(false);

        chatScreen.SetActive(false);
        chatIcon.onClick.AddListener(OnIconClick);
        closeButton.onClick.AddListener(CloseWindow);
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

    #region Close Window

    public void OnIconClick()
    {

        float timeSinceLastClick = Time.time - lastClickTime;
        lastClickTime = Time.time;

        if (timeSinceLastClick <= doubleClickThreshold)
        {
            OpenWindow();
        }
    }

    public void CloseWindow()
    {
        chatScreen.SetActive(false);
    }

    public void OpenWindow()
    {
        chatScreen.SetActive(true);
    }
    
    #endregion Close Window

    #region Response System
    public void SubmitResponse() //Called upon submitting an input, contains the behaviour that takes in the user's input and returns a response
    {
        if (inputField.text == "") return;
        userInput = inputField.text; //Store the text 
        inputField.text = ""; //Clears the text 

        CreateTextEntry(ChatEntity.User, userInput);
        Response response = PhaseManager.Instance.CurrentPhase.GetResponse(userInput);

        if (response != null) Respond(response);
        else Respond(PhaseManager.Instance.CurrentPhase.InvalidResponse());

        SetAttachmentPopUpActive(false);
    }
    public void Respond(Response response)
    {
        DebugMode.Instance.SetResponse(response);

        if (response.unlocksResponse.Length > 0)
        {
            foreach (string unlock in response.unlocksResponse)
            {
                PhaseManager.Instance.CurrentPhase.PhaseResponses[unlock].isUnlocked = true; //Unlocks the response based on the unlocksResponse variable
            }
        }
        CreateTextEntry(ChatEntity.ChatAPT, response.response);
    }
    private void CreateTextEntry(ChatEntity speaker, string text)
    {
        GameObject textPrefab = null;
        switch (speaker)
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
        if (!attachment.canAttach) return; 

        Attachment = attachment;
        attachmentImage.GetComponentInChildren<Image>().sprite = attachment.GetComponent<Image>().sprite;
        SetAttachmentPopUpActive(true);
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