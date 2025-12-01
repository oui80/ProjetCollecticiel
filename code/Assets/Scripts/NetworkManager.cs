using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string roomName = "SharedSceneRoom";
    public TextMeshProUGUI  feedbackText;


    private void Start()
    {
        // Make sure all users load the same scene when the host changes scenes
        feedbackText.text = "Connecting...";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        LogFeedback("Connected to Photon Master");
        Debug.Log("Connected to Photon Master");
        PhotonNetwork.JoinOrCreateRoom(
            roomName,
            new RoomOptions { MaxPlayers = 10 },
            TypedLobby.Default
        );
    }
    
	void LogFeedback(string message)
	{
		if (feedbackText == null) {
			return;
		}

		feedbackText.text += System.Environment.NewLine+message;
	}

    private IEnumerator ClearFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        feedbackText.text = "";
    }

    public override void OnJoinedRoom()
    {
        LogFeedback("Joined room: " + roomName);
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
        StartCoroutine(ClearFeedbackAfterDelay(1f));
    }

}