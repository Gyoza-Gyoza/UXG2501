using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class DatabaseHandler
{
    private static string csvData;

    public static IEnumerator GetDatabase(string link, Action<string> callback)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(link);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            callback?.Invoke(webRequest.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"Failed to download CSV: {webRequest.error}");
        }
    }
    public static Dictionary<string, Response> ParseCSV(string csv)
    {
        string[] data = csv.Split("\r\n");
        Dictionary<string, Response> result = new Dictionary<string, Response >();

        for (int i = 1; i < data.Length; i++)
        {
            string[] values = data[i].Split(',');

            result.Add(values[0], new Response(values[1].Split(' '), values[2].Replace('%', ','), values[3], values[0][0] == 'U'));
        }
        return result;
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