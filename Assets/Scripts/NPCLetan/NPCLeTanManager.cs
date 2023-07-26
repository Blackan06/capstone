using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class NPCLeTanManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject toActivate;
 

    [SerializeField] private Transform standingPoint;

    private Transform avatar;

    private async void OnTriggerEnter(Collider other)
    {
        PhotonView photonView = other.GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            return;
        }

        avatar = other.transform;

        // disable player input
        /*            avatar.GetComponent<PlayerInput>().enabled = false;
        */
        await Task.Delay(50);

        // teleport the avatar to standing point
        avatar.position = standingPoint.position;
        avatar.rotation = standingPoint.rotation;

        // disable main cam, enable dialog cam
        toActivate.SetActive(true);
       
        // d?splay cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
            
    }

    public void Recover()
    {
        /*        avatar.GetComponent<PlayerInput>().enabled = true;
        */
        toActivate.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
    }
}
