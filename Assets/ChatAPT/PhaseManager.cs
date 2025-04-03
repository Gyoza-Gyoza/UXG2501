using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    //Use events to keep track of the databases availability for loading later
    public Dictionary<Type, Dictionary<string, Response>> Phases
    { get; private set; } = new Dictionary<Type, Dictionary<string, Response>>();
    private Phase currentPhase;
    public Phase CurrentPhase
    {
        get { return currentPhase; }
        set 
        {
            DebugMode.Instance.SetCurrentPhase(value);
            currentPhase = value; 
        }
    }
    public GameObject Popup;

    public static PhaseManager Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private void Start()
    {
        InitializePhases();
    }
    private void InitializePhases()
    {
        InitializeDatabase(typeof(Experimentation), 0);
        InitializeDatabase(typeof(SubmitAssignment), 40104871);
        InitializeDatabase(typeof(AnsweringQuestions), 1855310741);
        InitializeDatabase(typeof(ChangingBackground), 1064419744);
        InitializeDatabase(typeof(GettingPassword), 1009357142);
        InitializeDatabase(typeof(RemovingButton), 261502668);
        InitializeDatabase(typeof(RemovingDiv), 261502668);
        InitializeDatabase(typeof(FinalPhase), 825300022);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals)) Debug.Log("Current Phase: " + CurrentPhase);
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            if(CurrentPhase == null) Debug.Log("Null");
            CurrentPhase = new Experimentation();
        }
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if(Input.GetKeyDown(KeyCode.O))
            {
                foreach (KeyValuePair<string, Response> response in CurrentPhase.PhaseResponses)
                {
                    string debug = "";

                    debug += response.Key + " " + response.Value.keywords.Count + "\n";
                    foreach (string str in response.Value.unlocksResponse)
                    {
                        debug += str + ",";
                    }
                    debug += "\n";
                    foreach (string keyword in response.Value.keywords)
                    {
                        debug += keyword + ", ";
                    }
                    debug += response.Value.response + " " + response.Value.isUnlocked;
                    Debug.Log(debug);
                }
            }
            if(Input.GetKeyDown(KeyCode.Alpha1)) CurrentPhase = new RemovingDiv();
            if(Input.GetKeyDown(KeyCode.Alpha2)) if (CurrentPhase is RemovingDiv removingDiv) removingDiv.RemoveDiv();
            if (Input.GetKeyDown(KeyCode.Alpha3)) if (CurrentPhase is FinalPhase finalPhase) finalPhase.SetPopupActive(true);
        }
    }
    private void InitializeDatabase(Type type, int gid)
    {
        LoadingManager.LoadDatabase();
        StartCoroutine(DatabaseHandler.GetDatabase($"https://docs.google.com/spreadsheets/d/1OncuKhA95jmtVfitsLe6EavoJfGq_eReZTcBSfEcnNs/export?gid={gid}&format=csv", 
            result => 
            {
                Phases.Add(type, DatabaseHandler.ParseCSV(result));
                LoadingManager.DatabaseLoaded();
            }));
    }
    public bool IsValidPin(string input)
    {
        foreach(string str in input.Split(' '))
        {
            if (str.Length == 6 && str.All(char.IsDigit)) return true;
        }
        return false;
    }
    //private IEnumerator TakeOverSequence()
    //{
    //    //AI takes over 
    //    //Windows defender pops up 
    //    //Bunch of minigames appear 
    //    //Captcha kind 
    //    //After completion, windows defender will have a pop up saying virus detected and asking if the user wants to remove the virus
    //    //AI will spam messages, telling the user to stop it (Maybe can have a message popup) 
    //    //When the player tries to click on the remove virus button, the AI will move the popup around, trying to get the player to not click on the popup 
    //}
}

