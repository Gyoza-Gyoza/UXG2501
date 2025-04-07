using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugMode : MonoBehaviour
{
    [SerializeField]
    public GameObject debugMode;
    [SerializeField]
    private TextMeshProUGUI currentPhaseTMP, responseGivenTMP;

    public static DebugMode Instance
    { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private void Update()
    {
        if ((Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q)) || (Input.GetKey(KeyCode.RightControl) && Input.GetKeyDown(KeyCode.Q)))
        {
            debugMode.SetActive(!debugMode.activeInHierarchy);
            debugMode.transform.SetAsLastSibling();
        }
    }
    public void SetCurrentPhase(Phase phase)
    {
        currentPhaseTMP.text = "Current Phase: " + phase.GetType().Name;
    }
    public void SetResponse(Response response)
    {
        foreach(KeyValuePair<string, Response> kvp in PhaseManager.Instance.CurrentPhase.PhaseResponses)
        {
            if (kvp.Value == response) responseGivenTMP.text = "Response Given: " + kvp.Key;
        }
    }
}
