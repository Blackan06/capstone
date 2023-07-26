using Google;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginFirebase : MonoBehaviourPunCallbacks
{
    private string imageUrl;
    private string webClientId = "668814474990-4p5vepvjsqil4e9cl8imk3covmv6evmo.apps.googleusercontent.com";
    public InputField userNameField;
    public InputField passWordField;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;
    public LoadLevel sceneLoader;

    private bool authenticationFinished = false;
    private bool joinedRoom = false;


    private GoogleSignInConfiguration configuration;
    private UserAPI userAPI;
    private PlayerAPI playerAPI;
    public GameObject LoginScreen, SignInScreen, CodeScreen;
    public Text errorText;
    private bool isPlayer = true;
    private bool isCheckPlay = true;
    // Defer the configuration creation until Awake so the web Client ID
    // Can be set via the property inspector in the Editor.
    void Awake()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true
        };
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        InitFirebase();
        PhotonNetwork.AutomaticallySyncScene = true;
        // T?m d?ng h?ng ??i tin nh?n c?a Photon
        PhotonNetwork.IsMessageQueueRunning = false;
        userAPI = GetComponent<UserAPI>();
        playerAPI = GetComponent<PlayerAPI>();
    }

    void InitFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }
    #region LoginGmail
    /*public void GoogleSignInClick()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Fault");
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Login Cancel");
        }
        else
        {
            Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithCredentialAsync was canceled");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    return;
                }

                user = auth.CurrentUser;
                LoginScreen.SetActive(false);
                CodeScreen.SetActive(false);
                SignInScreen.SetActive(false);

                // Ng??i ch?i t?n t?i

                authenticationFinished = true;
                PhotonNetwork.LocalPlayer.NickName = user.Email;
                ConnectToPhoton();
                *//*// Ki?m tra quy?n ?i?u khi?n tr??c khi k?t n?i v?i Photon
                    if (authenticationFinished && !joinedRoom)
                    {
                        PhotonNetwork.JoinOrCreateRoom("Shared Room", new RoomOptions { MaxPlayers = 10 }, TypedLobby.Default);
                        joinedRoom = true;
                    }*//*
            });
        }
    }*/
    #endregion
    #region CheckIsUser
    public IEnumerator CheckUserByEmail(string userName, string passWord)
    {
        string url = $"http://anhkiet-001-site1.htempurl.com/api/Users/user/{userName}/{passWord}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                UserDataWrapper userDataWrapper = JsonUtility.FromJson<UserDataWrapper>(response);

                if (userDataWrapper != null && userDataWrapper.data.id != null)
                {

                    string playerUrl = $"http://anhkiet-001-site1.htempurl.com/api/Players/user/{userDataWrapper.data.id}";
                    using (UnityWebRequest playerRequest = UnityWebRequest.Get(playerUrl))
                    {
                        yield return playerRequest.SendWebRequest();

                        if (playerRequest.result == UnityWebRequest.Result.Success)
                        {
                            string playerResponse = playerRequest.downloadHandler.text;
                            PlayerDataWrapper playerDataWrapper = JsonUtility.FromJson<PlayerDataWrapper>(playerResponse);
                            if (playerDataWrapper != null && playerDataWrapper.data.id != null)
                            {
                                Debug.Log("Ng??i ch?i t?n t?i");
                                Debug.Log("player nick name" + playerDataWrapper.data.nickname);
                                PhotonNetwork.NickName = playerDataWrapper.data.nickname;
                                isPlayer = true;
                                isCheckPlay = true;
                            }
                            else
                            {

                                Debug.Log("Ng??i ch?i kh?ng t?n t?i");
                                PhotonNetwork.NickName = userDataWrapper.data.username;

                                isPlayer = false;
                                isCheckPlay = true;

                            }
                        }
                        else
                        {
                            isCheckPlay = false;

                            Debug.LogError("L?i khi g?i y?u c?u API: " + playerRequest.error);
                        }
                    }

                }
                else
                {
                    isCheckPlay = false;
                    errorText.gameObject.SetActive(true);
                    errorText.text = "T?i kho?n ho?c m?t kh?u kh?ng ??ng";
                    Debug.Log("Kh?ng t?n t?i");
                    userNameField.text = "";
                    passWordField.text = "";
                }
            }
            else
            {
                isCheckPlay = false;
                Debug.LogError("L?i khi g?i y?u c?u API: " + request.error);
            }
        }
    }


    #endregion


    public void Logout()
    {
        if (auth != null)
        {
            auth.SignOut();
            GoogleSignIn.DefaultInstance.SignOut();

            // X?a d? li?u ng??i d?ng hi?n t?i
            user = null;

            // Th?c hi?n c?c t?c ??ng giao di?n ng??i d?ng sau khi ??ng xu?t

            // Hi?n th? m?n h?nh ??ng nh?p
            LoginScreen.SetActive(true);
            CodeScreen.SetActive(false);
            SignInScreen.SetActive(false);
        }
    }


    public void SignIn()
    {
        LoginScreen.SetActive(false);
        CodeScreen.SetActive(false);
        SignInScreen.SetActive(true);
    }

    public void SignInSuccess()
    {

        Debug.Log("Sigin In");
        string username = userNameField.text;
        string password = passWordField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.Log("Vui l?ng ?i?n ??y ?? th?ng tin.");
            errorText.gameObject.SetActive(true);
            errorText.text = "Vui long dien thong tin day du";
            return;
        }
        else
        {
            Debug.Log("sigin vao photon");

            StartCoroutine(CheckPlayer(username, password));

        }
    }
    private IEnumerator CheckPlayer(string userName, string password)
    {
        yield return StartCoroutine(CheckUserByEmail(userName, password));
        Debug.Log(userName);
        if (isCheckPlay)
        {
            LoginScreen.SetActive(false);
            CodeScreen.SetActive(false);
            authenticationFinished = true;
            ConnectToPhoton();
        }
        else
        {
            Debug.Log("Loi khong check ra");
        }
    }
    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser == null)
        {
            Debug.Log("success");
        }
    }

    public void OnDisconnect()
    {
        GoogleSignIn.DefaultInstance.Disconnect();
    }



    /* private string CheckImageUrl(string url)
     {
         if (!string.IsNullOrEmpty(url))
         {
             return url;
         }
         return imageUrl;
     }*/

    /* IEnumerator LoadImage(string imageUrl)
     {
         if (!string.IsNullOrEmpty(imageUrl))
         {
             WWW www = new WWW(imageUrl);
             yield return www;

             UserProfilePic.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
         }
     }*/

    #region Photon Callbacks

    private void DisconnectFromPhoton()
    {
        PhotonNetwork.LeaveRoom();

        PhotonNetwork.Disconnect();


    }
    public override void OnConnected()
    {
        Debug.Log("Connect to Internet");
    }

    private void ConnectToPhoton()
    {
        if (PhotonNetwork.IsConnected)
        {
            JoinRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = "1.0";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void JoinRoom()
    {
        if (joinedRoom)
            return;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;

        PhotonNetwork.JoinOrCreateRoom("Shared Room", roomOptions, TypedLobby.Default);
        joinedRoom = true;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon");


        JoinRoom();

    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            Debug.Log("Vao");
            if (isPlayer)
            {
                PhotonNetwork.LoadLevel("Playground1");

            }
            else
            {
                PhotonNetwork.LoadLevel("TutorialNPCSence");


            }
        }
        else
        {
            Debug.Log("Ra");
        }
    }
    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Người chơi đã tham gia: " + newPlayer.nickname);
        // Cập nhật danh sách người chơi hiện có trong game
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Người chơi đã rời phòng: " + otherPlayer.nickname);
        // Cập nhật danh sách người chơi hiện có trong game
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from Photon Server: " + cause);
    }

    #endregion

}
