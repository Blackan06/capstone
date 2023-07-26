using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoadPrefab : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private GameObject playerPrefab;
    private List<LocationData> locationDatas = new List<LocationData>();

    Vector3 defaultSpawn = new Vector3(-50, 2, 32);
    Vector3 spanwLocation = new Vector3(65, 0, 25);

    private string signal;
    private int vitri;

    private bool isAwakeCompleted = false;
    private void Awake()
    {
       

        StartCoroutine(GetFullLocationName(OnLocationDataReceived));
    }

    private void Start()
    {
        StartCoroutine(WaitForAwakeCompletion());
    }

    private IEnumerator WaitForAwakeCompletion()
    {
        while (!isAwakeCompleted)
        {
            yield return null; // Đợi một frame
        }
        if (isAwakeCompleted)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Signal", out object signalObj))
                {
                    signal = signalObj as string;
                    Debug.Log("Signal PlayGround: " + signal);
                    if (signal != null)
                    {
                        for (int i = 0; i < locationDatas.Count; i++)
                        {
                            if (locationDatas[i].locationName.Equals(signal))
                            {
                                vitri = i;
                            }
                        }
                        double x = locationDatas[vitri].x;
                        spanwLocation.x = float.Parse(x.ToString());
                        double y = locationDatas[vitri].y;
                        spanwLocation.y = float.Parse(y.ToString());
                        double z = locationDatas[vitri].z;
                        spanwLocation.z = float.Parse(z.ToString());
                        Debug.Log("vi tri: " + x + ", " + y + ", " + z);
                        Debug.Log("toa do: " + spanwLocation);
                    }
                    else
                    {
                        Debug.Log("Signal Null");
                    }
                }
                if (playerPrefab != null)
                {
                    if (signal == null)
                    {
                        PhotonNetwork.Instantiate(playerPrefab.name, defaultSpawn, Quaternion.identity);
                    }
                    else
                    {
                        PhotonNetwork.Instantiate(playerPrefab.name, spanwLocation, Quaternion.identity);
                    }

                }
                else
                {
                    Debug.Log("Player prefab is not assigned!");
                }
            }
        }
        else
        {
            Debug.Log("Not Done Yet");
        }
    }

    public string SetLocationSignal(string location)
    {
        Debug.Log("location name sign: " + location);
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "Signal", location } });

        return location;
    }

    public IEnumerator GetFullLocationName(Action<List<LocationData>> callback)
    {
        string url = $"http://anhkiet-001-site1.htempurl.com/api/Locations";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                // Parse JSON response to extract "data" array
                LocationListDataWrapper wrapper = JsonUtility.FromJson<LocationListDataWrapper>(response);
                List<LocationData> locationNames = new List<LocationData>();

                foreach (LocationData locationData in wrapper.data)
                {
                    locationNames.Add(locationData);
                }

                // Call the callback function with the location names list
                callback?.Invoke(locationNames);
            }
            else
            {
                Debug.LogError("API call failed. Error: " + webRequest.error);
            }
        }
    }

    void OnLocationDataReceived(List<LocationData> locationDataList)
    {
        // Xử lý danh sách LocationData ở đây
        foreach (LocationData locationData in locationDataList)
        {
            locationDatas.Add(locationData);
        }
        isAwakeCompleted = true;
    }
}
