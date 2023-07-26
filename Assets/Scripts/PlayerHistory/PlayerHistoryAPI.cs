using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerHistoryAPI : MonoBehaviour
{
    private List<TaskItem> taskItems;
    private string playerID;
    private string eventID;
    private string startTime;
    private DateTime timeDateTime;
    private EventTaskAPI eventTaskAPI;
    private static PlayerHistoryAPI instance;
    private TaskDto[] taskDtos;

    public static PlayerHistoryAPI GetInstance()
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


        if ( eventTaskAPI == null)
        {
            eventTaskAPI = FindObjectOfType<EventTaskAPI>();
        }
    }
    private void Start()
    {
        if (TaskManager.Instance != null)
        {
            if (TaskManager.Instance.taskItems != null)
            {
                taskItems = TaskManager.Instance.taskItems;
                StartCoroutine(LoadEventData());
            }
            else
            {
                Debug.Log("Task Items is null");
            }
        }
        else
        {
            Debug.Log("TaskManager.Instance is null");
        }

    }
    private IEnumerator LoadEventData()
    {
        // ??i cho ??n khi d? li?u ???c t?i xong t? EventLoader
        while (!EventLoader.Instance.IsDataLoaded())
        {
            yield return null;
        }

        // D? li?u ?ã t?i xong, ti?p t?c x? lý d? li?u ? ?ây
        taskDtos = EventLoader.Instance.GetAllTaskData();
        startTime = EventLoader.Instance.GetFirstStartTime();
    }
    public void CheckTaskCompletionUpdate(string taskId, int taskPoint, int duration)
    {
        StartCoroutine(PostPlayerHistoryData(taskId, taskPoint, duration));
    }

    public IEnumerator PostPlayerHistoryData(string taskId, int taskPoint, int duration)
    {
        NamePrefab namePrefab = NamePrefab.GetInstance();
        if (namePrefab != null)
        {
            yield return StartCoroutine(namePrefab.CheckPlayerByEmail(OnPlayerIdReceived));
        }
        Debug.Log("Player: " + playerID);
        if (playerID != null)
        {
            string apiEndpoint = "http://anhkiet-001-site1.htempurl.com/api/PlayerHistorys/playerhistory";
            // T?o m?t PlayerHistoryDataWrapper và ??t d? li?u vào
            PlayerHistoryData playerHistoryData = new PlayerHistoryData()
            {
                taskId = taskId,
                playerId = playerID,
                taskPoint = taskPoint,
                status = "Success",
                duration = duration
            };


            // Chuy?n ??i PlayerHistoryDataWrapper thành JSON
            string jsonData = JsonUtility.ToJson(playerHistoryData);

            // T?o m?t UnityWebRequest ?? g?i POST request
            var request = new UnityWebRequest(apiEndpoint, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("L?i: " + request.error);
            }
            else
            {
                Debug.Log("Ph?n h?i t? server: " + request.downloadHandler.text);
            }
        }
        else
        {
            Debug.Log("Player bang null hoac khong lay duoc");
        }

    }
    private void OnPlayerIdReceived(string playerId)
    {
        playerID = playerId;
    }

    public void NotifyTaskCompletion(string taskID, string dateTimeSuccess)
    {
        // Tìm ki?m TaskItem t??ng ?ng trong danh  sách taskItems
        TaskItem taskItem = taskItems.FirstOrDefault(item => item.taskId == taskID);

        if (taskItem != null)
        {
            var task = taskDtos.FirstOrDefault(x => x.id == taskID);
            if(task == null)
            {
                Debug.Log("taskDtos is Null");

            }
            else 
            {
                DateTime dateTimeStart = DateTime.ParseExact(startTime, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                var TimeStartMission = dateTimeStart.TimeOfDay;
                TimeSpan endTime = DateTime.Parse(task.durationCheckin).TimeOfDay;
                if (DateTime.TryParseExact(dateTimeSuccess, "hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out timeDateTime))
                {
                    string time24HourFormat = timeDateTime.ToString("HH:mm:ss");
                    TimeSpan timeSuccess = DateTime.Parse(time24HourFormat).TimeOfDay;
                    Debug.Log("startTime" + TimeStartMission);
                    Debug.Log("Time" + timeSuccess);
                    Debug.Log("EndTime" + endTime);
                    if (timeSuccess >= TimeStartMission && timeSuccess <= endTime)
                    {
                        TimeSpan duration = timeSuccess - TimeStartMission;


                        taskItem.CheckCompletion(true, duration.Minutes);
                    }
                    else
                    {
                        taskItem.CheckCompletion(false, 0);
                    }

                }
                else if (DateTime.TryParseExact(dateTimeSuccess, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out timeDateTime))
                {
                    TimeSpan time = timeDateTime.TimeOfDay;

                    Debug.Log("startTime1 " + TimeStartMission);
                    Debug.Log("Time1 " + time);
                    Debug.Log("EndTime1 " + endTime);
                    if (time >= TimeStartMission && time <= endTime)
                    {
                        TimeSpan duration = time - TimeStartMission;
                        Debug.Log("Co vao duoc khong");

                        taskItem.CheckCompletion(true, duration.Minutes);
                    }
                    else
                    {
                        Debug.Log("Khong vao duoc khong");

                        taskItem.CheckCompletion(false, 0);
                    }
                }
            }
        }
        else
        {
            Debug.Log("Task Item is Null");
        }
    }
  

    public IEnumerator GetPlayerHistoryByTaskId(string taskId, Action<string> callback)
    {
        string url = $"http://anhkiet-001-site1.htempurl.com/api/PlayerHistorys/task/{taskId}";
        Debug.Log(url);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                // Parse JSON response to extract "data" array
                PlayerHistoryDataWrapper wrapper = JsonUtility.FromJson<PlayerHistoryDataWrapper>(response);
                Debug.Log(wrapper.data.status);
                // Access the properties of majorData
                string status = wrapper.data.status;
                Debug.Log("Status: " + status);

                // Call the callback function with the majorName
                callback?.Invoke(status);
            }
            else
            {
                Debug.LogError("API call failed. Error: " + webRequest.error);
            }
        }
    }

    public IEnumerator GetPlayerHistoryByTaskIdAndPlayerId(string taskId, string playerId, Action<string> callback)
    {
        string url = $"http://anhkiet-001-site1.htempurl.com/api/PlayerHistorys/task/{taskId}/{playerId}";
        Debug.Log(url);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                // Parse JSON response to extract "data" array
                PlayerHistoryDataWrapper wrapper = JsonUtility.FromJson<PlayerHistoryDataWrapper>(response);
                Debug.Log(wrapper.data.status);
                // Access the properties of majorData
                string status = wrapper.data.status;
                Debug.Log("Status: " + status);

                // Call the callback function with the majorName
                callback?.Invoke(status);
            }
            else
            {
                Debug.LogError("API call failed. Error: " + webRequest.error);
            }
        }
    }
}

