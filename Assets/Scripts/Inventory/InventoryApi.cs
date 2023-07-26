using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class InventoryApi : MonoBehaviour
{
    private static InventoryApi instance;
    public static InventoryApi Instance { get; private set; }

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
    public void PostDataInventory(string jsonData)
    {
        StartCoroutine(PostRequest(jsonData));
    }
    public IEnumerator PostRequest(string jsonData)
    {
        var request = new UnityWebRequest("http://anhkiet-001-site1.htempurl.com/api/Inventorys/inventory", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("L?i: " + request.error);
        }
        else
        {
            Debug.Log("Ph?n h?i t? server: " + request.downloadHandler.text);
        }
    }
}
