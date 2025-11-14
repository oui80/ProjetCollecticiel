using Photon.Pun;
using UnityEngine;

public class PhotonConnection : MonoBehaviourPunCallbacks
{
    void Start()
    {
        Debug.Log("Connexion à Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connecté au serveur Photon !");
        PhotonNetwork.JoinLobby(); // permet de voir les salons
    }
}