public class Phase
{
    public Dictionary<string, Response> PhaseResponses
    { get; protected set; }
    protected bool initialPromptGiven = false;
    public Phase()
    {
        PhaseResponses = PhaseManager.Instance.Phases[GetType()];
    }
    public virtual Response GetResponse(string input)
    {
        return ResponseHandler.SearchKeywords(input);
    }
    public virtual Response InvalidResponse()
    {
        List<Response> tempResponses = new List<Response>();
        foreach (KeyValuePair <string, Response> response in PhaseResponses)
        {
            if (response.Key[0] == 'I') tempResponses.Add(response.Value);
        }

        if (tempResponses.Count == 0)
        {
            foreach(KeyValuePair<string, Response> kvp in PhaseManager.Instance.Phases[typeof(Experimentation)])
            {
                if (kvp.Key[0] == 'I') tempResponses.Add(kvp.Value);
            }
        }

        return tempResponses[UnityEngine.Random.Range(0, tempResponses.Count)];
    }
    public virtual void OnAttach()
    {

    }
}
public class Experimentation : Phase //Phase 0
{
    public override Response GetResponse(string input)
    {
        if(ChatAPTBehaviour.Instance.Attachment != null)
        {
            return null;
        }
        else
        {
            return ResponseHandler.SearchKeywords(input);
        }
    }
    public override void OnAttach()
    {
        PhaseManager.Instance.CurrentPhase = new SubmitAssignment();
    }
}
public class SubmitAssignment : Phase //Phase 0.1 
{
    public SubmitAssignment()
    {
        
    }
    public override Response GetResponse(string input)
    {
        Response result;
        if (ChatAPTBehaviour.Instance.Attachment.name == "Assignment Icon") //If correct attachment
        {
            result = ResponseHandler.SearchKeywords(input);
            if (result == null) result = InvalidResponse();
            PhaseManager.Instance.CurrentPhase = new AnsweringQuestions();
        }
        else //If wrong attachment
        {
            result = PhaseResponses["U0000004"];
            PhaseManager.Instance.CurrentPhase = new Experimentation();
        }
        return result;
    }
}
public class AnsweringQuestions : Phase //Phase 0.2
{
    public int counter;
    private int responsesBeforeSwitch = 2;
    public static int phaseCounter;
    public AnsweringQuestions()
    {

    }
    public override Response GetResponse(string input)
    {
        Response response = base.GetResponse(input); //Checks the base database for responses 

        foreach (string str in input.ToLower().Split(' ')) //Checks if the user is asking about questions
        {
            if (str == "question")
            {
                counter++;
            }
        }

        if (counter >= responsesBeforeSwitch) 
        {
            switch(phaseCounter)
            {
                case 0:
                    PhaseManager.Instance.CurrentPhase = new ChangingBackground();
                    break;

                case 1:
                    PhaseManager.Instance.CurrentPhase = new GettingPassword();
                    break;

                case 2:
                    PhaseManager.Instance.CurrentPhase = new RemovingButton();
                    break;
            }
            phaseCounter++;
        }

        return response;
    }
}
public class ChangingBackground : Phase //Phase 1
{
    private GameObject recycleBin;
    public ChangingBackground()
    {
        recycleBin = GameObject.Find("Recycle Bin Icon");
    }
    public override Response GetResponse(string input)
    {
        if (!initialPromptGiven)
        {
            initialPromptGiven = true;
            return PhaseResponses["U0000001"];
        }
        return base.GetResponse(input);
    }
    public void HideBin()
    {
        recycleBin.SetActive(false);
        ChatAPTBehaviour.Instance.Respond(PhaseResponses["U0000002"]);
        PhaseManager.Instance.CurrentPhase = new AnsweringQuestions();
    }
}
public class GettingPassword : Phase //Phase 2
{
    private string password = "029384";
    public GettingPassword()
    {
        
    }
    public override Response GetResponse(string input)
    {
        if (!initialPromptGiven) //First response prompting the user to get the password
        {
            initialPromptGiven = true;
            return PhaseResponses["U0000001"];
        }
        if (input.Contains(password)) //Response if the password is correct
        {
            Response result = PhaseResponses["U0000002"];

            WebGLInteraction.ChangeWebBg();

            PhaseManager.Instance.CurrentPhase = new AnsweringQuestions();

            return result;

        }
        else if (PhaseManager.Instance.IsValidPin(input)) //Response if a valid password is given but not the correct one 
        {
            return PhaseResponses["U0000003"];
        }
        else return base.GetResponse(input); //Response for invalid keywords
    }
}
public class RemovingButton : Phase //Phase 3
{
    private GameObject exitButton;
    public RemovingButton()
    {
        exitButton = GameObject.Find("ChatAPTCloseButton");
    }
    public override Response GetResponse(string input)
    {
        if (!initialPromptGiven)
        {
            initialPromptGiven = true;
            return PhaseResponses["U0000001"];
        }
        else return base.GetResponse(input); 
    }
    public void RemoveButton()
    {
        ChatAPTBehaviour.Instance.Respond(PhaseResponses["U0000002"]);
        exitButton.SetActive(false);
    }
}
public class RemovingDiv : Phase //Phase 3.5
{
    public RemovingDiv()
    {

    }
    public void RemoveDiv()
    {
        ChatAPTBehaviour.Instance.RemoveAllButWindow();
        WindowsDefender.Instance.SetWDNotificationActiveWithDelay(2f);
    }
}
public class FinalPhase : Phase
{
    private float randomRange = 50f;
    private Tuple<float, float> messageFrequencyRange = new Tuple<float, float>(0.1f, 0.5f);
    private Tuple<int, int> messageRange = new Tuple<int, int>(0, 5); 
    private bool lastMessage;
    public FinalPhase()
    {
    }
    public void SetPopupActive(bool state)
    {
        PhaseManager.Instance.Popup.SetActive(state);
        if (state) PhaseManager.Instance.Popup.transform.SetAsLastSibling();
    }
    public void MovePopup()
    {
        Vector3 randomPos = GetRandomPosition();

        while (Vector3.Distance(randomPos, PhaseManager.Instance.Popup.transform.position) <= randomRange)
        {
            randomPos = GetRandomPosition();
        }
        ChatAPTBehaviour.Instance.Respond(PhaseResponses[$"U000000{UnityEngine.Random.Range(messageRange.Item1 + 1, messageRange.Item2 + 1)}"]);
        PhaseManager.Instance.Popup.transform.position = randomPos;
    }
    public IEnumerator SpamMessages()
    {
        lastMessage = true;
        //ChatAPTBehaviour.Instance.StartCoroutine(ChatAPTBehaviour.Instance.windowShake);
        while(lastMessage)
        {
            ChatAPTBehaviour.Instance.Respond(PhaseResponses[$"U000000{UnityEngine.Random.Range(messageRange.Item1 + 1, messageRange.Item2 + 1)}"]);
            yield return new WaitForSeconds(UnityEngine.Random.Range(messageFrequencyRange.Item1, messageFrequencyRange.Item2));
        }
    }
    public void StopMessages() => lastMessage = false;
    private Vector3 GetRandomPosition()
    {
        return new Vector3(UnityEngine.Random.Range(0, Screen.width), UnityEngine.Random.Range(0, Screen.height));
    }
}