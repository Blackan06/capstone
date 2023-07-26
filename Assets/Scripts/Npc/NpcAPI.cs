using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NpcAPI : MonoBehaviour
{
    private static NpcAPI instance;
    public static NpcAPI GetInstance()
    {
        return instance;
    }
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // Thi?t l?p instance cho singleton
        instance = this;

        // ??m b?o r?ng GameObject ch?a MajorAPI không b? h?y khi chuy?n scene
        DontDestroyOnLoad(gameObject);
    }
    public IEnumerator GetNpcName(string npcId, Action<string> callback)
    {
        string url = $"http://anhkiet-001-site1.htempurl.com/api/Npcs/{npcId}";
        Debug.Log(url);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                // Parse JSON response to extract "data" array
                NpcDataWrapper wrapper = JsonUtility.FromJson<NpcDataWrapper>(response);
                Debug.Log(wrapper.data.npcName);
                // Access the properties of majorData
                string npcName = wrapper.data.npcName;
                Debug.Log("NPC Name: " + npcName);

                // Call the callback function with the majorName
                callback?.Invoke(npcName);
            }
            else
            {
                Debug.LogError("API call failed. Error: " + webRequest.error);
            }
        }
    }


}


