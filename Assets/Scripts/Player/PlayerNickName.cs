using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerNickName : MonoBehaviour
{
    private int playerScore;
    [SerializeField] private InputField inputField;
    [SerializeField] private Text errorText;
    private string playerNickName;
    // Start is called before the first frame update
    void Start()
    {
        playerNickName = PhotonNetwork.LocalPlayer.NickName;
        Debug.Log("Player nickname: " + playerNickName);
    }
    public void EnterFieldText()
    {
        StartCoroutine(CheckUserByEmailToUpdatePoint(playerNickName));

    }
    public IEnumerator CheckUserByEmailToUpdatePoint(string userName)
    {
        bool isInputValid = false;
        string url = $"http://anhkiet-001-site1.htempurl.com/api/Users/user/{userName}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // G?i yêu c?u và ch? ph?n h?i t? API
            yield return request.SendWebRequest();

            // Ki?m tra ph?n h?i t? API
            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;

                // Phân tích ph?n h?i t? API ?? xác ??nh ng??i dùng
                UserDataWrapper userDataWrapper = JsonUtility.FromJson<UserDataWrapper>(response);
                Debug.Log(userDataWrapper.data.email);
                Debug.Log(userDataWrapper.data.id);
                Debug.Log(QuestionDialog.scoreValue);
                Debug.Log(inputField.text);
                if (userDataWrapper != null)
                {
                    string playerNickNameUrl = $"http://anhkiet-001-site1.htempurl.com/api/Players/player/player-{inputField.text}";
                    using (UnityWebRequest playerNickNameRequest = UnityWebRequest.Get(playerNickNameUrl))
                    {
                        yield return playerNickNameRequest.SendWebRequest();

                        if (playerNickNameRequest.result == UnityWebRequest.Result.Success)
                        {
                            string playerNickResponse = playerNickNameRequest.downloadHandler.text;
                            PlayerDataWrapper playerDataNickWrapper = JsonUtility.FromJson<PlayerDataWrapper>(playerNickResponse);
                            Debug.Log(playerDataNickWrapper.data.id);
                            if (playerDataNickWrapper.data.nickname == null)
                            {
                                Debug.Log("Vao khong nguoi oi");
                                isInputValid = true; // ??t isInputValid = true n?u thông tin ?úng

                                var player = new Player()
                                {
                                    userId = userDataWrapper.data.id,
                                    totalPoint = QuestionDialog.scoreValue,
                                    totalTime = 0.0,
                                    nickname = inputField.text,
                                };
                                string jsonString = JsonUtility.ToJson(player);
                                PhotonNetwork.NickName = player.nickname;
                                StartCoroutine(PostRequest("http://anhkiet-001-site1.htempurl.com/api/Players/player", jsonString));
                              

                            }
                            else
                            {
                                errorText.gameObject.SetActive(true);
                                errorText.text = "Nick name b? trùng";
                            }
                        }
                        else
                        {
                            // X?y ra l?i khi g?i yêu c?u API l?y thông tin ng??i ch?i
                            Debug.Log("L?i khi g?i yêu c?u API: " + playerNickNameRequest.error);
                        }
                    }
                }
                else
                {
                    Debug.Log("User không t?n t?i");
                }
            }
            else
            {
                // X?y ra l?i khi g?i yêu c?u API
                Debug.Log("L?i khi g?i yêu c?u API: " + request.error);
            }
        }

        // Ki?m tra isInputValid và g?i l?i hàm n?u thông tin không ?úng
        if (!isInputValid)
        {
            yield return new WaitForSeconds(5f);
            StartCoroutine(CheckUserByEmailToUpdatePoint(userName));
        }
    }

    public IEnumerator PostRequest(string playerUrl, string jsonData)
    {
        var request = new UnityWebRequest(playerUrl, "POST");
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
           
            var playerDataWrapper = JsonUtility.FromJson<Wrapper<string>>(request.downloadHandler.text);
            if (playerDataWrapper != null && playerDataWrapper.data != null)
            {
                string playerId = playerDataWrapper.data;
                Debug.Log(playerDataWrapper.data);
                if (playerId != null)
                {
                    var inventory = new InventoryData()
                    {
                        playerId = playerId
                    };
                    string jsonStringData = JsonUtility.ToJson(inventory);
                    StartCoroutine(PostInventory("http://anhkiet-001-site1.htempurl.com/api/Inventorys/inventory", jsonStringData));
                }
                else
                {
                    Debug.LogError("playerDataWrapper.data.id là null ho?c không h?p l?.");
                }



            }

        }
    }
    public IEnumerator PostInventory(string inventoryUrl, string jsonData)
    {
        var request = new UnityWebRequest(inventoryUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("L?i khi g?i yêu c?u API: " + request.error);
            Debug.LogError("Ph?n h?i t? server: " + request.downloadHandler.text); // In ra ph?n h?i t? server ?? xem thông báo l?i chi ti?t.
        }
        else
        {
            Debug.Log("Ph?n h?i t? server (Inventory): " + request.downloadHandler.text);
            PhotonNetwork.LoadLevel("Playground1");
        }
    }
}
