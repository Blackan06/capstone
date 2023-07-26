using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class EnterRoom : MonoBehaviourPunCallbacks, Interactable
{
    [SerializeField] private string typeRoom = "1";
    [SerializeField] private string prompt;

    private LoadPrefab loadPrefab;
    private PhotonView photonView;
    Hashtable customProperties = new Hashtable();

    public string InteractionPromp => prompt;

    private void Start()
    {
        loadPrefab = FindObjectOfType<LoadPrefab>();
        photonView = GetComponent<PhotonView>();
    }

    public bool Interact(Interactor interactor)
    {
        if (photonView.IsMine)
        {
            Debug.Log("Contact Enter Room");
            loadPrefab?.SetLocationSignal(gameObject.name);
            if (typeRoom.Equals("1"))
            {
                Debug.Log("Load classroom type 1");
                PhotonNetwork.LoadLevel("RoomType1");
            }
            else if (typeRoom.Equals("2"))
            {
                Debug.Log("Load classroom type 2");
                PhotonNetwork.LoadLevel("RoomType1");
            }
            else if (typeRoom.Equals("3"))
            {
                Debug.Log("Load classroom type 3");
                PhotonNetwork.LoadLevel("RoomType3");
            }
            else if (typeRoom.Equals("4"))
            {
                Debug.Log("Load classroom type 4");
                PhotonNetwork.LoadLevel("RoomType4");
            }
            else if (typeRoom.Equals("5"))
            {
                Debug.Log("Load hội trường");
                PhotonNetwork.LoadLevel("Hall A");
            }
            else if (typeRoom.Equals("Hall B tầng 4"))
            {
                customProperties.Add("Signal", gameObject.name);
                CustomPropertiesManager.SetCustomProperties(PhotonNetwork.LocalPlayer.UserId, customProperties);
                Debug.Log("Load hội trường B tầng 4 cho ID: " + PhotonNetwork.LocalPlayer.UserId);
                PhotonNetwork.LoadLevel("Hall B");
            }
            else if (typeRoom.Equals("Hall B tầng 5"))
            {
                customProperties.Add("Signal", gameObject.name);
                CustomPropertiesManager.SetCustomProperties(PhotonNetwork.LocalPlayer.UserId, customProperties);
                Debug.Log("Load hội trường B tầng 5 cho ID: " + PhotonNetwork.LocalPlayer.UserId);
                PhotonNetwork.LoadLevel("Hall B");
            }
            else if (typeRoom.Equals("711 A"))
            {
                customProperties.Add("Signal", gameObject.name);
                CustomPropertiesManager.SetCustomProperties(PhotonNetwork.LocalPlayer.UserId, customProperties);
                Debug.Log("Load 711 cửa A cho ID: " + PhotonNetwork.LocalPlayer.UserId);
                PhotonNetwork.LoadLevel("711");
            }
            else if (typeRoom.Equals("711 B"))
            {
                customProperties.Add("Signal", gameObject.name);
                CustomPropertiesManager.SetCustomProperties(PhotonNetwork.LocalPlayer.UserId, customProperties);
                Debug.Log("Load 711 cửa B cho ID: " + PhotonNetwork.LocalPlayer.UserId);
                PhotonNetwork.LoadLevel("711");
            }
            else
            {
                Debug.Log("không có loại phòng");
            }
            return true;
        }
        return false;
    }
}
