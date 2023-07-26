using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class TaskItem : MonoBehaviour
{
    public Text taskText;
    public Text timeStartText;
    public Text timeEndText;
    public Text rewardText;
    public Text LocationNameText;
    public Text NPCNameText;
    public string taskId;
   
    private NamePrefab namePrefab;
    private PlayerHistoryAPI playerHistoryAPI;
    private TaskManager taskManager;
    public Image completedIcon;
    public TaskDto taskDto;
    private bool isSuccess = false;
    private string status;
    private string playerId;
    private string startTime;
    private bool isDataReady = false; 

    // G?i ph??ng th?c n�y khi kh?i t?o TaskItem t? prefab
    private void Awake()
    {
        // G�n c�c th�nh ph?n Text, Image, v� GameObject trong prefab
        taskText = transform.Find("TaskText").GetComponent<Text>();
        timeStartText = transform.Find("TimeStartText").GetComponent<Text>();
        timeEndText = transform.Find("TimeEndText").GetComponent<Text>();
        rewardText = transform.Find("RewardText").GetComponent<Text>();
        completedIcon = transform.Find("CompletedIcon").GetComponent<Image>();
        LocationNameText = transform.Find("LocationNameText").GetComponent<Text>();
        NPCNameText = transform.Find("NPCNameText").GetComponent<Text>();
        if (playerHistoryAPI == null || namePrefab == null)
        {
            // N?u ch?a kh?i t?o, t�m ??i t??ng MajorAPI trong c?nh
           
            playerHistoryAPI = FindObjectOfType<PlayerHistoryAPI>();
           
            namePrefab = FindObjectOfType<NamePrefab>();
        }
    }
    private void Start()
    {
        startTime = EventLoader.Instance.GetFirstStartTime();
        if (playerHistoryAPI == null)
        {
            playerHistoryAPI = GetComponent<PlayerHistoryAPI>();
        }
        isDataReady = true; // ?�nh d?u r?ng d? li?u ?� s?n s�ng


    }
    private IEnumerator WaitForNamePrefabData()
    {
        while (namePrefab == null && playerHistoryAPI == null) // Ki?m tra xem namePrefab ?� c� d? li?u ch?a
        {
            yield return null; // Ch? 1 frame ?? ki?m tra l?i
        }

        // T?i ?�y, namePrefab ?� c� d? li?u, ti?p t?c g?i LoadTasks()
        StartCoroutine(namePrefab.CheckPlayerByEmail(OnGetPlayerId));


    }
    public void Initialize(TaskDto data, TaskManager manager)
    {
     

        if (!isDataReady)
        {
            // N?u ch?a s?n s�ng, ??i cho ??n khi d? li?u ???c s?n s�ng
            StartCoroutine(WaitForNamePrefabData());
            StartCoroutine(playerHistoryAPI.GetPlayerHistoryByTaskIdAndPlayerId(data.id, playerId, OnStatus));

            return;
        }
        taskDto = data;
        taskId = data.id;
        Debug.Log("Task ID" + data.id); Debug.Log("namePrefab" + namePrefab);
        taskManager = manager;
        LocationNameText.text = data.locationName;
        NPCNameText.text = data.npcName;
        taskText.text = data.name;
        rewardText.text = data.point.ToString();
        timeEndText.text = data.durationCheckin;
        timeStartText.text = DateTime.Parse(startTime).TimeOfDay.ToString();

        if (status == "Success")
        {
            Debug.Log("go");
            completedIcon.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("don't go");

            completedIcon.gameObject.SetActive(false);
        }

        

    }

    public IEnumerator WaitForDataAndInitialize(TaskDto data, TaskManager manager)
    {
        // Ch? cho ??n khi d? li?u ?� s?n s�ng
        while (!isDataReady)
        {
            yield return null; // Ch? 1 frame ?? ki?m tra l?i
        }

        // G?i Initialize khi d? li?u ?� s?n s�ng
        Initialize(data, manager);
    }
    private void OnGetPlayerId(string PlayerId)
    {
        playerId = PlayerId;
    }
    private void OnStatus(string Status)
    {
        status = Status;
    }
  
   
    public void CheckCompletion(bool IsSuccess, int duration)
    {
        if (IsSuccess == true)
        {
            Debug.Log("Vao day di icon s");
            // Hi?n th? bi?u t??ng ho�n th�nh nhi?m v?
            // V� d?: completedIcon.SetActive(true);
            CompleteTask(duration);

            completedIcon.gameObject.SetActive(true);

        }
        else
        {
            // Hi?n th? bi?u t??ng ch?a ho�n th�nh nhi?m v?
            // V� d?: completedIcon.SetActive(false);
            completedIcon.gameObject.SetActive(false);
        }
    }
    public void CompleteTask(int duration)
    {
        Debug.Log("Id task Data " + taskDto.id);
        playerHistoryAPI.CheckTaskCompletionUpdate(taskDto.id, taskDto.point, duration);
    }


}
