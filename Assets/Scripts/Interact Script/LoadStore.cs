using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class LoadStore : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private GameObject playerPrefab;

    Vector3 doorA = new Vector3(14, 1, 2);
    Vector3 doorB = new Vector3(-12, 1, 1);

    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {

        if (!PhotonNetwork.IsConnected || !photonView.IsMine)
        {
            // Nếu không phải người chơi local gọi script này, thoát khỏi hàm Start
            return;
        }

        if (photonView.IsMine)
        {
            Hashtable customProperties = CustomPropertiesManager.GetCustomProperties(PhotonNetwork.LocalPlayer.UserId);
            if (customProperties != null)
            {
                if (customProperties.ContainsKey("Signal"))
                {
                    string signal = customProperties["Signal"] as string;
                    Debug.Log("Signal: " + signal);
                    if (signal.Equals("711 A"))
                    {
                        PhotonNetwork.Instantiate(playerPrefab.name, doorA, Quaternion.identity);
                    }
                    else if (signal.Equals("711 B"))
                    {
                        PhotonNetwork.Instantiate(playerPrefab.name, doorB, Quaternion.identity);
                    }
                }
                else
                {
                    Debug.Log("Signal not found");
                }
            }
            else
            {
                Debug.Log("CustomProperties not found for UserID: " + PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }

    }
}
