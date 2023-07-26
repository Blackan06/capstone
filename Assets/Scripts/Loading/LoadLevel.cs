using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviourPunCallbacks
{
    public GameObject loadingScreen;
    public Slider loadingSlider;
    public Text loadingText;
    public static LoadLevel Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLevels(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingScreen.SetActive(true);

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel(sceneName);

        while (!PhotonNetwork.LevelLoadingProgress.Equals(1f))
        {
            float progress = Mathf.Clamp01(PhotonNetwork.LevelLoadingProgress);
            loadingSlider.value = progress;
            loadingText.text = (progress * 100f).ToString("F0") + "%";

            yield return null;
        }
        PhotonNetwork.AutomaticallySyncScene = true;

    }
}
