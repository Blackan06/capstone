using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class ItemApi : MonoBehaviour
{
    public static ItemApi Instance;
    private const string apiURL = "http://anhkiet-001-site1.htempurl.com/api/Items";
    public List<ItemData> items;
    public string ImageUrl;
    // Khai báo delegate và event ?? thông báo khi d? li?u ?ã s?n sàng
    public delegate void OnDataLoaded();
    public static event OnDataLoaded onDataLoaded;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        // Kh?i t?o danh sách các m?c
        items = new List<ItemData>();

        // G?i hàm ?? th?c hi?n yêu c?u API và l?y danh sách các m?c
        StartCoroutine(GetItemsFromAPI());
    }
    public IEnumerator GetItemsFromAPI()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiURL))
        {
            // G?i yêu c?u ??n API
            yield return webRequest.SendWebRequest();

            // Ki?m tra n?u có l?i trong quá trình g?i yêu c?u
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("L?i khi g?i API: " + webRequest.error);
            }
            else
            {
                // X? lý d? li?u tr? v? t? API
                string jsonResult = webRequest.downloadHandler.text;
                items = JsonUtility.FromJson<ItemDataWrapper>(jsonResult).data;

                // G?i event ?? thông báo d? li?u ?ã s?n sàng
                if (onDataLoaded != null)
                {
                    onDataLoaded();
                }

                // Hi?n th? thông tin các m?c (ho?c làm b?t c? ?i?u gì b?n mu?n v?i d? li?u)
                foreach (ItemData item in items)
                {
                    Debug.Log("ID: " + item.id + ", Name: " + item.name + ", Price: " + item.price);
                    // Ti?p t?c hi?n th? các thu?c tính khác c?a item n?u c?n thi?t
                }
            }
        }
    }
    public IEnumerator CheckItemById(string id,Action<ItemData> callback)
    {
        string url = $"http://anhkiet-001-site1.htempurl.com/api/Items/{id}";
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
                Wrapper<ItemData> playerDataWrapper = JsonUtility.FromJson<Wrapper<ItemData>>(response);
                if (playerDataWrapper != null && playerDataWrapper.data.id != null)
                {
                    Debug.Log(playerDataWrapper.data);
                    callback?.Invoke(playerDataWrapper.data);

                }
            }
            else
            {
                Debug.LogError("API call failed. Error: " + request.error);
            }
        }
    }
   
}
