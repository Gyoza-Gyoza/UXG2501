using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public static class ResponseDatabase
{
    public static Dictionary<string, Response> ResponsesDB
    { get; private set; } = new Dictionary<string, Response>();

    private static StreamReader sr;
    private static string csvData;

    public static IEnumerator InitializeDatabases()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/1OncuKhA95jmtVfitsLe6EavoJfGq_eReZTcBSfEcnNs/export?format=csv");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            //On CSV downloaded
            csvData = webRequest.downloadHandler.text;
            Debug.Log("CSV Downloaded Successfully!");

            string[] data = csvData.Split("\r\n");

            for (int i = 1; i < data.Length; i++)
            {
                string[] values = data[i].Split(',');

                ResponsesDB.Add(values[0], new Response(values[1].Split(' '), values[2].Replace('#', ','), values[3], values[0][0] == 'U'));
            }
        }
        else
        {
            Debug.LogError($"Failed to download CSV: {webRequest.error}");
        }
    }
}

[System.Serializable]
public class Response
{
    public HashSet<string> keywords = new HashSet<string>();
    public string response;
    public string[] unlocksResponse;
    public bool isUnlocked;

    public Response(string[] keywords, string response, string unlocksResponse, bool isUnlocked)
    {
        for (int i = 0; i < keywords.Length; i++)
        {
            this.keywords.Add(keywords[i].ToLower());
        }

        this.response = response;
        if (!string.IsNullOrEmpty(unlocksResponse)) this.unlocksResponse = unlocksResponse.Split('@');
        else this.unlocksResponse = new string[0];
        this.isUnlocked = isUnlocked;
    }
}