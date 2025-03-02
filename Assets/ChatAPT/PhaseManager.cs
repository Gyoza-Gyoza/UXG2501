using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class Phase
{
    public Dictionary<string, Response> phaseResponsesDB = new Dictionary<string, Response>();
    public bool completed; 
}
