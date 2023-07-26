using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class LoadHallB : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private GameObject playerPrefab;

    Vector3 doorA = new Vector3(-5, 1, 5);
    Vector3 doorB = new Vector3(-5, 8, -9);

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
                    if (signal.Equals("Hall B tầng 4"))
                    {
                        PhotonNetwork.Instantiate(playerPrefab.name, doorA, Quaternion.identity);
                    }
                    else if (signal.Equals("Hall B tầng 5"))
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
