using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResponseHandler
{
    private static Response[] invalidInputResponses = new Response[3]
    {
        new Response(new string[0], "Sorry I don't understand that", "", true),
        new Response(new string[0], "Can you try something else", "", true),
        new Response(new string[0], "I understand basketball but not that", "", true)
    };
    public static Response SearchKeywords(string input) //Response process 
    {
        string[] userInput = input.ToLower().Split(' ');

        Response selectedResponse;
        int highestWeight = 0;

        //Check for exact responses
        if (CheckExact(input, out selectedResponse)) return selectedResponse;

        //If no exact responses, look for a most matched response
        foreach (KeyValuePair<string, Response> response in PhaseManager.Instance.CurrentPhase.PhaseResponses)
        {
            int responseWeight = 0; //Initialize the weight
            if (!response.Value.isUnlocked) continue; //Skip response if its not unlocked

            foreach (string word in userInput)
            {
                if (response.Value.keywords.Contains(word))
                {
                    responseWeight++;

                    if (highestWeight < responseWeight) highestWeight = responseWeight;
                }
            }
            if (responseWeight == 0) continue;
            if (responseWeight >= highestWeight) selectedResponse = response.Value;
        }

        //If there are no matches, respond with a random invalid response
        if (selectedResponse == null) selectedResponse = invalidInputResponses[Random.Range(0, invalidInputResponses.Length)];

        //Respond with the selected response
        return selectedResponse;
    }
    private static bool CheckExact(string input, out Response foundResponse)
    {
        string tempInput = input.ToLower().Replace(' ', '@');

        foreach (KeyValuePair<string, Response> response in PhaseManager.Instance.CurrentPhase.PhaseResponses)
        {
            if (response.Value.keywords.Contains(tempInput))
            {
                if (!response.Value.isUnlocked) continue; //Skip response if its not unlocked
                foundResponse = response.Value;
                return true;
            }
        }
        foundResponse = null;
        return false;
    }
}
