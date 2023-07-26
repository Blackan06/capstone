using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EventTaskAPI : MonoBehaviour
{
    private static EventTaskAPI instance;
    public static EventTaskAPI GetInstance()
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
    public IEnumerator GetEventTaskByTaskId(string taskId, Action<string> callback)
    {
        string url = $"http://anhkiet-001-site1.htempurl.com/api/EventTasks/eventtask/{taskId}";
        Debug.Log(url);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                // Parse JSON response to extract "data" array
                EventTaskDataWrapper wrapper = JsonUtility.FromJson<EventTaskDataWrapper>(response);
                Debug.Log(wrapper.data.eventId);
                // Access the properties of majorData
                string eventID = wrapper.data.eventId;

                // Call the callback function with the majorName
                callback?.Invoke(eventID);
            }
            else
            {
                Debug.LogError("API call failed. Error: " + webRequest.error);
            }
        }
    }
}
