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
        InitializeDatabase(typeof(AssignmentSolving), 1855310741);
        InitializeDatabase(typeof(BuildingTrust), 1064419744);
        InitializeDatabase(typeof(ProblemSolving), 1009357142);
        InitializeDatabase(typeof(EscalationIntimidation), 261502668);
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
public abstract class TakingOver : Phase
{
    public int counter; 
    public bool CheckResponse(string input)
    {
        foreach (string str in input.ToLower().Split(' '))
        {
            if (str == "question")
            {
                Debug.Log("counted");
                return true;
            }
        }
        return false;
    }
}
public class AssignmentSolving : TakingOver //Phase 0.2
{
    private int responsesBeforeSwitch = 2;
    public static int phaseCounter;
    public AssignmentSolving()
    {
        
    }
    public override Response GetResponse(string input)
    {
        Response response = base.GetResponse(input);

        if (CheckResponse(input)) counter++;

        if (counter >= responsesBeforeSwitch)
        {
            switch(phaseCounter)
            {
                case 0:
                    PhaseManager.Instance.CurrentPhase = new BuildingTrust();
                    break;

                case 1:
                    PhaseManager.Instance.CurrentPhase = new ProblemSolving();
                    break;

                case 2:
                    PhaseManager.Instance.CurrentPhase = new EscalationIntimidation();
                    break;
            }
            phaseCounter++;
        }

        return response;
    }
}
public class BuildingTrust : TakingOver //Phase 1
{
    private GameObject recycleBin; 
    public BuildingTrust()
    {
        recycleBin = GameObject.Find("Recycle Bin Icon");
    }
    public void HideBin()
    {
        recycleBin.SetActive(false);
        PhaseManager.Instance.CurrentPhase = new AssignmentSolving();
    }
}
public class ProblemSolving : TakingOver //Phase 2
{
    private string password = "029384";
    public ProblemSolving()
    {
        
    }
    public override Response GetResponse(string input)
    {
        if (input.Contains(password))
        {
            Response result = PhaseResponses["U0000001"];

            PhaseManager.Instance.CurrentPhase = new AssignmentSolving();

            return result;

        }
        else return base.GetResponse(input);
    }
}
public class EscalationIntimidation : Phase //Phase 3
{
    public EscalationIntimidation()
    {

    }
}