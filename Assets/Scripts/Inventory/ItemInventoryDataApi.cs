using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemInventoryDataApi : MonoBehaviour
{
    
    private static ItemInventoryDataApi instance;

    public static ItemInventoryDataApi GetInstance()
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
        DontDestroyOnLoad(this.gameObject);

    }

    public IEnumerator GetItemInventoryByItemName(string itemName , Action<ItemInventoryData> callback)
    {
        string url = $"http://anhkiet-001-site1.htempurl.com/api/ItemInventorys/iteminventory/byname/{itemName}";

        Debug.Log(url);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                // Parse JSON response to extract "data" array
                Wrapper<ItemInventoryData> wrapper = JsonUtility.FromJson<Wrapper<ItemInventoryData>>(response);
                // Call the callback function with the majorName
                callback?.Invoke(wrapper.data);
            }
            else
            {
                Debug.LogError("API call failed. Error: " + webRequest.error);
            }
        }
    }


    string baseUrl = "http://anhkiet-001-site1.htempurl.com/api/ItemInventorys/itemInventory";
    public IEnumerator CreateItemInventory(ItemInventoryData itemData)
    {
        string json = JsonUtility.ToJson(itemData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(baseUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("POST Error: " + request.error);
        }
        else
        {
            Debug.Log("POST ItemInventoryData Success!");
        }
    }

    // Hàm PUT ?? c?p nh?t thông tin ItemInventoryData
    public IEnumerator UpdateItemInventory(ItemInventoryData itemData)
    {
        string json = JsonUtility.ToJson(itemData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        string url = $"http://anhkiet-001-site1.htempurl.com/api/ItemInventorys/{itemData.id}";

        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("PUT Error: " + request.error);
        }
        else
        {
            Debug.Log("PUT Success!");
        }
    }
}
