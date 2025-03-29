using System;
using System.Collections;
using System.Collections.Generic;
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
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals)) Debug.Log("Current Phase: " + CurrentPhase);
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            if(CurrentPhase == null) Debug.Log("Null");
            CurrentPhase = new Experimentation();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.O))
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
}

public class Phase
{
    public Dictionary<string, Response> PhaseResponses
    { get; protected set; }
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
            Debug.LogError($"No invalid responses found in {GetType()}!");
            return null; 
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
    public void HideBin()
    {
        recycleBin.SetActive(false);
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
        if (input.Contains(password))
        {
            Response result = PhaseResponses["U0000001"];

            PhaseManager.Instance.CurrentPhase = new AnsweringQuestions();

            return result;

        }
        else return base.GetResponse(input);
    }
}
public class RemovingButton : Phase //Phase 3
{
    private GameObject button;
    public RemovingButton()
    {
        button = GameObject.Find("ButtonToRemove");
    }
    public void RemoveButton()
    {
        ChatAPTBehaviour.Instance.Respond(PhaseResponses["U0000001"]);
    }
}
public class RemoveDiv : Phase //Phase 3.5
{
    public RemoveDiv()
    {

    }
}