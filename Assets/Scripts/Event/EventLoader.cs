using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EventLoader : MonoBehaviour
{
    private const string apiUrl = "http://anhkiet-001-site1.htempurl.com/api/Events/events/time";
    private DataModel dataModel; // L?u tr? d? li?u nh?n ???c t? API
    private static EventLoader instance;
    private bool isDataLoaded = false;
  

    // T?o Singleton cho EventLoader
    public static EventLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EventLoader>();
            }
            return instance;
        }
    }
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
        StartCoroutine(LoadEventData());
       
    }

    IEnumerator LoadEventData()
    {
        while (true) // S? d?ng vòng l?p vô h?n ?? ki?m tra s? ki?n liên t?c
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error while fetching data from API: " + webRequest.error);
                }
                else
                {
                    string responseText = webRequest.downloadHandler.text;
                    dataModel = JsonUtility.FromJson<DataModel>(responseText);
                    Debug.Log("data model: " + dataModel.data.Length);
                    GetAllTaskData();
                    GetFirstStartTime();
                    isDataLoaded = true;
                  
                }
            }
            yield return new WaitForSeconds(10f); 
        }
    }
    public TaskDto[] GetAllTaskData()
    {

        if (dataModel != null && dataModel.data != null &&  dataModel.data.Length > 0)
        {
            Debug.Log("All Task vao");

            List<TaskDto> allTasks = new List<TaskDto>();

            foreach (EventData eventData in dataModel.data)
            {
                foreach (TaskDto task in eventData.taskDtos)
                {
                    allTasks.Add(task);
                }
            }
            Debug.Log("All Task" + allTasks.Count);
            return allTasks.ToArray();
        }

        return null;
    }
    public string GetFirstStartTime()
    {
        if (dataModel != null && dataModel.data.Length > 0)
        {
            return dataModel.data[0].startTime;
        }

        return null;
    }
    public bool IsDataLoaded()
    {
        return isDataLoaded;
    }
}