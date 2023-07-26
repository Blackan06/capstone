using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class MajorAPI : MonoBehaviour
{

    private static MajorAPI instance;
    public static MajorAPI GetInstance()
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

    public IEnumerator GetMajorName(string majorId, Action<string> callback)
    {
        string url = $"http://anhkiet-001-site1.htempurl.com/api/Major/{majorId}";
        Debug.Log(url);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                // Parse JSON response to extract "data" array
                MajorDataWrapper wrapper = JsonUtility.FromJson<MajorDataWrapper>(response);
                Debug.Log(wrapper.data.name);
                // Access the properties of majorData
                string majorName = wrapper.data.name;
                Debug.Log("Major Name: " + majorName);

                // Call the callback function with the majorName
                callback?.Invoke(majorName);
            }
            else
            {
                Debug.LogError("API call failed. Error: " + webRequest.error);
            }
        }
    }

   
}
