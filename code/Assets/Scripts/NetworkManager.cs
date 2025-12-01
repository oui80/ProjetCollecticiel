using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string roomName = "SharedSceneRoom";

    private void Start()
    {
        // Make sure all users load the same scene when the host changes scenes
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master");
        PhotonNetwork.JoinOrCreateRoom(
            roomName,
            new RoomOptions { MaxPlayers = 10 },
            TypedLobby.Default
        );
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + roomName);

        // Only spawn the cube if we don't already have one
        if (GameObject.FindWithTag("NetworkedCube") == null)
        {
            // Make sure your prefab is named exactly "NetworkedCube"
            // and lives in a Resources folder.
            GameObject cube = PhotonNetwork.Instantiate(
                "NetworkedCube",
                new Vector3(0, 0, 2f),  // initial position in front of user
                Quaternion.identity
            );
        }
    }

}