using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class NamePrefab : MonoBehaviourPunCallbacks
{
    public TextMeshPro textName;
    // Start is called before the first frame update
    private string playerNickname;
    private string PlayerId;
    public double Point;
    private static NamePrefab instance;
    public GetItemInventoryByPlayer inventories;
    public List<ItemData> itemDatas;
    public List<ItemInventoryData> itemInventoryDatas;
    public static NamePrefab GetInstance()
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
    void Start()
    {
        playerNickname = PhotonNetwork.LocalPlayer.NickName;
        Debug.Log("Player nickname: " + playerNickname);
        textName.text = playerNickname;
        StartCoroutine(CheckPlayerByEmail(OnPlayerIdReceived));
        StartCoroutine(CheckPlayerByPoint(OnGetPoint));
        StartCoroutine(GetlistItemInventoryByPlayerName(OnListItemInventory));
        StartCoroutine(GetlistItemByPlayerName(OnGetListItem));
        StartCoroutine(GetlistInventoryByPlayerName(OnGetItemInventoryList));
    }
    public void OnGetListItem(List<ItemData> listData)
    {
        itemDatas = listData;
    }
    public IEnumerator GetlistItemByPlayerName(Action<List<ItemData>> callback)
    {
        string url = $"http://anhkiet-001-site1.htempurl.com/api/ItemInventorys/iteminventory/{playerNickname}";

        Debug.Log(url);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                // Parse JSON response to extract "data" array
                Wrapper<GetItemInventoryByPlayer> wrapper = JsonUtility.FromJson<Wrapper<GetItemInventoryByPlayer>>(response);
                // Call the callback function with the majorName
                Debug.Log("data " + wrapper.data.listItem);
                callback?.Invoke(wrapper.data.listItem);
            }
            else
            {
                Debug.LogError("API call failed. Error: " + webRequest.error);
            }
        }
    }
    public IEnumerator GetlistInventoryByPlayerName(Action<List<ItemInventoryData>> callback)
    {
        string url = $"http://anhkiet-001-site1.htempurl.com/api/ItemInventorys/iteminventory/{playerNickname}";

        Debug.Log(url);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                // Parse JSON response to extract "data" array
                Wrapper<GetItemInventoryByPlayer> wrapper = JsonUtility.FromJson<Wrapper<GetItemInventoryByPlayer>>(response);
                // Call the callback function with the majorName
                Debug.Log("data " + wrapper.data.listItem);
                callback?.Invoke(wrapper.data.listItemInventory);
            }
            else
            {
                Debug.LogError("API call failed. Error: " + webRequest.error);
            }
        }
    }
    private void OnGetItemInventoryList(List<ItemInventoryData> listitemInventoryDatas)
    {
        itemInventoryDatas = listitemInventoryDatas;
    }

    public IEnumerator GetlistItemInventoryByPlayerName(Action<GetItemInventoryByPlayer> callback)
    {
        string url = $"http://anhkiet-001-site1.htempurl.com/api/ItemInventorys/iteminventory/{playerNickname}";

        Debug.Log(url);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                // Parse JSON response to extract "data" array
                Wrapper<GetItemInventoryByPlayer> wrapper = JsonUtility.FromJson<Wrapper<GetItemInventoryByPlayer>>(response);
                // Call the callback function with the majorName
                Debug.Log("data " + wrapper.data.listItem);
                callback?.Invoke(wrapper.data);
            }
            else
            {
                Debug.LogError("API call failed. Error: " + webRequest.error);
            }
        }
    }
    private void OnListItemInventory(GetItemInventoryByPlayer ItemInventorys)
    {
        inventories = ItemInventorys;
    }

    public IEnumerator GetPlayer(string playerId, Action<string> callback)
    {
        string url = $"http://anhkiet-001-site1.htempurl.com/api/Players/{playerId}";
        Debug.Log(url);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                // Parse JSON response to extract "data" array
                PlayerDataWrapper wrapper = JsonUtility.FromJson<PlayerDataWrapper>(response);
                Debug.Log(wrapper.data.id);
                PlayerId = wrapper.data.id;
                // Call the callback function with the majorName
                callback?.Invoke(wrapper.data.id);
            }
            else
            {
                Debug.LogError("API call failed. Error: " + webRequest.error);
            }
        }
    }
    public IEnumerator CheckPlayerByEmail(Action<string> callback)
    {
        string url = $"http://anhkiet-001-site1.htempurl.com/api/Players/player/player-{playerNickname}";
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
                PlayerDataWrapper playerDataWrapper = JsonUtility.FromJson<PlayerDataWrapper>(response);
                if (playerDataWrapper != null && playerDataWrapper.data.id != null)
                {

                    callback?.Invoke(playerDataWrapper.data.id);

                }
            }
            else
            {
                Debug.LogError("API call failed. Error: " + request.error);
            }
        }
    }   
    public IEnumerator CheckPlayerByPoint(Action<double> callback)
    {
        string url = $"http://anhkiet-001-site1.htempurl.com/api/Players/player/player-{playerNickname}";
        Debug.Log("NickName" + playerNickname);
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
                PlayerDataWrapper playerDataWrapper = JsonUtility.FromJson<PlayerDataWrapper>(response);
                if (playerDataWrapper != null && playerDataWrapper.data.id != null)
                {
                    callback?.Invoke(playerDataWrapper.data.totalPoint);

                }
            }
            else
            {
                Debug.LogError("API call failed. Error: " + request.error);
            }
        }
    }
    public IEnumerator CheckPlayer(Action<Player> callback)
    {
        string url = $"http://anhkiet-001-site1.htempurl.com/api/Players/player/player-{playerNickname}";
        Debug.Log("NickName" + playerNickname);
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
                PlayerDataWrapper playerDataWrapper = JsonUtility.FromJson<PlayerDataWrapper>(response);
                if (playerDataWrapper != null && playerDataWrapper.data.id != null)
                {
                    callback?.Invoke(playerDataWrapper.data);

                }
            }
            else
            {
                Debug.LogError("API call failed. Error: " + request.error);
            }
        }
    }
    public void PutDataPlayer(string url,string jsonData)
    {
        StartCoroutine(PutData(url,jsonData));
    }
    public IEnumerator UpdatePlayer(Player itemData)
    {
        string json = JsonUtility.ToJson(itemData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        string url = $"http://anhkiet-001-site1.htempurl.com/api/Players/{itemData.id}";

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
    public IEnumerator PutData(string url,string jsonData)
    {
        using (UnityWebRequest www = UnityWebRequest.Put(url, jsonData))
        {
            www.SetRequestHeader("Content-Type", "application/json");

            // G?i yêu c?u PUT ??n server
            yield return www.SendWebRequest();

            // Ki?m tra l?i trong quá trình g?i yêu c?u
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"PUT Error: {www.error}");
            }
            else
            {
                // Yêu c?u thành công, x? lý ph?n h?i t? server
                Debug.Log("PUT Request Successful!");
            }
        }
    }
    private void OnGetPoint(double point)
    {
        Point = point;
    }
    private void OnPlayerIdReceived(string playerId)
    {
        // X? lý playerId nh?n ???c t?i ?ây
        Debug.Log("Player Id: " + playerId);
    }
}
