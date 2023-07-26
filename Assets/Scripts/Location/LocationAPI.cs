using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LocationAPI : MonoBehaviour
{
    private string apiUrl = "http://anhkiet-001-site1.htempurl.com/api/Locations";
    private static LocationAPI instance;
    public static LocationAPI GetInstance()
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
    }
    public IEnumerator GetLocationName(string locationId, Action<string> callback)
    {
        string url = $"http://anhkiet-001-site1.htempurl.com/api/Locations/{locationId}";
        Debug.Log(url);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                // Parse JSON response to extract "data" array
                LocationDataWrapper wrapper = JsonUtility.FromJson<LocationDataWrapper>(response);
                Debug.Log(wrapper.data.locationName);
                // Access the properties of majorData
                string locationName = wrapper.data.locationName;
                Debug.Log("Location Name: " + locationName);

                // Call the callback function with the majorName
                callback?.Invoke(locationName);
            }
            else
            {
                Debug.LogError("API call failed. Error: " + webRequest.error);
            }
        }
    }

    public IEnumerator GetFullLocationName(Action<List<string>> callback)
    {
        string url = $"http://anhkiet-001-site1.htempurl.com/api/Locations";
        Debug.Log(url);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;
                // Parse JSON response to extract "data" array
                LocationListDataWrapper wrapper = JsonUtility.FromJson<LocationListDataWrapper>(response);
                List<string> locationNames = new List<string>();

                foreach (LocationData locationData in wrapper.data)
                {
                    locationNames.Add(locationData.locationName);
                }

                Debug.Log("Location Names: " + string.Join(", ", locationNames));

                // Call the callback function with the location names list
                callback?.Invoke(locationNames);
            }
            else
            {
                Debug.LogError("API call failed. Error: " + webRequest.error);
            }
        }
    }
}
