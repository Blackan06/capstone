using Photon.Pun;
using UnityEngine;

public class ExitRoom : MonoBehaviour, Interactable
{
    [SerializeField] private string prompt;
    public string InteractionPromp => prompt;

    public bool Interact(Interactor interactor)
    {
        Debug.Log("Contact Exit Room");
        PhotonNetwork.LoadLevel("Playground1");
        return true;
    }
}
