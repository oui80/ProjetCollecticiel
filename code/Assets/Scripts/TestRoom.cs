using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomTest : MonoBehaviourPunCallbacks
{
    void OnGUI()
    {
        if (GUILayout.Button("Créer une salle"))
        {
            PhotonNetwork.CreateRoom("MaSalleTest");
        }

        if (GUILayout.Button("Rejoindre une salle"))
        {
            PhotonNetwork.JoinRoom("MaSalleTest");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Tu es dans la salle !");
    }
}
