using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Task = System.Threading.Tasks.Task;

public class NPCReception : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject toActivate;

    private Transform avatar;





    private void OnTriggerEnter(Collider other)
    {
        PhotonView photonView = other.GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            return;
        }
        toActivate.SetActive(true);
      
    }
   
    public void Recover()
    {

        toActivate.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
