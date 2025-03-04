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
    { get; private set; }
    public static PhaseManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    private void Start()
    {
        InitializePhases();
    }
    private void InitializePhases()
    {
        StartCoroutine(ResponseDatabase.GetDatabase("https://docs.google.com/spreadsheets/d/1OncuKhA95jmtVfitsLe6EavoJfGq_eReZTcBSfEcnNs/export?gid=40104871&format=csv", 
            result =>
            {
                Phases.Add(typeof(Trust), ResponseDatabase.ParseCSV(result));
                CurrentPhase = new Trust(this);
            }));
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log(CurrentPhase.phaseResponses.Count);
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.O))
        {
            foreach (KeyValuePair<string, Response> response in CurrentPhase.phaseResponses)
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
}

public abstract class Phase
{
    public Dictionary<string, Response> phaseResponses
    { get; protected set; }
    protected PhaseManager phaseManager;
    public enum SubPhases
    {
        SubmitAsg, 
        ChangeBG, 
        DeleteBin
    }
    public Phase(PhaseManager phaseManager)
    {
        this.phaseManager = phaseManager;
    }
    public abstract void CompleteCondition(SubPhases state);
}
public class Trust : Phase
{
    public Trust(PhaseManager phaseManager) : base(phaseManager)
    {
        this.phaseManager = phaseManager;
        phaseResponses = phaseManager.Phases[GetType()];
    }
    public override void CompleteCondition(SubPhases state)
    {
        switch(state)
        {
            case SubPhases.SubmitAsg:
                break;
            case SubPhases.ChangeBG:
                break;
            case SubPhases.DeleteBin:
                break;
        }
    }
}
public class ProblemSolving : Phase
{
    public ProblemSolving(PhaseManager phaseManager) : base(phaseManager)
    {
        this.phaseManager = phaseManager; 
        phaseResponses = phaseManager.Phases[GetType()];
    }
    public override void CompleteCondition(SubPhases state)
    {

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

//Check for subphase state 
//If its in that state and the action is performed, complete the condition and move to the next phase 
//Don't check all the time so that the conditions can't be completed prematurely 
//Can events be used? 