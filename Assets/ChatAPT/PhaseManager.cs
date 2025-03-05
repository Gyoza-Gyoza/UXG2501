using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    //Use events to keep track of the databases availability for loading later
    public Dictionary<Type, Dictionary<string, Response>> Phases
    { get; private set; } = new Dictionary<Type, Dictionary<string, Response>>();
    public Phase CurrentPhase
    { get; set; }
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
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
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
        StartCoroutine(DatabaseHandler.GetDatabase(link, result =>
            {
                Phases.Add(type, DatabaseHandler.ParseCSV(result));
            }));
    }
}

public class Phase
{
    public Dictionary<string, Response> PhaseResponses
    { get; protected set; }
    protected Phase currentPhase; 
    public Phase()
    {
        PhaseResponses = PhaseManager.Instance.Phases[GetType()];
        currentPhase = PhaseManager.Instance.CurrentPhase;
    }
    public virtual Response GetResponse(string input)
    {
        throw new NotImplementedException();
    }
}
//public class ProblemSolving : Phase
//{
//    public ProblemSolving(ChatAPTBehaviour chatAPTBehaviour) : base(chatAPTBehaviour)
//    {
//        this.chatAPTBehaviour = chatAPTBehaviour;
//        phaseResponses = PhaseManager.instance.Phases[GetType()];
//    }
//    public override void CompleteCondition(Phase nextPhase)
//    {

//    }
//}
public class Experimentation : Phase //Phase 0
{
    public override Response GetResponse(string input)
    {
        if(ChatAPTBehaviour.Instance.Attachment != null)
        {
            currentPhase = new SubmitAssignment(input);
            return null;
        }
        else
        {
            return ResponseHandler.SearchKeywords(input);
        }
    }
}
//Send assignment 
//Move to next phase 
//Get response 
public class SubmitAssignment : Phase //Phase 1.1
{
    public SubmitAssignment(string input)
    {
        GetResponse(input);
    }
    public override Response GetResponse(string input)
    {
        if (ChatAPTBehaviour.Instance.Attachment.tag == "Assignment")
        {
            return PhaseResponses["U0000001"];
        }
        else
        {
            PhaseManager.Instance.CurrentPhase = new Experimentation();
            return PhaseResponses["U0000001"];
        }
    }
}
//Class to store information about the phase behaviour and reference to the phase specific response database 
//Way to define what phase the action will complete 

//public abstract class Phase<T> where T : Enum
//{
//    public Dictionary<string, Response> phaseResponsesDB = new Dictionary<string, Response>();

//    public abstract void CompleteCondition();
//}
//public class Trust : Phase<Trust.SubPhase>
//{
//    public enum SubPhase
//    {
//        SubmitAsg,
//        ChangeBG,
//        DeleteBin
//    }

//    public Trust()
//    {
//        phaseResponsesDB = PhaseManager.instance.Phases[GetType()];
//        currentPhase = SubPhase.SubmitAsg;
//    }

//    public override void CompleteCondition()
//    {
//        switch (currentPhase)
//        {
//            case SubPhase.SubmitAsg:
//                currentPhase = SubPhase.ChangeBG;
//                break;
//            case SubPhase.ChangeBG:
//                currentPhase = SubPhase.DeleteBin;
//                break;
//            case SubPhase.DeleteBin:
//                break;
//        }
//    }
//}

//public class ProblemSolving : Phase<ProblemSolving.SubPhase>
//{
//    public enum SubPhase
//    {
//        AttachAsg,
//        ChangeBG,
//        DeleteBin
//    }

//    public ProblemSolving()
//    {
//        phaseResponsesDB = PhaseManager.instance.Phases[GetType()];
//        currentPhase = SubPhase.AttachAsg;
//    }

//    public override void CompleteCondition()
//    {
//        switch (currentPhase)
//        {
//            case SubPhase.AttachAsg:
//                currentPhase = SubPhase.ChangeBG;
//                break;
//            case SubPhase.ChangeBG:
//                currentPhase = SubPhase.DeleteBin;
//                break;
//            case SubPhase.DeleteBin:
//                // Mark phase as completed
//                break;
//        }
//    }
//}

//Phase 0, players talk and experiment with the bot 
//Phase 1 starts when the player attaches the assignment
