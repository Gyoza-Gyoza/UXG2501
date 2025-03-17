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
        InitializeDatabase(typeof(Experimentation), "https://docs.google.com/spreadsheets/d/1OncuKhA95jmtVfitsLe6EavoJfGq_eReZTcBSfEcnNs/export?gid=0&format=csv");
        InitializeDatabase(typeof(SubmitAssignment), "https://docs.google.com/spreadsheets/d/1OncuKhA95jmtVfitsLe6EavoJfGq_eReZTcBSfEcnNs/export?gid=40104871&format=csv");
        InitializeDatabase(typeof(AssignmentSolving), "https://docs.google.com/spreadsheets/d/1OncuKhA95jmtVfitsLe6EavoJfGq_eReZTcBSfEcnNs/export?gid=1855310741&format=csv");
        InitializeDatabase(typeof(BuildingTrust), "https://docs.google.com/spreadsheets/d/1OncuKhA95jmtVfitsLe6EavoJfGq_eReZTcBSfEcnNs/export?gid=1064419744&format=csv");
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
    private void InitializeDatabase(Type type, string link)
    {
        LoadingManager.LoadDatabase();
        StartCoroutine(DatabaseHandler.GetDatabase(link, result =>
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
        if (ChatAPTBehaviour.Instance.Attachment.name == "Assignment Icon")
        {
            result = ResponseHandler.SearchKeywords(input);
            PhaseManager.Instance.CurrentPhase = new AssignmentSolving();
        }
        else
        {
            result = PhaseResponses["U0000004"];
            PhaseManager.Instance.CurrentPhase = new Experimentation();
        }
        return result;
    }
}
public class AssignmentSolving : Phase //Phase 0.2
{
    private int counter, responsesBeforeSwitch = 3;
    public AssignmentSolving()
    {
        
    }
    public override Response GetResponse(string input)
    {
        counter++;
        Response response = base.GetResponse(input);

        if(counter >= responsesBeforeSwitch) PhaseManager.Instance.CurrentPhase = new BuildingTrust();

        return response;
    }
}
public class BuildingTrust : Phase
{
    private GameObject recycleBin; 
    public BuildingTrust()
    {
        recycleBin = GameObject.FindGameObjectWithTag("RecycleBin");
    }
    public void HideBin()
    {
        recycleBin.SetActive(false);
    }
}