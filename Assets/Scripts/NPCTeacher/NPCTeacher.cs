using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTeacher : MonoBehaviour
{
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject toActivate;
    [SerializeField] private Transform standingPoint;


    private void OnTriggerEnter(Collider other)
    {
/*        PhotonView photonView = GetComponent<PhotonView>();
*/    
    
        toActivate.SetActive(true);
    }

}
