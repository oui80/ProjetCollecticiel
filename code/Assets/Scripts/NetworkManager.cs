using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string roomName = "SharedSceneRoom";
    public TextMeshProUGUI  feedbackText;


    public GameObject[] prefabs;
    public float spacing = 0.3f;     // Distance between each object
    public Transform table;
    private List<GameObject> objects = new List<GameObject>();

    public QRCodeReader qrReader = new QRCodeReader();
    public GameObject qrCodeObject;
     
    private void Start()
    {
        // Make sure all users load the same scene when the host changes scenes
        feedbackText.text = "Connecting...";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    void Update()
    {
        foreach (GameObject obj in objects)
        {
            if (IsCloseToTable(obj))
            {
                SnapToSurface(obj);
            }
        }

        // Stop QR code tracking when it is detected by the Hololens
        if (qrReader.init)
        {
            qrCodeObject.SetActive(false);
        }

    }

    private void SnapToSurface(GameObject obj)
    {
        // Align the objects's position and rotation with the table's surface
        Vector3 tablePosition = table.position;
        obj.transform.position = new Vector3(obj.transform.position.x, tablePosition.y + obj.transform.localScale.x / 2, obj.transform.position.z);

        obj.transform.rotation = Quaternion.Euler(0, obj.transform.eulerAngles.y, 0);
    }

    private bool IsCloseToTable(GameObject obj)
    {
        // Project the object's position onto the table's vertical (Y-axis) plane
        Vector3 projectedPosition = new Vector3(obj.transform.position.x, table.position.y, obj.transform.position.z);

        // Calculate the distance between the object and its projection on the table
        float distanceToTable = Vector3.Distance(obj.transform.position, projectedPosition);

        return distanceToTable <= 0.07;
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

        if (PhotonNetwork.IsMasterClient)
        {
            LogFeedback("You are the Master Client");
            Debug.Log("You are the Master Client");
        }
        else
        {
            LogFeedback("You are a regular client");
            Debug.Log("You are a regular client");
            return;
        }

        // Only spawn shapes if they haven't been spawned yet
        if (GameObject.FindWithTag("NetworkedCube") == null)
        {
            // Make sure your prefab is named exactly "NetworkedCube"
            // and lives in a Resources folder.
            GameObject cube = PhotonNetwork.Instantiate(
                "NetworkedCube",
                new Vector3(0, 0, 2f),  // initial position in front of user
                Quaternion.identity
            );

            for (int i = 0; i < prefabs.Length; i++)
            {
                // First set of objects
                Vector3 newPosition = table.position + new Vector3(-table.localScale.x / 2, 0.4f, table.localScale.z / 2) + new Vector3(i * spacing, 0, 0);
                GameObject newObject = PhotonNetwork.Instantiate(prefabs[i].name, newPosition, Quaternion.identity);
                objects.Add(newObject);
                // Second set of objects
                Vector3 newPosition2 = table.position + new Vector3(-table.localScale.x / 2, 0.4f, -table.localScale.z / 2) + new Vector3(i * spacing, 0, 0);
                GameObject newObject2 = PhotonNetwork.Instantiate(prefabs[i].name, newPosition2, Quaternion.Euler(0, 90, 0));
                objects.Add(newObject2);

            }
        }
        StartCoroutine(ClearFeedbackAfterDelay(1f));
    }

}