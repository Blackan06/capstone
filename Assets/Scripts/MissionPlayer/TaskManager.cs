using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using System;
using UnityEngine.Rendering.Universal;
using System.Net;

public class TaskManager : MonoBehaviour
{
    public GameObject taskPrefab;
    public Transform taskParent;
    public GameObject buttonUIMission;
    public GameObject UIMission;
    public List<TaskItem> taskItems;
    private static TaskManager instance;
    private TaskDto[] taskDtos;
    public EventLoader eventLoader;
    private bool tasksLoaded = false; 

    public static TaskManager Instance { get; private set; }
    private void Awake()
    {
        // Ki?m tra n?u ?ã có m?t instance t?n t?i, n?u có thì h?y b?n thân
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // Kh?i t?o instance
        Instance = this;

        // ??m b?o r?ng instance không b? h?y khi chuy?n c?nh
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        buttonUIMission.SetActive(true);
        UIMission.SetActive(false);
        StartCoroutine(LoadEventData());


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
        if (!tasksLoaded)
        {
            LoadTasks();

            tasksLoaded = true; // ?ánh d?u r?ng nhi?m v? ?ã ???c t?i
        }
    }
    public void UIMissions()
    {
        buttonUIMission.SetActive(false);
        UIMission.SetActive(true);

        if (!tasksLoaded)
        {
            tasksLoaded = true; // ?ánh d?u r?ng nhi?m v? ?ã ???c t?i
        }
    }

    public void recover()
    {
        buttonUIMission.SetActive(true);
        UIMission.SetActive(false);
    }

    private void LoadTasks()
    {
        if(taskDtos != null)
        {
            foreach (var task in taskDtos)
            {
                GameObject taskObject = Instantiate(taskPrefab, taskParent);
                TaskItem taskItem = taskObject.GetComponent<TaskItem>();
                StartCoroutine(taskItem.WaitForDataAndInitialize(task,this));
                taskItems.Add(taskItem);
            }
        }
        else
        {
            Debug.Log("task dto is null");
        }
    }

    
   
    public IEnumerator CheckTaskById(string taskId, Action<string> callback)
    {
        string url = $"http://anhkiet-001-site1.htempurl.com/api/Tasks/{taskId}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // G?i yêu c?u và ch? ph?n h?i t? API
            yield return request.SendWebRequest();

            // Ki?m tra ph?n h?i t? API
            if (request.result == UnityWebRequest.Result.Success)
            {
                // Phân tích ph?n h?i t? API ?? xác ??nh ng??i dùng
                string response = request.downloadHandler.text;
                // Parse JSON response to extract "data" array
                TaskDataWrapperDetail taskDataWrapper = JsonUtility.FromJson<TaskDataWrapperDetail>(response);
                if (taskDataWrapper != null && taskDataWrapper.data.id != null)
                {
                    callback?.Invoke(taskDataWrapper.data.id);

                }
            }
            else
            {
                Debug.LogError("API call failed. Error: " + request.error);
            }
        }
    }

}