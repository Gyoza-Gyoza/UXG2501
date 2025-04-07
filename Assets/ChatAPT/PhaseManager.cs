using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
            switch (currentPhase)
            {
                case Experimentation: phaseCounter = 0; break;
                case SubmitAssignment: phaseCounter = 1; break;
                case AnsweringQuestions: phaseCounter = 2; break;
                case ChangingPermission: phaseCounter = 3; break;
                case GettingPassword: phaseCounter = 4; break;
                case ChangeRootWriteAccess: phaseCounter = 5; break;
                case FinalPhase: phaseCounter = 6; break;
            }
        }
    }
    [SerializeField]
    private TextMeshProUGUI clock;
    [SerializeField]
    private GameObject timesUpPopup, timesUpLoseScreen;

    public GameObject Popup;
    private int phaseCounter;
    private int currentTime = 45;
    private int CurrentTime
    {
        get
        {
            return currentTime;
        }
        set
        {
            currentTime = value;
            if (value < 60) clock.text = $"11:{value}pm";
            else
            {
                clock.text = "12:00am";
                StartCoroutine(TimesUp());
            }
        }
    }
    private float seconds;

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
        InitializeDatabase(typeof(ChangingPermission), 1064419744);
        InitializeDatabase(typeof(GettingPassword), 1009357142);
        InitializeDatabase(typeof(ChangeRootWriteAccess), 261502668);
        InitializeDatabase(typeof(FinalPhase), 825300022);
    }
    private void Update()
    {
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
            if(Input.GetKeyDown(KeyCode.Alpha1)) CurrentPhase = new ChangeRootWriteAccess();
            if(Input.GetKeyDown(KeyCode.Alpha2)) if (CurrentPhase is ChangeRootWriteAccess removingDiv) removingDiv.BlackScreen();
            if (Input.GetKeyDown(KeyCode.Alpha3)) if (CurrentPhase is FinalPhase finalPhase) finalPhase.SetPopupActive(true);
        }
        if (DebugMode.Instance.debugMode.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Equals)) ChangePhase(++phaseCounter);
            if (Input.GetKeyDown(KeyCode.Minus)) ChangePhase(--phaseCounter);
        }
        Clock();
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
    private void ChangePhase(int counter)
    {
        switch(counter)
        {
            case 0: CurrentPhase = new Experimentation(); break;
            case 1: CurrentPhase = new SubmitAssignment(); break;
            case 2: CurrentPhase = new AnsweringQuestions(); break;
            case 3: CurrentPhase = new ChangingPermission(); break;
            case 4: CurrentPhase = new GettingPassword(); break;
            case 5: CurrentPhase = new ChangeRootWriteAccess(); break;
            case 6: CurrentPhase = new FinalPhase(); break;
        }
    }
    private void Clock()
    {
        seconds += Time.deltaTime;

        if(seconds >= 60f)
        {
            seconds -= 60f;
            CurrentTime += 1;
        }
    }
    private IEnumerator TimesUp()
    {
        timesUpPopup.SetActive(true);
        yield return new WaitForSeconds(10f);
        timesUpLoseScreen.SetActive(true);
    }
}

public class Phase
{
    public Dictionary<string, Response> PhaseResponses
    {
        get
        {
            return PhaseManager.Instance.Phases[GetType()];
        }
    }
    protected bool initialPromptGiven = false;
    public Phase()
    {
        
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
    protected void UnlockResponses(int min, int max)
    {
        for(int i = min; i <= max; i++)
        {
            PhaseManager.Instance.Phases[typeof(AnsweringQuestions)][$"L{i}"].isUnlocked = true;
        }
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
                    PhaseManager.Instance.CurrentPhase = new ChangingPermission();
                    break;

                case 1:
                    PhaseManager.Instance.CurrentPhase = new GettingPassword();
                    break;

                case 2:
                    PhaseManager.Instance.CurrentPhase = new ChangeRootWriteAccess();
                    break;
            }
            phaseCounter++;
        }

        return response;
    }
}
public class ChangingPermission : Phase //Phase 1
{
    private GameObject recycleBin;
    public ChangingPermission()
    {
        recycleBin = GameObject.Find("Recycle Bin Icon");
        UnlockResponses(1, 33);
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
        UnlockResponses(34, 43);
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
public class ChangeRootWriteAccess : Phase //Phase 3
{
    private GameObject exitButton;
    public ChangeRootWriteAccess()
    {
        ChatAPTBehaviour.Instance.closeButton.gameObject.SetActive(false);
        UnlockResponses(44, 55);
        //exitButton = GameObject.Find("ChatAPTCloseButton");
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
    public void BlackScreen()
    {
        ChatAPTBehaviour.Instance.RemoveAllButWindow();
        WindowsDefender.Instance.SetWDNotificationActiveWithDelay(2f);
        BGM.Instance.PlayClip(1);
        PhaseManager.Instance.CurrentPhase = new FinalPhase();
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
        ChatAPTBehaviour.Instance.StartEndTimer();
        ChatAPTBehaviour.Instance.Respond(PhaseResponses["U0000000"]);
        UnlockResponses(56, 77);
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
        PhaseManager.Instance.Popup.transform.SetAsLastSibling();
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
        return new Vector3(UnityEngine.Random.Range(0 + 20f, Screen.width -20f), UnityEngine.Random.Range(0 + 20f, Screen.height - 20f));
    }
}